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

        // Maximum allowed measurement limit
        // Prevents unrealistic or overflow-prone values
        private const double MAX_FEET_VALUE = 1_000_000;

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
    }
}