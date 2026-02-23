using System;

namespace QuantityMeasurementApp.Entities
{
    // Supported length units (UC3 + UC4)
    public enum LengthUnit
    {
        FEET,
        INCH,
        YARD,
        CENTIMETER
    }

    // Extension methods for unit conversions
    public static class LengthUnitExtensions
    {
        // Convert unit to FEET (base unit)
        public static double ToFeetFactor(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return 1.0;

                case LengthUnit.INCH:
                    return 1.0 / 12.0; // 12 inch = 1 foot

                case LengthUnit.YARD:
                    return 3.0; // 1 yard = 3 feet

                case LengthUnit.CENTIMETER:
                    return 0.0328084; // 1 cm = 0.0328084 feet

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }

        // Convert unit to INCHES (needed by QuantityLength)
        public static double ToInchesFactor(this LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.FEET:
                    return 12.0;

                case LengthUnit.INCH:
                    return 1.0;

                case LengthUnit.YARD:
                    return 36.0; // 1 yard = 36 inches

                case LengthUnit.CENTIMETER:
                    return 0.393701; // 1 cm = 0.393701 inch

                default:
                    throw new ArgumentException("Unsupported unit");
            }
        }
    }
}