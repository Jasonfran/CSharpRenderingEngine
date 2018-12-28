using System;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Engine.Core
{
    public class UniformBuffer
    {
        private int _bufferId;
        private BufferUsageHint _usageHint;
        public int Size { get; }

        public UniformBuffer(int bufferId, int size, IntPtr data, BufferUsageHint usageHint)
        {
            _bufferId = bufferId;
            _usageHint = usageHint;
            Size = size;
            
            Bind();
            GL.BufferData(BufferTarget.UniformBuffer, Size, data, _usageHint);
            Unbind();
        }

        public UniformBuffer(int bufferId, int size, BufferUsageHint usageHint) 
            : this(bufferId, size, IntPtr.Zero, usageHint)
        {

        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, _bufferId);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void SetBufferIndex(int index)
        {
            Bind();
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, index, _bufferId);
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
                    var pointer = GL.MapBufferRange(BufferTarget.UniformBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                    Buffer.MemoryCopy(objPtr, pointer.ToPointer(), size, size); // Copy only data
                    GL.UnmapBuffer(BufferTarget.UniformBuffer);
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
                var pointer = GL.MapBufferRange(BufferTarget.UniformBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                Buffer.MemoryCopy(data.ToPointer(), pointer.ToPointer(), size, size); // Copy only data
                GL.UnmapBuffer(BufferTarget.UniformBuffer);
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
                    var pointer = GL.MapBufferRange(BufferTarget.UniformBuffer, offsetIntPtr, size, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit); // Map full buffer
                    Buffer.MemoryCopy(objPtr, pointer.ToPointer(), size, size); // Copy only data
                    GL.UnmapBuffer(BufferTarget.UniformBuffer);
                }
            }
            Unbind();
        }

        public void Resize(int size)
        {
            Bind();
            GL.BufferData(BufferTarget.UniformBuffer, size, IntPtr.Zero, _usageHint);
            Unbind();
        }
    }
}