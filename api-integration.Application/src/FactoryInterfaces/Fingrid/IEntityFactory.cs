using api_integration.Domain.src.Entities;

namespace api_integration.Application.src.FactoryInterfaces.Fingrid
{
    public interface IEntityFactory<T, in TCreateDto, in TUpdateDto, in TReadDto>
        where T : BaseEntity
    {
        T CreateEntity(TCreateDto createDto);
        T UpdateEntity(TUpdateDto updateDto, T existingEntity);
    }
}