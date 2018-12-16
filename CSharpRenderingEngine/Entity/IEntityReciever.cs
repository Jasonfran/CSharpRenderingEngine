namespace Engine.Entity
{
    public interface IEntityReciever
    {
        void EntityAdded(EntityManager.Entity entity);
        void EntityRemoved(EntityManager.Entity entity);
    }
}