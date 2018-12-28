namespace Engine.Renderer
{
    public interface IRenderCommand
    {
        RenderCommandType Type { get; }
    }
}