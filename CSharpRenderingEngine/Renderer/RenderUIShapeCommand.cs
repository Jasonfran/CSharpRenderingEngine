using Engine.UI;

namespace Engine.Renderer
{
    public class RenderUIShapeCommand : RenderCommand
    {
        public UIShape Shape { get; private set; }

        public RenderUIShapeCommand()
        {
            Type = RenderCommandType.RenderUIShape;
        }

        public void Package(UIShape shape)
        {
            Shape = shape;
        }
    }
}