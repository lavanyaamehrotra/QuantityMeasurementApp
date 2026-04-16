using QmaService.Exceptions;
using QmaService.Models;
using QmaService.Repository;
using QmaService.Units;

namespace QmaService.Services
{
    public interface IMeasurementService
    {
        Task<OperationResult> CompareAsync(QuantityDto q1, QuantityDto q2, int? userId = null);
        Task<OperationResult> ConvertAsync(QuantityDto q1, QuantityDto target, int? userId = null);
        Task<OperationResult> AddAsync(QuantityDto q1, QuantityDto q2, int? userId = null);
        Task<OperationResult> SubtractAsync(QuantityDto q1, QuantityDto q2, int? userId = null);
        Task<OperationResult> DivideAsync(QuantityDto q1, QuantityDto q2, int? userId = null);
        Task<List<MeasurementHistoryEntity>> GetHistoryAsync();
        Task<List<MeasurementHistoryEntity>> GetHistoryByOperationAsync(string operation);
        Task<List<MeasurementHistoryEntity>> GetHistoryByCategoryAsync(string category);
        Task<List<MeasurementHistoryEntity>> GetHistoryByUserAsync(int userId);
        Task<int> GetCountAsync();
        Task ClearHistoryAsync();
    }

    public class MeasurementService : IMeasurementService
    {
        private readonly IMeasurementRepository _repo;
        private readonly ILogger<MeasurementService> _logger;

