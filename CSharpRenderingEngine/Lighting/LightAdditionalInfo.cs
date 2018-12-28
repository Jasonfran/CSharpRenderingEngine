using System.Runtime.InteropServices;

namespace Engine.Lighting
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct LightAdditionalInfo
    {
        
        [FieldOffset(0)] public int MaxNumberOfLights;
        [FieldOffset(4)] public int NumberOfLights;

        public LightAdditionalInfo(int maxNumberOfLights, int numberOfLights)
        {
            MaxNumberOfLights = maxNumberOfLights;
            NumberOfLights = numberOfLights;
        }
    }
}