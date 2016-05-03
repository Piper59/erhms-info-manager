namespace ERHMS.Presentation.Messages
{
    public class RefreshMessage<TEntity>
    {
        public TEntity Entity { get; private set; }

        public RefreshMessage(TEntity entity)
        {
            Entity = entity;
        }
    }
}
