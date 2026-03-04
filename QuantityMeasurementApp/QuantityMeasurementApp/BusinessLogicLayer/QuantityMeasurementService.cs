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
        // ================= UC10 - Generic Quantity<U> Methods =================

        /// <summary>
        /// UC10: Creates a validated generic Quantity for any IMeasurable unit type.
        /// </summary>
        public Quantity<U> CreateGenericQuantity<U>(double value, U unit) where U : class, IMeasurable
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null.");
            if (!double.IsFinite(value))
                throw new ArgumentException("Invalid input: Value must be finite (not NaN or Infinity).");

            Quantity<U> quantity = new Quantity<U>(value, unit);
            logger.Log($"Created Quantity({value}, {unit.GetUnitName()})");
            return quantity;
        }

        /// <summary>
        /// UC10: Compares two generic Quantity instances of the same unit type.
        /// </summary>
        public bool CompareGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable
        {
            if (q1 == null || q2 == null)
                throw new ArgumentNullException("Quantities cannot be null.");

            bool result = q1.Equals(q2);
            logger.Log($"Compared {q1} and {q2} -> Equality Result: {result}");
            return result;
        }

        /// <summary>
        /// UC10: Converts a generic Quantity to the specified target unit.
        /// </summary>
        public Quantity<U> ConvertGenericQuantity<U>(Quantity<U> quantity, U targetUnit) where U : class, IMeasurable
        {
            if (quantity == null)
                throw new ArgumentNullException(nameof(quantity), "Quantity cannot be null.");
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");

            Quantity<U> result = quantity.ConvertTo(targetUnit);
            logger.Log($"Converted {quantity} -> {result}");
            return result;
        }

        /// <summary>
        /// UC10: Adds two generic Quantity instances. Result is in unit of first operand.
        /// </summary>
        public Quantity<U> AddGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable
        {
            if (q1 == null || q2 == null)
                throw new ArgumentNullException("Quantities cannot be null.");

            Quantity<U> result = q1.Add(q2);
            logger.Log($"Added {q1} + {q2} -> {result}");
            return result;
        }

        /// <summary>
        /// UC10: Adds two generic Quantity instances. Result is in specified target unit.
        /// </summary>
        public Quantity<U> AddGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2, U targetUnit) where U : class, IMeasurable
        {
            if (q1 == null || q2 == null)
                throw new ArgumentNullException("Quantities cannot be null.");
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");

            Quantity<U> result = q1.Add(q2, targetUnit);
            logger.Log($"Added {q1} + {q2} -> {result} (target: {targetUnit.GetUnitName()})");
            return result;
        }

        /// <summary>
        /// UC10: Cross-category equality check — always false by design (type-safe prevention).
        /// Routed through BLL so Presentation Layer never calls .Equals() directly.
        /// </summary>
        public bool CheckCrossCategory<U1, U2>(Quantity<U1> q1, Quantity<U2> q2)
            where U1 : class, IMeasurable
            where U2 : class, IMeasurable
        {
            bool result = q1.Equals(q2);
            logger.Log($"Cross-category check: {q1.GetType().Name} vs {q2.GetType().Name} -> {result}");
            return result;
        }

        // ================= UC11 - Volume Measurement =================

        /// <summary>
        /// UC11: Creates and validates a QuantityVolume.
        /// Validates unit is not null and value is finite before delegating to entity.
        /// </summary>
        public QuantityVolume CreateVolume(double value, VolumeUnit unit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite (not NaN or Infinity).");

            if (!Enum.IsDefined(typeof(VolumeUnit), unit))
                throw new ArgumentException("Invalid volume unit provided.");

            QuantityVolume volume = new QuantityVolume(value, unit);
            logger.Log($"Created volume: {volume}");
            return volume;
        }

        /// <summary>
        /// UC11: Compares two QuantityVolume instances by normalizing to base unit (litre).
        /// </summary>
        public bool CompareVolume(QuantityVolume v1, QuantityVolume v2)
        {
            if (v1 == null || v2 == null)
                throw new ArgumentNullException("Volume quantities cannot be null.");

            bool result = v1.Equals(v2);
            logger.Log($"Compared volumes: {v1} == {v2} -> {result}");
            return result;
        }

        /// <summary>
        /// UC11: Converts a raw volume value from one unit to another. Returns converted double.
        /// </summary>
        public double ConvertVolume(double value, VolumeUnit fromUnit, VolumeUnit toUnit)
        {
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite (not NaN or Infinity).");

            double result = QuantityVolume.Convert(value, fromUnit, toUnit);
            logger.Log($"Converted volume: {value} {fromUnit} -> {result} {toUnit}");
            return result;
        }

        /// <summary>
        /// UC11: Converts a QuantityVolume instance to the target unit. Returns new QuantityVolume.
        /// </summary>
        public QuantityVolume ConvertVolume(QuantityVolume volume, VolumeUnit toUnit)
        {
            if (volume == null)
                throw new ArgumentNullException("Volume cannot be null.");

            if (!Enum.IsDefined(typeof(VolumeUnit), toUnit))
                throw new ArgumentException("Invalid target volume unit.");

            QuantityVolume result = volume.ConvertTo(toUnit);
            logger.Log($"Converted volume: {volume} -> {result}");
            return result;
        }

        /// <summary>
        /// UC11: Adds two volumes. Result is in unit of first operand.
        /// </summary>
        public QuantityVolume AddVolume(QuantityVolume v1, QuantityVolume v2)
        {
            if (v1 == null || v2 == null)
                throw new ArgumentNullException("Volume quantities cannot be null.");

            QuantityVolume result = v1.Add(v2);
            logger.Log($"Added volumes: {v1} + {v2} = {result}");
            return result;
        }

        /// <summary>
        /// UC11: Adds two volumes. Result is in the specified target unit.
        /// </summary>
        public QuantityVolume AddVolume(QuantityVolume v1, QuantityVolume v2, VolumeUnit targetUnit)
        {
            if (v1 == null || v2 == null)
                throw new ArgumentNullException("Volume quantities cannot be null.");

            if (!Enum.IsDefined(typeof(VolumeUnit), targetUnit))
                throw new ArgumentException("Invalid target volume unit.");

            QuantityVolume result = v1.Add(v2, targetUnit);
            logger.Log($"Added volumes: {v1} + {v2} = {result} (target: {targetUnit})");
            return result;
        }
    }
}