using System;

namespace QuantityMeasurementApp.Entities
{
    // Generic Quantity class (UC3 & UC4)
    // Handles all length units using DRY principle

    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthUnit unit;

        // Constructor
        public QuantityLength(double value, LengthUnit unit){
        // numeric validation
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Invalid numeric value.");

        // UNIT VALIDATION (THIS FIXES YOUR TEST)
        if (!Enum.IsDefined(typeof(LengthUnit), unit))
            throw new ArgumentException("Invalid unit provided.");

        this.value = value;
        this.unit = unit;
    }

        // ================= CONVERSION =================
        // Converts ANY unit into FEET (base unit)

        public double ConvertToFeet()
        {
            return unit switch
            {
                LengthUnit.FEET => value,

                LengthUnit.INCH =>
                    value / 12.0,

                LengthUnit.YARD =>
                    value * 3.0,

                LengthUnit.CENTIMETER =>
                    (value * 0.393701) / 12.0,

                _ => throw new ArgumentException("Unsupported unit.")
            };
        }

        // ================= EQUALITY =================

        public override bool Equals(object? obj)
        {
            if (this == obj)
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            QuantityLength other = (QuantityLength)obj;

            double thisFeet = ConvertToFeet();
            double otherFeet = other.ConvertToFeet();

            return Math.Abs(thisFeet - otherFeet) < 0.0001;
        }

        public override int GetHashCode()
        {
            return ConvertToFeet().GetHashCode();
        }

        public override string ToString()
        {
            return $"{value} {unit}";
        }
    }
}