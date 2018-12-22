namespace Engine.Lighting
{
    public class LightDataMapper
    {
        public float[] GetData(Light light)
        {
            if (light is PointLight pointLight)
            {
                var position = pointLight.Transform.Position;
                var ambient = pointLight.AmbientColor;
                var diffuse = pointLight.DiffuseColor;
                var specular = pointLight.SpecularColor;

                // 16 BYTE ALIGNED
                var data = new float[]
                {
                    position.X, position.Y, position.Z, 0,
                    ambient.X, ambient.Y, ambient.Z, 0,
                    diffuse.X, diffuse.Y, diffuse.Z, 0,
                    specular.X, specular.Y, specular.Z, 0,
                    pointLight.Constant,
                    pointLight.Linear,
                    pointLight.Quadratic
                };

                return data;
            }

            return new float[0];
        }
    }
}