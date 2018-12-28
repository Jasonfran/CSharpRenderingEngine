using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine.Lighting
{
    public class LightDataMapper
    {
        private LightDataContainer<PointLightData> GetData(PointLight pointLight)
        {
            return new LightDataContainer<PointLightData>(Marshal.SizeOf<PointLightData>(),
                new PointLightData(
                pointLight.Transform.Position,
                pointLight.AmbientColor,
                pointLight.DiffuseColor,
                pointLight.SpecularColor,
                pointLight.Constant,
                pointLight.Linear,
                pointLight.Quadratic
            ));
        }

        public CompletePointLightData GetPointLightData(List<PointLight> lights, int maxNumberOfLights)
        {
            var totalSize = 0;
            var lightData = new PointLightData[lights.Count];
            for (var index = 0; index < lights.Count; index++)
            {
                var light = lights[index];
                var mappedData = GetData(light);
                if (mappedData.Size > 0)
                {
                    totalSize += mappedData.Size;
                    lightData[index] = mappedData.Data;
                }
            }

            var additionalInfo = new LightAdditionalInfo(maxNumberOfLights, lights.Count);

            return new CompletePointLightData(totalSize, additionalInfo, lightData);
        }
    }
}