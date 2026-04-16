using QmaService.Exceptions;
using QmaService.Units;

namespace QmaService.Units
{
    public static class UnitParser
    {
        public static LengthUnitM ParseLength(string unit) =>
            unit.ToUpperInvariant() switch
            {
                "FEET" or "FT" or "FOOT"        => LengthUnitM.FEET,
                "INCH" or "IN" or "INCHES"      => LengthUnitM.INCHES,
                "YARD" or "YD" or "YARDS"       => LengthUnitM.YARDS,
                "CM" or "CENTIMETER" or "CENTIMETERS" => LengthUnitM.CENTIMETERS,
                _ => throw new QmaException($"Unknown length unit: '{unit}'")
            };

        public static WeightUnitM ParseWeight(string unit) =>
            unit.ToUpperInvariant() switch
            {
                "KG" or "KILOGRAM" or "KILOGRAMS" => WeightUnitM.KILOGRAM,
                "G"  or "GRAM"     or "GRAMS"     => WeightUnitM.GRAM,
                "LB" or "POUND"    or "POUNDS"    => WeightUnitM.POUND,
                _ => throw new QmaException($"Unknown weight unit: '{unit}'")
            };

        public static VolumeUnitM ParseVolume(string unit) =>
            unit.ToUpperInvariant() switch
            {
                "L"   or "LITRE"      or "LITER"       or "LITRES"      => VolumeUnitM.LITRE,
                "ML"  or "MILLILITRE" or "MILLILITER"  or "MILLILITRES" => VolumeUnitM.MILLILITRE,
                "GAL" or "GALLON"     or "GALLONS"                      => VolumeUnitM.GALLON,
                _ => throw new QmaException($"Unknown volume unit: '{unit}'")
            };

        public static TemperatureUnitM ParseTemperature(string unit) =>
            unit.ToUpperInvariant() switch
            {
                "C" or "CELSIUS"    => TemperatureUnitM.CELSIUS,
                "F" or "FAHRENHEIT" => TemperatureUnitM.FAHRENHEIT,
                "K" or "KELVIN"     => TemperatureUnitM.KELVIN,
                _ => throw new QmaException($"Unknown temperature unit: '{unit}'")
            };
    }
}
