namespace api_integration.Domain.src.Entities
{
    public class BaseEntity
    {
        // Make the Id immutable
        public Guid Id { get; init; }

        protected BaseEntity(Guid? Id = null)
        {
            if (Id != null)
            {
                this.Id = (Guid)Id;
            }
            else
            {
                this.Id = Guid.NewGuid();
            }
        }
    }
}