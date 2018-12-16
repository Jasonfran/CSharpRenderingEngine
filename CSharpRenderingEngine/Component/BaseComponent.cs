using System;

namespace Engine.Component
{
    public abstract class BaseComponent : IComponent
    {
        private bool idSet = false;
        private int _id;

        public int Id
        {
            get => _id;
            set => _id = idSet ? throw new InvalidOperationException("Component id can only be set once!") : value;
        }
    }
}