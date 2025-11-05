using api_integration.Application.src.DTOs.Fingrid;
using api_integration.Application.src.FactoryInterfaces.Fingrid;
using api_integration.Domain.src.Entities;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.Application.src.Factories.Fingrid
{
    public class MetaDataFactory : 
        IDtoFactory<MetaData, MetaDataReadDto, MetaDataCreateDto, MetaDataUpdateDto>,
        IEntityFactory<MetaData, MetaDataCreateDto, MetaDataUpdateDto, MetaDataReadDto>,
        IApiMapperFactory<MetaDataApiResponseDto, MetaDataReadDto>
    {
        /* (Entity → DTO)*/
        public MetaDataReadDto CreateReadDto(MetaData entity)
        {
            return new MetaDataReadDto(
                entity.Id,
                entity.DatasetId,
                entity.ModifiedAtUtc,
                entity.Type,
                entity.Status,
                entity.Organization,
                entity.NameEn,
                entity.DescriptionEn,
                entity.DataPeriodEn,
                entity.UpdateCadenceEn,
                entity.UnitEn,
                entity.ContactPersons,
                entity.License.Name,
                entity.License.TermsLink,
                entity.KeyWordsEn,
                entity.ContentGroupsEn,
                entity.AvailableFormats,
                entity.DataAvailableFromUtc
            );
        }

        public MetaDataCreateDto CreateCreateDto(MetaData entity)
        {
            return new MetaDataCreateDto(
                entity.DatasetId,
                entity.ModifiedAtUtc,
                entity.Type,
                entity.Status,
                entity.Organization,
                entity.NameEn,
                entity.DescriptionEn,
                entity.DataPeriodEn,
                entity.UpdateCadenceEn,
                entity.UnitEn,
                entity.ContactPersons,
                entity.License.Name,
                entity.License.TermsLink,
                entity.KeyWordsEn,
                entity.ContentGroupsEn,
                entity.AvailableFormats,
                entity.DataAvailableFromUtc
            );
        }

        public MetaDataUpdateDto CreateUpdateDto(MetaData entity)
        {
            return new MetaDataUpdateDto(
                entity.DatasetId,
                entity.ModifiedAtUtc,
                entity.Type,
                entity.Status,
                entity.Organization,
                entity.NameEn,
                entity.DescriptionEn,
                entity.DataPeriodEn,
                entity.UpdateCadenceEn,
                entity.UnitEn,
                entity.ContactPersons,
                entity.License.Name,
                entity.License.TermsLink,
                entity.KeyWordsEn,
                entity.ContentGroupsEn,
                entity.AvailableFormats,
                entity.DataAvailableFromUtc
            );
        }


        /* IEntityFactory Implementation (DTO → Entity) */
        public MetaDataReadDto MapFromApiResponse(MetaDataApiResponseDto apiResponse)
        {
            return new MetaDataReadDto(
                Id: Guid.Empty,
                DatasetId: apiResponse.Id,
                ModifiedAtUtc: apiResponse.ModifiedAtUtc,
                Type: apiResponse.Type,
                Status: apiResponse.Status,
                Organization: apiResponse.Organization,
                NameEn: apiResponse.NameEn,
                DescriptionEn: apiResponse.DescriptionEn,
                DataPeriodEn: apiResponse.DataPeriodEn,
                UpdateCadenceEn: apiResponse.UpdateCadenceEn,
                UnitEn: apiResponse.UnitEn,
                ContactPersons: apiResponse.ContactPersons,
                LicenseName: apiResponse.License.Name,
                LicenseTermsLink: apiResponse.License.TermsLink,
                KeyWordsEn: apiResponse.KeyWordsEn,
                ContentGroupsEn: apiResponse.ContentGroupsEn,
                AvailableFormats: apiResponse.AvailableFormats,
                DataAvailableFromUtc: apiResponse.DataAvailableFromUtc
            );
        }

        public MetaData CreateEntity(MetaDataCreateDto createDto)
        {
            return new MetaData(
                datasetId: createDto.DatasetId,
                modifiedAtUtc: createDto.ModifiedAtUtc,
                type: createDto.Type,
                status: createDto.Status,
                organization: createDto.Organization,
                nameEn: createDto.NameEn,
                descriptionEn: createDto.DescriptionEn,
                dataPeriodEn: createDto.DataPeriodEn,
                updateCadenceEn: createDto.UpdateCadenceEn,
                unitEn: createDto.UnitEn,
                contactPersons: createDto.ContactPersons,
                license: new License(createDto.LicenseName, createDto.LicenseTermsLink),
                keyWordsEn: createDto.KeyWordsEn,
                contentGroupsEn: createDto.ContentGroupsEn,
                availableFormats: createDto.AvailableFormats,
                dataAvailableFromUtc: createDto.DataAvailableFromUtc
            );
        }

        public MetaData UpdateEntity(MetaDataUpdateDto updateDto, MetaData existingEntity)
        {
            return new MetaData(
                Id: existingEntity.Id,
                datasetId: updateDto.DatasetId,
                modifiedAtUtc: updateDto.ModifiedAtUtc,
                type: updateDto.Type,
                status: updateDto.Status,
                organization: updateDto.Organization,
                nameEn: updateDto.NameEn,
                descriptionEn: updateDto.DescriptionEn,
                dataPeriodEn: updateDto.DataPeriodEn,
                updateCadenceEn: updateDto.UpdateCadenceEn,
                unitEn: updateDto.UnitEn,
                contactPersons: updateDto.ContactPersons,
                license: new License(updateDto.LicenseName, updateDto.LicenseTermsLink),
                keyWordsEn: updateDto.KeyWordsEn,
                contentGroupsEn: updateDto.ContentGroupsEn,
                availableFormats: updateDto.AvailableFormats,
                dataAvailableFromUtc: updateDto.DataAvailableFromUtc
            );
        }
    }
}