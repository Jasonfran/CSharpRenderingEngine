using System.Collections.Generic;

namespace Engine.Lighting
{
    public struct LightDataContainer<T> where T : ILightData
    {
        public int Size;
        public T Data;

        public LightDataContainer(int size, T data)
        {
            Size = size;
            Data = data;
        }
    }
}