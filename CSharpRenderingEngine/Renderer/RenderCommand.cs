namespace Engine.Renderer
{
    public abstract class RenderCommand
    {
        public RenderCommandType Type { get; protected set; }
    }
}