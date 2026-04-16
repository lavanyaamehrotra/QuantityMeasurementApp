namespace QmaService.Models
{
    public class QuantityDto
    {
        public double Value { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class TwoOperandRequest
    {
        public QuantityDto Operand1 { get; set; } = new();
        public QuantityDto Operand2 { get; set; } = new();
    }

    public class ConvertRequest
    {
        public QuantityDto Operand1 { get; set; } = new();
        public QuantityDto Target { get; set; } = new();
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public QuantityDto? Result { get; set; }
        public string Operation { get; set; } = string.Empty;
        public QuantityDto? Operand1 { get; set; }
        public QuantityDto? Operand2 { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class MeasurementHistoryEntity
    {
        public int Id { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
        public double Op1Value { get; set; }
        public string Op1Unit { get; set; } = string.Empty;
        public string Op1Category { get; set; } = string.Empty;
        public double? Op2Value { get; set; }
        public string? Op2Unit { get; set; }
        public string? Op2Category { get; set; }
        public double? ResultValue { get; set; }
        public string? ResultUnit { get; set; }
        public string? ResultCategory { get; set; }
        public int? UserId { get; set; }
    }
}
