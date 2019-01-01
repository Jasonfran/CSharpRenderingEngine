using System.Collections.Generic;
using Engine.Core;
using Engine.Renderer;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Engine.UI
{
    public class UIRendererCore
    {

        private List<UIShape> _renderableShapes;
        private Dictionary<UIShape, UIMeshDataPointer> _dataPointers;
        private int _initialBufferSize = 1024;
        private int VAO;
        private ArrayBuffer uiVertexBuffer;

        private Matrix4 projection;

        public UIRendererCore(int width, int height)
        {
            _renderableShapes = new List<UIShape>();
            _dataPointers = new Dictionary<UIShape, UIMeshDataPointer>();

            projection = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1000);

            LoadVertexBuffer();
        }

        private void LoadVertexBuffer()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            uiVertexBuffer = new ArrayBuffer(GL.GenBuffer(), _initialBufferSize, BufferUsageHint.DynamicDraw);
            uiVertexBuffer.EnableVertexAttrib(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            uiVertexBuffer.Unbind();
            GL.BindVertexArray(0);
        }

        public void LoadUIObjectMeshData()
        {
            GL.BindVertexArray(VAO);
            List<float> verts = new List<float>();
            foreach (var shape in _renderableShapes)
            {
                var rectVerts = shape.GetVerts();
                if (!_dataPointers.ContainsKey(shape))
                {
                    _dataPointers.Add(shape, new UIMeshDataPointer(verts.Count/4, rectVerts.Length / 4));
                }
                verts.AddRange(rectVerts);
            }

            if (sizeof(float) * verts.Count < uiVertexBuffer.GetSize())
            {
                uiVertexBuffer.Resize(sizeof(float) * verts.Count);
            }

            uiVertexBuffer.UpdateData(0, sizeof(float) * verts.Count, verts.ToArray());
            uiVertexBuffer.Unbind();
            GL.BindVertexArray(0);
        }

        public void RenderShape(Shader shader, UIShape shape)
        {
            if (!_dataPointers.ContainsKey(shape))
            {
                _renderableShapes.Add(shape);
                LoadUIObjectMeshData();
            }

            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(VAO);
            shader.Use();
            shader.SetMat4("projection", projection);
            uiVertexBuffer.Bind();
            shader.SetVec3("color", new Vector3(shape.Color.R, shape.Color.G, shape.Color.B));
            if (_dataPointers.ContainsKey(shape))
            {
                var pointer = _dataPointers[shape];
                GL.DrawArrays(PrimitiveType.Triangles, pointer.Start, pointer.Count);
            }
            uiVertexBuffer.Unbind();
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}