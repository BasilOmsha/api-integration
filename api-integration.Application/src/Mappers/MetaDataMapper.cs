using api_integration.Application.src.DTOs;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.Application.src.Mappers
{
    public static class MetaDataMapper
    {
        public static MetaDataCreateDto ToInternalDto(this MetaDataExternalApiResDto external)
        {
            return new MetaDataCreateDto
            {
                DatasetId = external.Id,
                ModifiedAtUtc = external.ModifiedAtUtc,
                Type = external.Type,
                Status = external.Status,
                Organization = external.Organization,
                NameEn = external.NameEn,
                DescriptionEn = external.DescriptionEn,
                DataPeriodEn = external.DataPeriodEn,
                UpdateCadenceEn = external.UpdateCadenceEn,
                UnitEn = external.UnitEn,
                ContactPersons = external.ContactPersons,
                License = new License(external.License.Name, external.License.TermsLink),
                KeyWordsEn = external.KeyWordsEn,
                ContentGroupsEn = external.ContentGroupsEn,
                AvailableFormats = external.AvailableFormats,
                DataAvailableFromUtc = external.DataAvailableFromUtc
            };
        }

        public static MetaDataReadDto ToReadDto(this MetaData entity)
        {
            return new MetaDataReadDto
            {
                Id = entity.Id,
                DatasetId = entity.DatasetId,
                ModifiedAtUtc = entity.ModifiedAtUtc,
                Type = entity.Type,
                Status = entity.Status,
                Organization = entity.Organization,
                NameEn = entity.NameEn,
                DescriptionEn = entity.DescriptionEn,
                DataPeriodEn = entity.DataPeriodEn,
                UpdateCadenceEn = entity.UpdateCadenceEn,
                UnitEn = entity.UnitEn,
                ContactPersons = entity.ContactPersons,
                License = entity.License,
                KeyWordsEn = entity.KeyWordsEn,
                ContentGroupsEn = entity.ContentGroupsEn,
                AvailableFormats = entity.AvailableFormats,
                DataAvailableFromUtc = entity.DataAvailableFromUtc
            };
        }
    }
}