        public MeasurementService(IMeasurementRepository repo, ILogger<MeasurementService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // ── Compare ──────────────────────────────────────────────────────────

        public async Task<OperationResult> CompareAsync(QuantityDto q1, QuantityDto q2, int? userId = null)
        {
            try
            {
                double b1 = ToBase(q1);
                double b2 = ToBase(q2);
                bool equal = Math.Abs(b1 - b2) < 1e-9;

                var result = new QuantityDto
                {
                    Value = equal ? 1 : 0,
                    UnitName = equal ? "EQUAL" : "NOT_EQUAL",
                    Category = "RESULT"
                };

                await SaveAsync("COMPARE", q1, q2, result, false, null, userId);
                return new OperationResult { Success = true, Operation = "COMPARE", Result = result, Operand1 = q1, Operand2 = q2 };
            }
            catch (QmaException ex)
            {
                await SaveAsync("COMPARE", q1, q2, null, true, ex.Message, userId);
                return new OperationResult { Success = false, Error = ex.Message, Operation = "COMPARE", Operand1 = q1, Operand2 = q2 };
            }
        }

        // ── Convert ──────────────────────────────────────────────────────────

        public async Task<OperationResult> ConvertAsync(QuantityDto q1, QuantityDto target, int? userId = null)
        {
            try
            {
                double baseVal = ToBase(q1);
                double converted = FromBase(baseVal, target);
                double rounded = Math.Round(converted, 2);

                var result = new QuantityDto
                {
                    Value = rounded,
                    UnitName = target.UnitName.ToUpperInvariant(),
                    Category = q1.Category.ToUpperInvariant()
                };

                await SaveAsync("CONVERT", q1, null, result, false, null, userId);
                return new OperationResult { Success = true, Operation = "CONVERT", Result = result, Operand1 = q1, Operand2 = target };
            }
            catch (QmaException ex)
            {
                await SaveAsync("CONVERT", q1, null, null, true, ex.Message, userId);
                return new OperationResult { Success = false, Error = ex.Message, Operation = "CONVERT", Operand1 = q1 };
            }
        }

        // ── Add ──────────────────────────────────────────────────────────────

        public async Task<OperationResult> AddAsync(QuantityDto q1, QuantityDto q2, int? userId = null)
        {
            try
            {
                ValidateArithmetic(q1, "Add");
                double sum = ToBase(q1) + ToBase(q2);
                var result = BuildResult(sum, q1);

                await SaveAsync("ADD", q1, q2, result, false, null, userId);
                return new OperationResult { Success = true, Operation = "ADD", Result = result, Operand1 = q1, Operand2 = q2 };
            }
            catch (QmaException ex)
            {
                await SaveAsync("ADD", q1, q2, null, true, ex.Message, userId);
                return new OperationResult { Success = false, Error = ex.Message, Operation = "ADD", Operand1 = q1, Operand2 = q2 };
            }
        }

        // ── Subtract ─────────────────────────────────────────────────────────

        public async Task<OperationResult> SubtractAsync(QuantityDto q1, QuantityDto q2, int? userId = null)
        {
            try
            {
                ValidateArithmetic(q1, "Subtract");
                double diff = ToBase(q1) - ToBase(q2);
                var result = BuildResult(diff, q1);

                await SaveAsync("SUBTRACT", q1, q2, result, false, null, userId);
                return new OperationResult { Success = true, Operation = "SUBTRACT", Result = result, Operand1 = q1, Operand2 = q2 };
            }
            catch (QmaException ex)
            {
                await SaveAsync("SUBTRACT", q1, q2, null, true, ex.Message, userId);
                return new OperationResult { Success = false, Error = ex.Message, Operation = "SUBTRACT", Operand1 = q1, Operand2 = q2 };
            }
        }

        // ── Divide ───────────────────────────────────────────────────────────

        public async Task<OperationResult> DivideAsync(QuantityDto q1, QuantityDto q2, int? userId = null)
        {
            try
            {
                ValidateArithmetic(q1, "Divide");
                double b2 = ToBase(q2);
                if (Math.Abs(b2) < 1e-15)
                    throw new QmaException("Division by zero.");
                double ratio = Math.Round(ToBase(q1) / b2, 6);

                var result = new QuantityDto { Value = ratio, UnitName = "RATIO", Category = "SCALAR" };

                await SaveAsync("DIVIDE", q1, q2, result, false, null, userId);
                return new OperationResult { Success = true, Operation = "DIVIDE", Result = result, Operand1 = q1, Operand2 = q2 };
            }
            catch (QmaException ex)
            {
                await SaveAsync("DIVIDE", q1, q2, null, true, ex.Message, userId);
                return new OperationResult { Success = false, Error = ex.Message, Operation = "DIVIDE", Operand1 = q1, Operand2 = q2 };
            }
        }

        // ── History ──────────────────────────────────────────────────────────

        public Task<List<MeasurementHistoryEntity>> GetHistoryAsync() => _repo.GetAllAsync();
        public Task<List<MeasurementHistoryEntity>> GetHistoryByOperationAsync(string op) => _repo.GetByOperationAsync(op);
        public Task<List<MeasurementHistoryEntity>> GetHistoryByCategoryAsync(string cat) => _repo.GetByCategoryAsync(cat);
        public Task<List<MeasurementHistoryEntity>> GetHistoryByUserAsync(int userId) => _repo.GetByUserAsync(userId);
        public Task<int> GetCountAsync() => _repo.GetCountAsync();
        public Task ClearHistoryAsync() => _repo.ClearAsync();

        // ── Private Helpers ──────────────────────────────────────────────────

        private static double ToBase(QuantityDto q)
        {
            string cat = q.Category.ToUpperInvariant();
            string unit = q.UnitName;
            return cat switch
            {
                "LENGTH"      => UnitParser.ParseLength(unit).ConvertToBaseUnit(q.Value),
                "WEIGHT"      => UnitParser.ParseWeight(unit).ConvertToBaseUnit(q.Value),
                "VOLUME"      => UnitParser.ParseVolume(unit).ConvertToBaseUnit(q.Value),
                "TEMPERATURE" => UnitParser.ParseTemperature(unit).ConvertToBaseUnit(q.Value),
                _ => throw new QmaException($"Unknown category: '{q.Category}'")
            };
        }

        private static double FromBase(double baseVal, QuantityDto target)
        {
            string cat = target.Category.ToUpperInvariant();
            string unit = target.UnitName;
            return cat switch
            {
                "LENGTH"      => UnitParser.ParseLength(unit).ConvertFromBaseUnit(baseVal),
                "WEIGHT"      => UnitParser.ParseWeight(unit).ConvertFromBaseUnit(baseVal),
                "VOLUME"      => UnitParser.ParseVolume(unit).ConvertFromBaseUnit(baseVal),
                "TEMPERATURE" => UnitParser.ParseTemperature(unit).ConvertFromBaseUnit(baseVal),
                _ => throw new QmaException($"Unknown category: '{target.Category}'")
            };
        }

        private static void ValidateArithmetic(QuantityDto q, string op)
        {
            if (q.Category.ToUpperInvariant() == "TEMPERATURE")
                throw new QmaException($"Temperature does not support {op}.");
        }

        private static QuantityDto BuildResult(double baseValue, QuantityDto q1)
        {
            double fromBase = FromBase(baseValue, q1);
            string unitName = q1.Category.ToUpperInvariant() switch
            {
                "LENGTH"      => UnitParser.ParseLength(q1.UnitName).GetUnitName(),
                "WEIGHT"      => UnitParser.ParseWeight(q1.UnitName).GetUnitName(),
                "VOLUME"      => UnitParser.ParseVolume(q1.UnitName).GetUnitName(),
                _ => q1.UnitName.ToUpperInvariant()
            };
            return new QuantityDto
            {
                Value = Math.Round(fromBase, 2),
                UnitName = unitName,
                Category = q1.Category.ToUpperInvariant()
            };
        }

        private async Task SaveAsync(string op, QuantityDto q1, QuantityDto? q2, QuantityDto? result,
            bool hasError, string? errorMsg, int? userId)
        {
            try
            {
                await _repo.SaveAsync(new MeasurementHistoryEntity
                {
                    OperationType = op,
                    HasError = hasError,
                    ErrorMessage = errorMsg,
                    Op1Value = q1.Value,
                    Op1Unit = q1.UnitName,
                    Op1Category = q1.Category.ToUpperInvariant(),
                    Op2Value = q2?.Value,
                    Op2Unit = q2?.UnitName,
                    Op2Category = q2?.Category.ToUpperInvariant(),
                    ResultValue = result?.Value,
                    ResultUnit = result?.UnitName,
                    ResultCategory = result?.Category,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to save measurement history: {Msg}", ex.Message);
            }
        }
    }
}
