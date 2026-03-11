namespace api_integration.Domain.src.Entities.Fingrid
{
    public class CachedDataPoint : BaseEntity
    {
        public int DatasetId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Value { get; set; }
        public DateTime CachedAtUtc { get; set; } = DateTime.UtcNow;

        public CachedDataPoint() : base() { }

        public CachedDataPoint(int datasetId, DateTime startTime, DateTime endTime, double value) : base()
        {
            DatasetId = datasetId;
            StartTime = startTime;
            EndTime = endTime;
            Value = value;
        }
    }
}