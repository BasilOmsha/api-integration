using api_integration.Domain.src.Entities;

namespace api_integration.Application.src.Interfaces.IDtos
{
    public interface IBaseReadDto<T> where T : BaseEntity
    {
        public Guid Id { get; } 
    }
}