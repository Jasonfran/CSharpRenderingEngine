using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine.Lighting
{
    public struct CompletePointLightData
    {
        public CompletePointLightData(int totalSizeInBytes, LightAdditionalInfo additionalInfo, PointLightData[] lightData)
        {
            TotalSizeInBytes = totalSizeInBytes;
            LightData = lightData;
            AdditionalInfo = additionalInfo;
        }

        public LightAdditionalInfo AdditionalInfo;
        public PointLightData[] LightData;
        public int TotalSizeInBytes;
    }
}