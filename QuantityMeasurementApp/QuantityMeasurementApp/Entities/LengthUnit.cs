namespace QuantityMeasurementApp.Entities
{
    // Enum representing supported length units
    // Conversion factor is relative to FEET (base unit)

    public enum LengthUnit
    {
        FEET,
        INCH
    }

    // Helper class for conversion factors
    public static class LengthUnitExtensions
    {
        public static double ToFeetFactor(this LengthUnit unit)
        {
            return unit switch
            {
                LengthUnit.FEET => 1.0,
                LengthUnit.INCH => 1.0 / 12.0,
                _ => throw new ArgumentException("Unsupported unit")
            };
        }
    }
}