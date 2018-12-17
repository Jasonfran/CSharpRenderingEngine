using Engine.Renderer;

namespace Engine.Component
{
    public class RenderableComponent : BaseComponent
    {
        public Model Model { get; }


        public RenderableComponent(Model model)
        {
            Model = model;
        }
    }
}