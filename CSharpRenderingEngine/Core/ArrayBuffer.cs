using System;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Engine.Core
{
    public class ArrayBuffer
    {
        private readonly int _bufferId;
        private int _size;
        private readonly BufferUsageHint _usageHint;

        public ArrayBuffer(int bufferId, int size, IntPtr data, BufferUsageHint usageHint)
        {
            _bufferId = bufferId;
            _size = size;
            _usageHint = usageHint;

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

        public int GetSize()
        {
            return _size;
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void SetData(int size, IntPtr data)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, data, _usageHint);
            Unbind();
        }

        public void UpdateData<T>(int offset, int size, ref T data) where T : unmanaged
        {
            var offsetIntPtr = (IntPtr)offset;
            Bind();
            unsafe
            {
                fixed (T* objPtr = &data)
                {
                    var pointer = GL.MapBufferRange(BufferTarget.ArrayBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                    Buffer.MemoryCopy(objPtr, pointer.ToPointer(), size, size); // Copy only data
                    GL.UnmapBuffer(BufferTarget.ArrayBuffer);
                }
            }
            Unbind();
        }

        public void UpdateData(int offset, int size, IntPtr data)
        {
            var offsetIntPtr = (IntPtr)offset;
            Bind();
            unsafe
            {
                var pointer = GL.MapBufferRange(BufferTarget.ArrayBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                Buffer.MemoryCopy(data.ToPointer(), pointer.ToPointer(), size, size); // Copy only data
                GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            }
            Unbind();
        }

        public void UpdateData<T>(int offset, int size, T[] data) where T : unmanaged
        {
            var offsetIntPtr = (IntPtr)offset;
            Bind();
            unsafe
            {
                fixed (T* objPtr = data)
                {
                    var pointer = GL.MapBufferRange(BufferTarget.ArrayBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                    Buffer.MemoryCopy(objPtr, pointer.ToPointer(), size, size); // Copy only data
                    GL.UnmapBuffer(BufferTarget.ArrayBuffer);
                }
            }
            Unbind();
        }

        public void EnableVertexAttrib(int index, int size, VertexAttribPointerType type, bool normalised, int stride,
            int offset)
        {
            Bind();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalised, stride, offset);
            Unbind();
        }

        public void EnableVertexAttrib(int index, int size, VertexAttribPointerType type, bool normalised, int stride,
            IntPtr offset)
        {
            Bind();
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, type, normalised, stride, offset);
            Unbind();
        }

        public void Resize(int size)
        {
            Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, _usageHint);
            _size = size;
            Unbind();
        }
    }
}