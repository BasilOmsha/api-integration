using api_integration.Domain.src.Entities;

namespace api_integration.Application.src.FactoryInterfaces.Fingrid
{
    public interface IDtoFactory<in T, out TReadDto, out TCreateReadDto, out TUpdateDto>
        where T : BaseEntity
    {
        TReadDto CreateReadDto(T entity);
        TCreateReadDto CreateCreateDto(T entity);
        TUpdateDto CreateUpdateDto(T entity);
    }
}