using System;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.DataAccessLayer;

namespace QuantityMeasurementApp.BusinessLogicLayer
{
    // <summary>
    // Handles business validation and measurement comparison.
    // All validation rules are centralized here.
    // </summary>
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        // Logger instance
        private readonly MeasurementLogger logger = new MeasurementLogger();

        // Maximum allowed measurement limit for length (in feet)
        // Prevents unrealistic or overflow-prone values
        private const double MAX_FEET_VALUE = 1_000_000;

        // Maximum allowed weight limit (in kilograms)
        private const double MAX_KG_VALUE = 1_000_000;

        /// <summary>
        /// Validates input and creates Feet object.
        /// Implements fail-fast validation.
        /// </summary>
        public Feet CreateFeet(double value)
        {
            // ================= INVALID NUMBER CHECK =================

            // NaN check
            if (double.IsNaN(value)){
                throw new ArgumentException("Invalid input: Value cannot be NaN.");
            }
            // Infinity check
            if (double.IsInfinity(value)){
                throw new ArgumentException("Invalid input: Infinite values are not allowed.");
            }
            // ================= NEGATIVE VALUE CHECK =================
            if (value < 0){
                throw new ArgumentException("Invalid input: Negative values are not allowed.");
            }
            // ================= LARGE VALUE CHECK =================
            if (value > MAX_FEET_VALUE){
                throw new ArgumentException($"Invalid input: Value exceeds maximum allowed limit ({MAX_FEET_VALUE} ft).");
            }
            // ================= ZERO WARNING =================
            if (value == 0)
            {
                logger.Log("Warning: Zero measurement entered.");
            }

            // Create Feet entity after validation
            return new Feet(value);
        }

        /// <summary>
        /// Compares two Feet measurements.
        /// </summary>
        public bool CompareFeet(Feet f1, Feet f2)
        {
            bool result = f1.Equals(f2);
            logger.Log($"Compared {f1} and {f2} -> Equality Result: {result}");
            return result;
        }
        // ================= UC2 METHODS =================

        /// <summary>
        /// Validates input and creates Inches object.
        /// </summary>
        public Inches CreateInches(double value)
        {
            // Same validation rules as Feet

            if (double.IsNaN(value)){
                throw new ArgumentException("Invalid input: Value cannot be NaN.");
            }

            if (double.IsInfinity(value)){
                throw new ArgumentException("Invalid input: Infinite values are not allowed.");
            }

            if (value < 0){
                throw new ArgumentException("Invalid input: Negative values are not allowed.");
            }

            if (value > MAX_FEET_VALUE){
                throw new ArgumentException($"Invalid input: Value exceeds maximum allowed limit ({MAX_FEET_VALUE} in).");
            }

            if (value == 0){
                logger.Log("Warning: Zero measurement entered.");
            }

            return new Inches(value);
        }

        /// <summary>
        /// Compares two Inches measurements.
        /// </summary>
        public bool CompareInches(Inches i1, Inches i2)
        {
            bool result = i1.Equals(i2);
            logger.Log($"Compared {i1} and {i2} -> Equality Result: {result}");
            return result;
        }
        // ================= UC3 =================

        // Creates generic quantity
        public QuantityLength CreateQuantity(double value, LengthUnit unit)
        {
            if (value < 0)
                throw new ArgumentException("Negative values not allowed.");

            return new QuantityLength(value, unit);
        }

        // Compare generic quantities
        public bool CompareQuantity(QuantityLength q1, QuantityLength q2)
        {
            bool result = q1.Equals(q2);
            logger.Log($"Compared {q1} and {q2} -> Equality Result: {result}");
            return result;
        }

        // ================= UC9 - Weight Measurement (Kilogram, Gram, Pound) =================

        public QuantityWeight CreateWeight(double value, WeightUnit unit)
        {
            if (double.IsNaN(value))
                throw new ArgumentException("Invalid input: Value cannot be NaN.");
            if (double.IsInfinity(value))
                throw new ArgumentException("Invalid input: Infinite values are not allowed.");

            // allow negative weights (e.g., deltas) for UC9 tests
            double absKg = Math.Abs(unit.ConvertToBaseUnit(value));
            if (absKg > MAX_KG_VALUE)
                throw new ArgumentException($"Invalid input: Value exceeds maximum allowed weight ({MAX_KG_VALUE} kg).");

            return new QuantityWeight(value, unit);
        }

