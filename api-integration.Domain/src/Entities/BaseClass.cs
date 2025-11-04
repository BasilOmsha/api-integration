namespace api_integration.Domain.src.Entities
{
    public class BaseClass
    {
        // Make the Id immutable
        public Guid Id { get; init; }

        protected BaseClass(Guid? Id = null)
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