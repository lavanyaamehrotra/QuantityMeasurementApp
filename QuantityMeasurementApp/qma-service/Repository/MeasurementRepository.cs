using Microsoft.EntityFrameworkCore;
using QmaService.Models;

namespace QmaService.Repository
{
    public class QmaDbContext : DbContext
    {
        public QmaDbContext(DbContextOptions<QmaDbContext> options) : base(options) { }

        public DbSet<MeasurementHistoryEntity> Measurements => Set<MeasurementHistoryEntity>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<MeasurementHistoryEntity>(e =>
            {
                e.ToTable("measurements");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.OperationType).HasMaxLength(50).IsRequired();
                e.Property(x => x.Op1Unit).HasMaxLength(50);
                e.Property(x => x.Op1Category).HasMaxLength(50);
                e.Property(x => x.Op2Unit).HasMaxLength(50);
                e.Property(x => x.Op2Category).HasMaxLength(50);
                e.Property(x => x.ResultUnit).HasMaxLength(50);
                e.Property(x => x.ResultCategory).HasMaxLength(50);
            });
        }
    }

    public interface IMeasurementRepository
    {
        Task SaveAsync(MeasurementHistoryEntity entity);
        Task<List<MeasurementHistoryEntity>> GetAllAsync();
        Task<List<MeasurementHistoryEntity>> GetByOperationAsync(string operation);
        Task<List<MeasurementHistoryEntity>> GetByCategoryAsync(string category);
        Task<List<MeasurementHistoryEntity>> GetByUserAsync(int userId);
        Task<int> GetCountAsync();
        Task ClearAsync();
    }

    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly QmaDbContext _db;
        public MeasurementRepository(QmaDbContext db) => _db = db;

        public async Task SaveAsync(MeasurementHistoryEntity entity)
        {
            _db.Measurements.Add(entity);
            await _db.SaveChangesAsync();
        }

        public Task<List<MeasurementHistoryEntity>> GetAllAsync()
            => _db.Measurements.OrderByDescending(m => m.Timestamp).ToListAsync();

        public Task<List<MeasurementHistoryEntity>> GetByOperationAsync(string operation)
            => _db.Measurements
                  .Where(m => m.OperationType == operation.ToUpperInvariant())
                  .OrderByDescending(m => m.Timestamp)
                  .ToListAsync();

        public Task<List<MeasurementHistoryEntity>> GetByCategoryAsync(string category)
            => _db.Measurements
                  .Where(m => m.Op1Category == category.ToUpperInvariant())
                  .OrderByDescending(m => m.Timestamp)
                  .ToListAsync();

        public Task<List<MeasurementHistoryEntity>> GetByUserAsync(int userId)
            => _db.Measurements
                  .Where(m => m.UserId == userId)
                  .OrderByDescending(m => m.Timestamp)
                  .ToListAsync();

        public Task<int> GetCountAsync() => _db.Measurements.CountAsync();

        public async Task ClearAsync()
        {
            _db.Measurements.RemoveRange(_db.Measurements);
            await _db.SaveChangesAsync();
        }
    }
}
