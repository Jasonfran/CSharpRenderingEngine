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
    public class RenderManager : EngineSystem
    {
        private readonly EngineSystemsCollection _engineSystems;
        private WorldManager _worldManager;
        private WindowManager _windowManager;

        private List<RenderCommand> _renderCommands;
        private Dictionary<string, List<BufferPointer>> _bufferDataPointers;
        private List<Vertex> _vertices;
        private List<uint> _indices;
        private int _initialBufferSize = 1024 * 1024 * Vertex.Stride; // 32MB of initial buffer storage
        private int _resizeBufferSize = 1024 * 1024 * Vertex.Stride; // 32MB extra buffer after resize


        private int _vboVertexSize = 0;
        private int _eboIndicesSize = 0;

        private int _vboVertexFill = 0;
        private int _eboIndicesFill = 0;

        private int _vao;
        private int _vboVertex;
        private int _eboIndices;
        private ResourceManager _resourceManager;

        private Shader testShader;

        public RenderManager(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _engineSystems = engineSystems;
            _renderCommands = new List<RenderCommand>();
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
            _vboVertexSize = _initialBufferSize;

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
            _eboIndicesSize = _initialBufferSize;
        }

        private void ResizeBuffer(BufferTarget target, int size)
        {
            GL.BufferData(target, size, IntPtr.Zero, BufferUsageHint.StaticDraw);
            Console.WriteLine($"Resized buffer {target}");
        }

        private void AddBufferDataPointer(string name, int start, int count)
        {
            if (!_bufferDataPointers.ContainsKey(name))
            {
                _bufferDataPointers.Add(name, new List<BufferPointer>());
            }

            _bufferDataPointers[name].Add(new BufferPointer(start, count));
        }

        private void LoadModelData(string name)
        {
            var model = _resourceManager.LoadModel(name);
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

        }

        public void SendRenderData(RenderCommand command)
        {
            _renderCommands.Add(command);
            if (!_bufferDataPointers.ContainsKey(command.ModelName))
            {
                LoadModelData(command.ModelName);
            }
        }

        public void Render()
        {
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (var data in _renderCommands)
            {
                if (_bufferDataPointers.TryGetValue(data.ModelName, out var pointers))
                {
                    foreach (var pointer in pointers)
                    {

                        var model = data.Entity.Transform.ModelMatrix;
                        var view = Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);
                        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75.0f),
                            (float)_windowManager.GetActiveWindow().Width / (float)_windowManager.GetActiveWindow().Height, 0.1f, 10000.0f);

                        testShader.SetMat4("model", model);
                        testShader.SetMat4("mvp", model * view * projection);

                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, pointer.count, DrawElementsType.UnsignedInt, (IntPtr)0, pointer.start);
                    }
                }
            }

            _renderCommands.Clear();
            
        }
    }
}