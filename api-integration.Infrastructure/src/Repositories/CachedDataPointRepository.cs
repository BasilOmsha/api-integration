using System.Text;
using api_integration.Domain.src.Entities.Fingrid;
using api_integration.Domain.src.Interfaces.Repositories;
using api_integration.Infrastructure.src.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api_integration.Infrastructure.src.Repositories
{
    public class CachedDataPointRepository : ICachedDataPointRepository
    {
        private readonly AppDbContext _context;

        // PostgreSQL max params = 65535; 6 params per row → safe chunk size
        private const int BatchSize = 5000;

        public CachedDataPointRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CachedDataPoint>> GetByDatasetIdAndRangeAsync(int datasetId, DateTime startTime, DateTime endTime) =>
            await _context.DataPointCache
                .Where(d => d.DatasetId == datasetId && d.StartTime >= startTime && d.EndTime <= endTime)
                .OrderBy(d => d.StartTime)
                .ToListAsync();

        public async Task UpsertRangeAsync(int datasetId, List<CachedDataPoint> dataPoints)
        {
            if (dataPoints.Count == 0) return;

            // Wrap all chunks in a single transaction for all-or-nothing reliability
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                for (int chunk = 0; chunk < dataPoints.Count; chunk += BatchSize)
                {
                    var batch = dataPoints.Skip(chunk).Take(BatchSize).ToList();
                    await ExecuteBatchUpsertAsync(batch);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteOlderThanAsync(DateTime cutoff)
        {
            return await _context.DataPointCache
                .Where(d => d.CachedAtUtc < cutoff)
                .ExecuteDeleteAsync();
        }

        private async Task ExecuteBatchUpsertAsync(List<CachedDataPoint> batch)
        {
            var sql = new StringBuilder();
            var parameters = new List<NpgsqlParameter>();

            sql.Append(@"INSERT INTO ""DataPointCache"" (""Id"", ""CachedAtUtc"", ""DatasetId"", ""EndTime"", ""StartTime"", ""Value"") VALUES ");

            for (int i = 0; i < batch.Count; i++)
            {
                if (i > 0) sql.Append(", ");

                int p = i * 6;
                sql.Append($"(@p{p}, @p{p + 1}, @p{p + 2}, @p{p + 3}, @p{p + 4}, @p{p + 5})");

                parameters.Add(new NpgsqlParameter($"@p{p}", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = batch[i].Id });
                parameters.Add(new NpgsqlParameter($"@p{p + 1}", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = batch[i].CachedAtUtc });
                parameters.Add(new NpgsqlParameter($"@p{p + 2}", NpgsqlTypes.NpgsqlDbType.Integer) { Value = batch[i].DatasetId });
                parameters.Add(new NpgsqlParameter($"@p{p + 3}", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = batch[i].EndTime });
                parameters.Add(new NpgsqlParameter($"@p{p + 4}", NpgsqlTypes.NpgsqlDbType.TimestampTz) { Value = batch[i].StartTime });
                parameters.Add(new NpgsqlParameter($"@p{p + 5}", NpgsqlTypes.NpgsqlDbType.Double) { Value = batch[i].Value });
            }

            sql.Append(@" ON CONFLICT (""DatasetId"", ""StartTime"") DO UPDATE SET ");
            sql.Append(@"""EndTime"" = EXCLUDED.""EndTime"", ");
            sql.Append(@"""Value"" = EXCLUDED.""Value"", ");
            sql.Append(@"""CachedAtUtc"" = EXCLUDED.""CachedAtUtc""");

            await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());
        }
    }
}