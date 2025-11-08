using api_integration.Domain.src.Entities;

namespace api_integration.Application.src.Interfaces.IDtos
{
    public interface IBaseCreateDto<out T> where T : BaseEntity
    {
        public T ToEntity();
    }
}