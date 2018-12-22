using Engine.Renderer;

namespace Engine.Resources
{
    public interface IResourceManager
    {
        Shader LoadShader(string vertex, string fragment);
        Model LoadModel(string filePath);
    }
}