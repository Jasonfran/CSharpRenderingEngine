using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Assimp;
using Engine.Core;
using Engine.Renderer;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using Material = Engine.Renderer.Material;
using Mesh = Engine.Renderer.Mesh;

namespace Engine.Resources
{
    public class ResourceManager : EngineSystem, IResourceManager
    {
        private readonly EngineSystemsCollection _engineSystems;
        private Dictionary<string, Model> _loadedModels = new Dictionary<string, Model>();
        private Dictionary<string, Shader> _loadedShaders = new Dictionary<string, Shader>();

        private string rootFolder = "Assets";
        private string modelFolder = "Models";

        public ResourceManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
        }

        public override void Init()
        {
        }

        public Shader LoadShader(string vertex, string fragment)
        {
            var key = Path.GetFileName(vertex) + '|' + Path.GetFileName(fragment);

            if (_loadedShaders.TryGetValue(key, out var shader))
            {
                return shader;
            }

            var vertexShaderCode = File.ReadAllText(Path.Combine(rootFolder, vertex));
            var fragmentShaderCode = File.ReadAllText(Path.Combine(rootFolder, fragment));

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, vertexShaderCode);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var vertexCompileSuccess);
            if (vertexCompileSuccess == 0)
            {
                var infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine("Error: Vertex shader compilation failed!");
                Console.WriteLine(infoLog);
            }

            GL.ShaderSource(fragmentShader, fragmentShaderCode);
            GL.CompileShader(fragmentShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var fragmentCompileSuccess);
            if (fragmentCompileSuccess == 0)
            {
                var infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine("Error: Fragment shader compilation failed!");
                Console.WriteLine(infoLog);
            }

            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var linkStatus);
            if (linkStatus == 0)
            {
                var infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine("Error: Program link failed!");
                Console.WriteLine(infoLog);
            }

            var completeShader = new Shader(program);

            _loadedShaders.Add(key, completeShader);

            return completeShader;
        }

        public Model LoadModel(string filePath)
        {
            string path = Path.Combine(rootFolder, filePath);

            if (_loadedModels.TryGetValue(path, out var mesh))
            {
                return mesh;
            }

            var model = new Model(filePath.ToLower());

            var context = new AssimpContext();
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate);

            if (scene.SceneFlags == SceneFlags.Incomplete)
            {
                throw new Exception("Assimp import error");
            }


            foreach (var aiMesh in scene.Meshes)
            {
                model.Meshes.Add(LoadMesh(aiMesh, scene));
            }

            
            //SetupModel(model);

            _loadedModels.Add(path, model);

            return model;
        }

        private Mesh LoadMesh(Assimp.Mesh mesh, Assimp.Scene scene)
        {
            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            for (var i = 0; i < mesh.Vertices.Count; i++)
            {
                var vert = new Vertex
                {
                    Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z),
                    Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z)
                };

                if (mesh.HasTextureCoords(0))
                {
                    var texCoords = new Vector2(mesh.TextureCoordinateChannels[0][i].X,
                        mesh.TextureCoordinateChannels[0][i].Y);
                    vert.TexCoords = texCoords;
                }
                else
                {
                    vert.TexCoords = new Vector2(0.0f);
                }

                vertices.Add(vert);
            }

            foreach (var face in mesh.Faces)
            {
                foreach (var i in face.Indices)
                {
                    indices.Add((uint)i);
                }
            }

            Material material = null;

            if (mesh.MaterialIndex >= 0)
            {
                material = LoadMaterial(scene.Materials[mesh.MaterialIndex]);
            }

            Console.WriteLine("Loaded mesh: " + mesh.Name);
            return new Mesh(vertices, indices, material);
        }

        private Material LoadMaterial(Assimp.Material mat)
        {
            var material = new Material();
            if (mat.HasTextureAmbient)
            {
                var texture = LoadTexture(mat.TextureAmbient.FilePath);
                if (texture != null)
                {
                    material.AmbientTexture = texture;
                }
            }
            else if (mat.HasColorAmbient)
            {
                material.AmbientColor = new Vector4(mat.ColorAmbient.R, mat.ColorAmbient.G, mat.ColorAmbient.B, mat.ColorAmbient.A);
            }

            if (mat.HasTextureDiffuse)
            {
                var texture = LoadTexture(mat.TextureDiffuse.FilePath);
                if (texture != null)
                {
                    material.DiffuseTexture = texture;
                }
            }
            else if (mat.HasColorDiffuse)
            {
                material.DiffuseColor = new Vector4(mat.ColorDiffuse.R, mat.ColorDiffuse.G, mat.ColorDiffuse.B, mat.ColorDiffuse.A);
            }

            if (mat.HasTextureSpecular)
            {
                var texture = LoadTexture(mat.TextureSpecular.FilePath);
                if (texture != null)
                {
                    material.SpecularTexture = texture;
                }
            }

            if (mat.HasColorSpecular)
            {
                material.SpecularColor = new Vector4(mat.ColorSpecular.R, mat.ColorSpecular.G, mat.ColorSpecular.B, mat.ColorSpecular.A);
            }

            return material;

        }

        private Texture2D LoadTexture(string filePath)
        {
            var texture = new Texture2D();

            filePath = Path.Combine(rootFolder, modelFolder, Path.GetFileName(filePath));
            try
            {
                var image = Image.Load(filePath);

                byte[] data;
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new BmpEncoder());
                    data = memoryStream.ToArray();
                }

                texture.Path = filePath;
                texture.TextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0,
                    PixelFormat.Bgr, PixelType.UnsignedByte, data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int) OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int) OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int) OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int) OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear);

                GL.BindTexture(TextureTarget.Texture2D, 0);
                return texture;
            }
            catch (FileNotFoundException exception)
            {
                Console.WriteLine($"Couldn't find file: {filePath}");
            }

            return null;
        }
    }
}