using api_integration.Application.src.DTOs;
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
    }
}
