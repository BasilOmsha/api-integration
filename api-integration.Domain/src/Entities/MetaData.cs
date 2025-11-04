using System.Diagnostics.CodeAnalysis;
using api_integration.Domain.src.ValueObjects;

namespace api_integration.Domain.src.Entities
{
    public class MetaData : BaseClass
    {
        public int DatasetId { get; set; }
        public DateTime ModifiedAtUtc { get; set; }
        public required string Type { get; set; }
        public required string Status { get; set; }
        public required string Organization { get; set; }
        public required string NameEn { get; set; }
        public required string DescriptionEn { get; set; }
        public required string DataPeriodEn { get; set; }
        public string? UpdateCadenceEn { get; set; }
        public required string UnitEn { get; set; }
        public required string ContactPersons { get; set; }
        public required License License { get; set; }
        public required List<string> KeyWordsEn { get; set; }
        public required List<string> ContentGroupsEn { get; set; }
        public required List<string> AvailableFormats { get; set; }
        public DateTime? DataAvailableFromUtc { get; set; }

        [SetsRequiredMembers]
        public MetaData() : base()
        {
            Type = string.Empty;
            Status = string.Empty;
            Organization = string.Empty;
            NameEn = string.Empty;
            DescriptionEn = string.Empty;
            DataPeriodEn = string.Empty;
            UnitEn = string.Empty;
            ContactPersons = string.Empty;
            License = new License(string.Empty, string.Empty);
            KeyWordsEn = []; // [] the new way of "new List<string>()"
            ContentGroupsEn = [];
            AvailableFormats = [];
        }

        [SetsRequiredMembers]
        public MetaData(int datasetId, DateTime modifiedAtUtc, string type, string status, string organization, string nameEn, string descriptionEn, string dataPeriodEn, string updateCadenceEn, string unitEn, string contactPersons, License license, List<string> keyWordsEn, List<string> contentGroupsEn, List<string> availableFormats, DateTime? dataAvailableFromUtc)
        : this(
            null,
            datasetId,
            modifiedAtUtc,
            type,
            status,
            organization,
            nameEn,
            descriptionEn,
            dataPeriodEn,
            updateCadenceEn,
            unitEn,
            contactPersons,
            license,
            keyWordsEn,
            contentGroupsEn,
            availableFormats,
            dataAvailableFromUtc
        )
        {
        }
        
        [SetsRequiredMembers]
        public MetaData(
            Guid? Id,
            int datasetId,
            DateTime modifiedAtUtc,
            string type,
            string status,
            string organization,
            string nameEn,
            string descriptionEn,
            string dataPeriodEn,
            string updateCadenceEn,
            string unitEn,
            string contactPersons,
            License license,
            List<string> keyWordsEn,
            List<string> contentGroupsEn,
            List<string> availableFormats,
            DateTime? dataAvailableFromUtc
        ) : base(Id)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(status);
            ArgumentNullException.ThrowIfNull(license);
            ArgumentNullException.ThrowIfNull(keyWordsEn);
            ArgumentNullException.ThrowIfNull(availableFormats);

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type cannot be empty or whitespace", nameof(type));
            
            if (datasetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(datasetId), "Dataset ID must be positive");

            if (availableFormats.Count == 0)
                throw new ArgumentException("Must have at least one available format", nameof(availableFormats));
        
            DatasetId = datasetId;
            ModifiedAtUtc = modifiedAtUtc;
            Type = type;
            Status = status;
            Organization = organization;
            NameEn = nameEn;
            DescriptionEn = descriptionEn;
            DataPeriodEn = dataPeriodEn;
            UpdateCadenceEn = updateCadenceEn;
            UnitEn = unitEn;
            ContactPersons = contactPersons;
            License = license;
            KeyWordsEn = keyWordsEn;
            ContentGroupsEn = contentGroupsEn;
            AvailableFormats = availableFormats;
            DataAvailableFromUtc = dataAvailableFromUtc;
        }
    }
}