using System;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// Generic Quantity class (UC3, UC4, UC5)
    /// Handles all length units using DRY principle.
    /// UC5: Provides unit-to-unit conversion via ConvertTo (instance) and Convert (static).
    /// Base unit: FEET. All conversions normalize through feet, rounded to 2 decimal places.
    /// </summary>
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
                LengthUnit.INCH => value / 12.0,
                LengthUnit.YARD => value * 3.0,
                LengthUnit.CENTIMETER => (value * 0.393701) / 12.0,
                _ => throw new ArgumentException("Unsupported unit.")
            };
        }

        /// <summary>
        /// Private utility: Converts this length to base unit (feet).
        /// Used internally for conversions. Full precision for accuracy.
        /// </summary>
        private double ConvertToBaseUnit()
        {
            return ConvertToFeet();
        }

        /// <summary>
        /// UC5: Converts this length to the specified target unit.
        /// Returns a new QuantityLength (immutable). Never modifies the receiver.
        /// Final value rounded to 2 decimal places for consistency.
        /// </summary>
        /// <param name="targetUnit">The unit to convert to. Must be a valid LengthUnit.</param>
        /// <returns>New QuantityLength representing the same physical length in targetUnit.</returns>
        public QuantityLength ConvertTo(LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            double valueInFeet = ConvertToBaseUnit();
            double factor = targetUnit.ToFeetFactor();
            double convertedValue = valueInFeet / factor;
            double rounded = Math.Round(convertedValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
        }

        /// <summary>
        /// UC5: Static conversion from source unit to target unit.
        /// Validates value (finite) and units. Returns numeric result rounded to 2 decimals.
        /// </summary>
        /// <param name="value">Numeric value to convert. Must be finite (not NaN, not Infinity).</param>
        /// <param name="sourceUnit">Source unit. Must be valid LengthUnit.</param>
        /// <param name="targetUnit">Target unit. Must be valid LengthUnit.</param>
        /// <returns>Converted numeric value in target unit.</returns>
        public static double Convert(double value, LengthUnit sourceUnit, LengthUnit targetUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid value: must be finite (not NaN or Infinity).");

            if (!Enum.IsDefined(typeof(LengthUnit), sourceUnit))
                throw new ArgumentException("Invalid or unsupported source unit.");

            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            double valueInFeet = value * sourceUnit.ToFeetFactor();
            double result = valueInFeet / targetUnit.ToFeetFactor();
            return Math.Round(result, 2, MidpointRounding.AwayFromZero);
        }

        // ================= UC6/UC7: ADDITION =================

        /// <summary>
        /// UC6: Adds another QuantityLength to this one. Result in unit of first operand.
        /// </summary>
        public QuantityLength Add(QuantityLength thatLength)
        {
            return AddAndConvert(thatLength, unit);
        }

        /// <summary>
        /// UC7: Adds another QuantityLength to this one with explicit target unit.
        /// Result is returned in the specified target unit.
        /// </summary>
        /// <param name="thatLength">The QuantityLength to add. Must not be null.</param>
        /// <param name="targetUnit">The unit for the result. Must not be null and must be valid.</param>
        public QuantityLength Add(QuantityLength thatLength, LengthUnit targetUnit)
        {
            if (!Enum.IsDefined(typeof(LengthUnit), targetUnit))
                throw new ArgumentException("Invalid or unsupported target unit.");

            return AddAndConvert(thatLength, targetUnit);
        }

        /// <summary>
        /// Private utility: Converts both to base unit, sums, converts to target unit.
        /// Used by Add(thatLength) and Add(thatLength, targetUnit) to avoid duplication.
        /// </summary>
        private QuantityLength AddAndConvert(QuantityLength thatLength, LengthUnit targetUnit)
        {
            if (thatLength == null)
                throw new ArgumentNullException(nameof(thatLength), "Cannot add null QuantityLength.");

            if (!double.IsFinite(value) || !double.IsFinite(thatLength.value))
                throw new ArgumentException("Both values must be finite (not NaN or Infinity).");

            double thisInFeet = ConvertToBaseUnit();
            double thatInFeet = thatLength.ConvertToFeet();
            double sumInFeet = thisInFeet + thatInFeet;

            double factor = targetUnit.ToFeetFactor();
            double resultValue = sumInFeet / factor;
            double rounded = Math.Round(resultValue, 2, MidpointRounding.AwayFromZero);

            return new QuantityLength(rounded, targetUnit);
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