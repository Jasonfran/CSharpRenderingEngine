using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Engine.Core;
using Engine.Resources;
using Engine.World;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Renderer
{
    public class OpenGLRendererCore : EngineSystem
    {
        private readonly EngineSystemsCollection _engineSystems;
        private WorldManager _worldManager;
        private WindowManager _windowManager;

        private Dictionary<string, List<BufferPointer>> _bufferDataPointers;
        private List<Vertex> _vertices;
        private List<uint> _indices;
        private int _initialBufferSize = 1024 * 1024 * Vertex.Stride; // 32MB of initial buffer storage

        private int _vao;
        private int _vboVertex;
        private int _eboIndices;
        private ResourceManager _resourceManager;

        private Shader testShader;

        private Matrix4 view;
        private Matrix4 projection;

        public OpenGLRendererCore(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
            _bufferDataPointers = new Dictionary<string, List<BufferPointer>>();
            _vertices = new List<Vertex>();
            _indices = new List<uint>();
        }

        public override void Init()
        {
            _windowManager = _engineSystems.GetSystem<WindowManager>();
            _windowManager.GetActiveWindow().MakeCurrent();

            _resourceManager = _engineSystems.GetSystem<ResourceManager>();

            GL.Viewport(0, 0, _windowManager.GetActiveWindow().Width, _windowManager.GetActiveWindow().Height);
            GL.Enable(EnableCap.DepthTest);

            testShader = _resourceManager.LoadShader("Shaders/Main.vert", "Shaders/Main.frag");

            testShader.Use();

            _vao = GL.GenVertexArray();
            _vboVertex = GL.GenBuffer();
            _eboIndices = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboVertex);
            GL.BufferData(BufferTarget.ArrayBuffer, _initialBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride,
                Marshal.OffsetOf(typeof(Vertex), "Normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Stride,
                Marshal.OffsetOf(typeof(Vertex), "TexCoords"));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _initialBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw);

            view = Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75.0f),
                (float)_windowManager.GetActiveWindow().Width / (float)_windowManager.GetActiveWindow().Height, 0.1f, 10000.0f);
        }

        private void LoadModelData(Model model)
        {
            var name = model.Path;
            var meshes = model.Meshes;

            _bufferDataPointers.Add(name, new List<BufferPointer>());

            foreach (var mesh in meshes)
            {
                var baseIndex = _vertices.Count;
                _vertices.AddRange(mesh.Vertices);
                _indices.AddRange(mesh.Indices);

                _bufferDataPointers[name].Add(new BufferPointer(baseIndex, mesh.Indices.Count));
            }

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vertex.Stride, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(),
                BufferUsageHint.StaticDraw);

            Console.WriteLine($"Loaded {model.Path} into VBO");
        }

        public void RenderModel(Model model, Matrix4 modelMatrix)
        {
            if (!_bufferDataPointers.ContainsKey(model.Path))
            {
                LoadModelData(model);
            }

            if (_bufferDataPointers.TryGetValue(model.Path, out var pointers))
            {
                foreach (var pointer in pointers)
                {
                    testShader.SetMat4("model", modelMatrix);
                    testShader.SetMat4("mvp", modelMatrix * view * projection);

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, pointer.count, DrawElementsType.UnsignedInt, (IntPtr)0, pointer.start);
                }
            }
        }

        public void FrameBegin()
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void FrameEnd()
        {

            _windowManager.GetActiveWindow().SwapBuffers();
        }
    }
}