namespace Engine.Component
{
    public class ModelComponent : BaseComponent
    {
        public string Name { get; }

        public ModelComponent(string name)
        {
            Name = name;
        }
    }
}