        public bool CompareWeight(QuantityWeight w1, QuantityWeight w2)
        {
            bool result = w1.Equals(w2);
            logger.Log($"Compared {w1} and {w2} -> Equality Result: {result}");
            return result;
        }

        public double ConvertWeight(double value, WeightUnit fromUnit, WeightUnit toUnit)
        {
            double absKg = Math.Abs(fromUnit.ConvertToBaseUnit(value));
            if (absKg > MAX_KG_VALUE)
                throw new ArgumentException($"Invalid input: Larger values not allowed. Maximum is {MAX_KG_VALUE:N0} kg equivalent.");

            double result = QuantityWeight.Convert(value, fromUnit, toUnit);
            logger.Log($"Converted {value} {fromUnit} -> {result} {toUnit}");
            return result;
        }

        public QuantityWeight ConvertWeight(QuantityWeight weight, WeightUnit toUnit)
        {
            QuantityWeight converted = weight.ConvertTo(toUnit);
            logger.Log($"Converted {weight} -> {converted}");
            return converted;
        }

        public QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2)
        {
            QuantityWeight result = w1.Add(w2);
            logger.Log($"Added {w1} + {w2} -> {result}");
            return result;
        }

        public QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2, WeightUnit targetUnit)
        {
            QuantityWeight result = w1.Add(w2, targetUnit);
            logger.Log($"Added {w1} + {w2} -> {result} (target: {targetUnit})");
            return result;
        }

        // ================= UC5 - Unit Conversion =================

        /// <summary>
        /// UC5: Converts length value from one unit to another.
        /// Delegates to QuantityLength.Convert with validation.
        /// </summary>
        public double ConvertLength(double value, LengthUnit fromUnit, LengthUnit toUnit)
        {
            // Large value check - convert to feet (base unit) and validate
            double valueInFeet = Math.Abs(value * fromUnit.ToFeetFactor());
            if (valueInFeet > MAX_FEET_VALUE)
                throw new ArgumentException($"Invalid input: Larger values not allowed. Maximum is {MAX_FEET_VALUE:N0} ft equivalent.");

            double result = QuantityLength.Convert(value, fromUnit, toUnit);
            logger.Log($"Converted {value} {fromUnit} -> {result} {toUnit}");
            return result;
        }

        /// <summary>
        /// UC5: Converts existing QuantityLength to target unit. Returns new instance (immutable).
        /// </summary>
        public QuantityLength ConvertLength(QuantityLength length, LengthUnit toUnit)
        {
            QuantityLength converted = length.ConvertTo(toUnit);
            logger.Log($"Converted {length} -> {converted}");
            return converted;
        }

        // ================= UC6 - Addition =================

        /// <summary>
        /// UC6: Adds two QuantityLength instances. Result is in unit of first operand.
        /// </summary>
        public QuantityLength AddLength(QuantityLength length1, QuantityLength length2)
        {
            QuantityLength result = length1.Add(length2);
            logger.Log($"Added {length1} + {length2} -> {result}");
            return result;
        }

        /// <summary>
        /// UC7: Adds two QuantityLength instances. Result is in specified target unit.
        /// </summary>
        public QuantityLength AddLength(QuantityLength length1, QuantityLength length2, LengthUnit targetUnit)
        {
            QuantityLength result = length1.Add(length2, targetUnit);
            logger.Log($"Added {length1} + {length2} -> {result} (target: {targetUnit})");
            return result;
        }

        // ================= EXTRA FEATURE =================
        // Calculates difference and returns user-friendly message
        public string GetMeasurementAnalysis(double value1, double value2, string unit)
        {
            double difference = Math.Abs(value1 - value2);

            if (difference == 0){
                return $"Measurement Difference: {difference} {unit}\nBoth measurements are exactly identical.";
            }
            if (difference < 1){
                return $"Measurement Difference: {difference} {unit}\nMeasurements are very close.";
            }
            return $"Measurement Difference: {difference} {unit}\nMeasurements differ significantly.";
        }
    }
}