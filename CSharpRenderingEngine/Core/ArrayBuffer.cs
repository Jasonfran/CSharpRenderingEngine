using System;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Core
{
    public class ArrayBuffer
    {
        private int _bufferId;
        private readonly int _size;        private readonly BufferUsageHint _usageHint;

        public ArrayBuffer(int bufferId, int size, IntPtr data, BufferUsageHint usageHint)
        {
            _bufferId = bufferId;
            _size = size;
            _usageHint = usageHint;

            Bind();
            SetData(size, data);
        }

        public ArrayBuffer(int bufferId, int size, BufferUsageHint usageHint)
        {
            _bufferId = bufferId;
            _size = size;
            _usageHint = usageHint;

            Bind();
            SetData(size, IntPtr.Zero);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetData(int size, IntPtr data)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, _usageHint);
        }

        public void SetData<T>(int size, T[] data) where T : struct
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, _usageHint);
        }

        public void EnableVertexAttrib(int index, int size, VertexAttribPointerType type, bool normalised, int stride,
            int offset)
        {
            Bind();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalised, stride, offset);
        }

        public void EnableVertexAttrib(int index, int size, VertexAttribPointerType type, bool normalised, int stride,
            IntPtr offset)
        {
            Bind();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalised, stride, offset);
        }
    }
}