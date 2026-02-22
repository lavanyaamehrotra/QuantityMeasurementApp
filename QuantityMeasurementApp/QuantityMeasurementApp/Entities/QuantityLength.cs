using System;

namespace QuantityMeasurementApp.Entities
{
    // UC3 - Generic Quantity Length Class
    // Implements DRY principle and cross-unit equality

    public class QuantityLength
    {
        private readonly double value;
        private readonly LengthUnit unit;

        // Constructor
        public QuantityLength(double value, LengthUnit unit)
        {
            // ================= VALUE VALIDATION =================

            if (double.IsNaN(value))
                throw new ArgumentException("Invalid input: NaN not allowed.");

            if (double.IsInfinity(value))
                throw new ArgumentException("Invalid input: Infinite value.");

            if (value < 0)
                throw new ArgumentException("Invalid input: Negative value.");

            // ================= UNIT VALIDATION =================
            // VERY IMPORTANT FIX
            // Enum accepts any int internally → must validate manually

            bool validUnit = Enum.IsDefined(typeof(LengthUnit), unit);

            if (!validUnit)
                throw new ArgumentException("Invalid unit provided.");

            this.value = value;
            this.unit = unit;
        }

        // Convert measurement into FEET (base unit)
        private double ConvertToFeet()
        {
            return value * unit.ToFeetFactor();
        }

        // Equality comparison
        public override bool Equals(object? obj)
        {
            if (this == obj)
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            QuantityLength other = (QuantityLength)obj;

            double difference =
                Math.Abs(this.ConvertToFeet() - other.ConvertToFeet());

            return difference < 0.0001;
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