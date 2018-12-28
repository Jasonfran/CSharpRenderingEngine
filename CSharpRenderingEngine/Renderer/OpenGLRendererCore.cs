using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp.Configs;
using Engine.Core;
using Engine.Entity;
using Engine.Lighting;
using Engine.Resources;
using Engine.World;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Engine.Renderer
{
    public class OpenGLRendererCore : EngineSystem
    {
        private WorldManager _worldManager;
        private WindowManager _windowManager;

        private readonly LightDataMapper _lightDataMapper;

        private readonly int maxNumOfPointLights = 16;

        private Dictionary<string, List<MeshDataPointer>> _bufferDataPointers; // Each pointer is a new mesh

        private Dictionary<string, List<MeshDataPointer>> _debugDataPointers;

        private List<Vertex> _vertices;
        private List<uint> _indices;
        private int _initialBufferSize = 1024 * 1024 * Vertex.Stride; // 32MB of initial buffer storage

        private int _vaoDebug;
        private int _vboDebug;

        private int _vao;
        private ArrayBuffer vertexBuffer;
        private int _eboIndices;
        private UniformBuffer lightsBuffer;

        private ResourceManager _resourceManager;

        private Shader testShader;
        private Shader debugShader;

        private Matrix4 view;
        private Matrix4 projection;

        public bool DebugMode = true;
        private Logger _logger;

        // Render command stuff

        private Dictionary<RenderCommandType, Stack<IRenderCommand>> _commandPool;
        private Stack<IRenderCommand> _renderCommands;

        private IntPtr _lightDataPtr;

        public OpenGLRendererCore(EngineSystemsCollection engineSystems) : base(engineSystems)
        {
            _lightDataMapper = new LightDataMapper();

            _bufferDataPointers = new Dictionary<string, List<MeshDataPointer>>();
            _debugDataPointers = new Dictionary<string, List<MeshDataPointer>>();
            _vertices = new List<Vertex>();
            _indices = new List<uint>();

            _commandPool = new Dictionary<RenderCommandType, Stack<IRenderCommand>>();
            _renderCommands = new Stack<IRenderCommand>();
        }

        public override void Init()
        {
            _logger = EngineSystems.GetSystem<Logger>();
            _windowManager = EngineSystems.GetSystem<WindowManager>();
            _windowManager.GetActiveWindow().MakeCurrent();

            _resourceManager = EngineSystems.GetSystem<ResourceManager>();

            GL.Viewport(0, 0, _windowManager.GetActiveWindow().Width, _windowManager.GetActiveWindow().Height);
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);

            CreateDebugVertexBuffer();

            CreateVertexDataBuffers();


            CreateLightUniformBuffer(0);

            testShader = _resourceManager.LoadShader("Shaders/Main.vert", "Shaders/Main.frag");
            testShader.Use();

            debugShader = _resourceManager.LoadShader("Shaders/debug.vert", "Shaders/debug.frag");

            view = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75.0f),
                (float)_windowManager.GetActiveWindow().Width / (float)_windowManager.GetActiveWindow().Height, 1.0f, 1000.0f);

            LoadModelData(_resourceManager.LoadModel("Models/2b.obj"));
        }

        private void CreateDebugVertexBuffer()
        {
            _vaoDebug = GL.GenVertexArray();
            _vboDebug = GL.GenBuffer();

            GL.BindVertexArray(_vaoDebug);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboDebug);

            GL.BufferData(BufferTarget.ArrayBuffer, _initialBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw);

            Line3D xAxis = new Line3D();
            xAxis.AddPoint(0.0f, 0.0f, 0.0f);
            xAxis.AddPoint(10.0f, 0.0f, 0.0f);
            xAxis.Color = new Vector3(1.0f, 0.0f, 0.0f);
            xAxis.Width = 1.0f;

            Line3D yAxis = new Line3D();
            yAxis.AddPoint(0.0f, 0.0f, 0.0f);
            yAxis.AddPoint(0.0f, 10.0f, 0.0f);
            yAxis.Color = new Vector3(0.0f, 1.0f, 0.0f);
            yAxis.Width = 1.0f;
            yAxis.NormalDirection = Vector3.UnitX;

            Line3D zAxis = new Line3D();
            zAxis.AddPoint(0.0f, 0.0f, 0.0f);
            zAxis.AddPoint(0.0f, 0.0f, 10.0f);
            zAxis.Color = new Vector3(0.0f, 0.0f, 1.0f);
            zAxis.Width = 1.0f;

            var xAxisVerts = xAxis.GetVertices().ToArray();
            var yAxisVerts = yAxis.GetVertices().ToArray();
            var zAxisVerts = zAxis.GetVertices().ToArray();

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, Vertex.Stride * xAxisVerts.Length, xAxisVerts);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(Vertex.Stride * xAxisVerts.Length) , Vertex.Stride * yAxisVerts.Length, yAxisVerts);
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(Vertex.Stride * (xAxisVerts.Length + yAxisVerts.Length)), Vertex.Stride * zAxisVerts.Length, zAxisVerts);

            _debugDataPointers.Add("axis", new List<MeshDataPointer>()
            {
                new MeshDataPointer(0, xAxisVerts.Length, null),
                new MeshDataPointer(xAxisVerts.Length, yAxisVerts.Length, null),
                new MeshDataPointer(xAxisVerts.Length + yAxisVerts.Length, zAxisVerts.Length, null)
            });

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride,
                Marshal.OffsetOf(typeof(Vertex), "Normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Stride,
                Marshal.OffsetOf(typeof(Vertex), "Color"));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Vertex.Stride,
                Marshal.OffsetOf(typeof(Vertex), "TexCoords"));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void CreateVertexDataBuffers()
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            vertexBuffer = new ArrayBuffer(GL.GenBuffer(), _initialBufferSize, BufferUsageHint.StaticDraw);
            vertexBuffer.EnableVertexAttrib(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, 0);
            vertexBuffer.EnableVertexAttrib(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, Marshal.OffsetOf(typeof(Vertex), "Normal"));
            vertexBuffer.EnableVertexAttrib(2, 3, VertexAttribPointerType.Float, false, Vertex.Stride, Marshal.OffsetOf(typeof(Vertex), "Color"));
            vertexBuffer.EnableVertexAttrib(3, 3, VertexAttribPointerType.Float, false, Vertex.Stride, Marshal.OffsetOf(typeof(Vertex), "TexCoords"));

            vertexBuffer.Unbind();

            _eboIndices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _initialBufferSize, IntPtr.Zero, BufferUsageHint.StaticDraw);
        }

        private void CreateLightUniformBuffer(int bufferIndex)
        {
            // two ints + lights. two ints padded to 16 bytes
            var size = 16 + 1024;
            lightsBuffer = new UniformBuffer(GL.GenBuffer(), size, BufferUsageHint.DynamicDraw);
            lightsBuffer.SetBufferIndex(bufferIndex);
        }

        public void UpdatePointLights(List<PointLight> lights)
        {
            var lightData = _lightDataMapper.GetPointLightData(lights, maxNumOfPointLights);

            if (lightsBuffer.Size < lightData.TotalSizeInBytes)
            {
                lightsBuffer.Resize(lightData.TotalSizeInBytes);
            }

            lightsBuffer.UpdateData(0, Marshal.SizeOf<LightAdditionalInfo>(), ref lightData.AdditionalInfo);
            lightsBuffer.UpdateData(Marshal.OffsetOf(typeof(CompletePointLightData), "LightData").ToInt32(), Marshal.SizeOf<PointLightData>() * lightData.LightData.Length, lightData.LightData);
        }

        public void SetView(Matrix4 viewMatrix)
        {
            view = viewMatrix;
        }

        private void LoadModelData(Model model)
        {
            var name = model.Path;
            var meshes = model.Meshes;

            _bufferDataPointers.Add(name, new List<MeshDataPointer>());

            foreach (var mesh in meshes)
            {
                var baseIndex = _vertices.Count;
                _vertices.AddRange(mesh.Vertices);
                _indices.AddRange(mesh.Indices);

                _bufferDataPointers[name].Add(new MeshDataPointer(baseIndex, mesh.Indices.Count, mesh.Material));
            }

            vertexBuffer.SetData(_vertices.Count * Vertex.Stride, _vertices.ToArray());

            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(uint), _indices.ToArray(),
                BufferUsageHint.StaticDraw);

            _logger.Info($"Loaded {model.Path} into VBO");
        }

        public void RenderModel(Model model, Transform transform)
        {
            if (!_bufferDataPointers.ContainsKey(model.Path))
            {
                LoadModelData(model);
            }

            if (_bufferDataPointers.TryGetValue(model.Path, out var pointers))
            {
                foreach (var pointer in pointers)
                {
                    testShader.SetMaterial("material", pointer.MeshMaterial);
                    testShader.SetMat4("model", transform.ModelMatrix);
                    testShader.SetMat4("view", view);
                    testShader.SetMat4("projection", projection);
                    testShader.SetMat4("mvp", transform.ModelMatrix * view * projection);

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, pointer.Count, DrawElementsType.UnsignedInt, (IntPtr)0, pointer.Start);

                    DrawElementsBaseVertexCommand command;
                    if (_commandPool.ContainsKey(RenderCommandType.RenderMesh))
                    {
                        if (_commandPool[RenderCommandType.RenderMesh].Count > 0)
                        {
                            command = (DrawElementsBaseVertexCommand)_commandPool[RenderCommandType.RenderMesh].Pop();
                            command.Repackage(pointer, transform, testShader);
                        }
                        else
                        {
                            command = new DrawElementsBaseVertexCommand(pointer, transform, testShader);
                            _logger.Warn("Created new render command");
                        }
                    }
                    else
                    {
                        command = new DrawElementsBaseVertexCommand(pointer, transform, testShader);
                        _logger.Warn("Created new render command");
                    }

                    _renderCommands.Push(command);

                }
            }

            if (DebugMode)
            {
                RenderDebugAxis(transform.ModelMatrix, 0.1f);
            }
        }

        public void ProcessRenderCommands()
        {
            while (_renderCommands.Count > 0)
            {
                var c = _renderCommands.Pop();
                if (c.Type == RenderCommandType.RenderMesh)
                {
                    var command = (DrawElementsBaseVertexCommand) c;
                    command.Shader.Use();
                    testShader.SetMaterial("material", command.DataPointer.MeshMaterial);
                    testShader.SetMat4("model", command.Transform.ModelMatrix);
                    testShader.SetMat4("view", view);
                    testShader.SetMat4("projection", projection);
                    testShader.SetMat4("mvp", command.Transform.ModelMatrix * view * projection);

                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, command.DataPointer.Count, DrawElementsType.UnsignedInt,
                        (IntPtr) 0, command.DataPointer.Start);

                    if (_commandPool.ContainsKey(command.Type))
                    {
                        _commandPool[command.Type].Push(command);
                    }
                    else
                    {
                        _commandPool.Add(command.Type, new Stack<IRenderCommand>());
                        _commandPool[command.Type].Push(command);
                    }
                }
            }
        }

        public void RenderDebugAxis(Matrix4 modelMatrix, float width)
        {
            GL.BindVertexArray(_vaoDebug);

            debugShader.Use();
            debugShader.SetMat4("model", modelMatrix);
            debugShader.SetMat4("view", view);
            debugShader.SetMat4("projection", projection);
            debugShader.SetMat4("mvp", modelMatrix * view * projection);
            debugShader.SetFloat("linewidth", width);

            foreach (var pointer in _debugDataPointers["axis"])
            {
                GL.DrawArrays(PrimitiveType.TriangleStrip, pointer.Start, pointer.Count);
            }

            GL.BindVertexArray(_vao);
        }

        public void FrameBegin()
        {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(_vao);
            testShader.Use();
        }

        public void FrameEnd()
        {

            _windowManager.GetActiveWindow().SwapBuffers();
        }
    }
}