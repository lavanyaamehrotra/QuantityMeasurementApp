using QmaService.Exceptions;

namespace QmaService.Units
{
    public interface IMeasurable
    {
        double GetConversionFactor();
        double ConvertToBaseUnit(double value);
        double ConvertFromBaseUnit(double baseValue);
        string GetUnitName();
        bool SupportsArithmetic() => true;
        void ValidateOperationSupport(string operation) { }
    }

    // ── Length ────────────────────────────────────────────────────────────────
    public enum LengthUnitM
    {
        FEET, INCHES, YARDS, CENTIMETERS
    }

    public static class LengthUnitMExtensions
    {
        public static double GetConversionFactor(this LengthUnitM u) => u switch
        {
            LengthUnitM.FEET        => 1.0,
            LengthUnitM.INCHES      => 1.0 / 12.0,
            LengthUnitM.YARDS       => 3.0,
            LengthUnitM.CENTIMETERS => 0.0328084,
            _ => throw new QmaException($"Unknown LengthUnit: {u}")
        };

        public static double ConvertToBaseUnit(this LengthUnitM u, double value)
            => value * u.GetConversionFactor();

        public static double ConvertFromBaseUnit(this LengthUnitM u, double baseValue)
            => baseValue / u.GetConversionFactor();

        public static string GetUnitName(this LengthUnitM u) => u.ToString();
    }

    // ── Weight ────────────────────────────────────────────────────────────────
    public enum WeightUnitM
    {
        KILOGRAM, GRAM, POUND
    }

    public static class WeightUnitMExtensions
    {
        public static double GetConversionFactor(this WeightUnitM u) => u switch
        {
            WeightUnitM.KILOGRAM => 1.0,
            WeightUnitM.GRAM     => 0.001,
            WeightUnitM.POUND    => 0.453592,
            _ => throw new QmaException($"Unknown WeightUnit: {u}")
        };

        public static double ConvertToBaseUnit(this WeightUnitM u, double value)
            => value * u.GetConversionFactor();

        public static double ConvertFromBaseUnit(this WeightUnitM u, double baseValue)
            => baseValue / u.GetConversionFactor();

        public static string GetUnitName(this WeightUnitM u) => u.ToString();
    }

    // ── Volume ────────────────────────────────────────────────────────────────
    public enum VolumeUnitM
    {
        LITRE, MILLILITRE, GALLON
    }

    public static class VolumeUnitMExtensions
    {
        public static double GetConversionFactor(this VolumeUnitM u) => u switch
        {
            VolumeUnitM.LITRE      => 1.0,
            VolumeUnitM.MILLILITRE => 0.001,
            VolumeUnitM.GALLON     => 3.78541,
            _ => throw new QmaException($"Unknown VolumeUnit: {u}")
        };

        public static double ConvertToBaseUnit(this VolumeUnitM u, double value)
            => value * u.GetConversionFactor();

        public static double ConvertFromBaseUnit(this VolumeUnitM u, double baseValue)
            => baseValue / u.GetConversionFactor();

        public static string GetUnitName(this VolumeUnitM u) => u.ToString();
    }

    // ── Temperature ───────────────────────────────────────────────────────────
    public enum TemperatureUnitM
    {
        CELSIUS, FAHRENHEIT, KELVIN
    }

    public static class TemperatureUnitMExtensions
    {
        // Converts any temperature to Celsius (base unit)
        public static double ConvertToBaseUnit(this TemperatureUnitM u, double value) => u switch
        {
            TemperatureUnitM.CELSIUS    => value,
            TemperatureUnitM.FAHRENHEIT => (value - 32.0) * 5.0 / 9.0,
            TemperatureUnitM.KELVIN     => value - 273.15,
            _ => throw new QmaException($"Unknown TemperatureUnit: {u}")
        };

        public static double ConvertFromBaseUnit(this TemperatureUnitM u, double baseCelsius) => u switch
        {
            TemperatureUnitM.CELSIUS    => baseCelsius,
            TemperatureUnitM.FAHRENHEIT => baseCelsius * 9.0 / 5.0 + 32.0,
            TemperatureUnitM.KELVIN     => baseCelsius + 273.15,
            _ => throw new QmaException($"Unknown TemperatureUnit: {u}")
        };

        public static string GetUnitName(this TemperatureUnitM u) => u.ToString();

        public static void ValidateArithmetic(this TemperatureUnitM _, string operation)
            => throw new QmaException($"Temperature does not support {operation}.");
    }
}
