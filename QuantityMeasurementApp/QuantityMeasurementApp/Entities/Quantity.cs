using System;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.Entities
{
    /// <summary>
    /// UC10: Generic, type-safe Quantity class that works with any IMeasurable unit.
    /// Replaces both QuantityLength and QuantityWeight, eliminating code duplication.
    /// Adding a new measurement category requires only a new IMeasurable implementation.
    /// </summary>
    public class Quantity<U> where U : class, IMeasurable
    {
        private readonly double value;
        private readonly U unit;

        public double Value => value;
        public U Unit => unit;

        public Quantity(double value, U unit)
        {
            if (unit == null)
                throw new ArgumentException("Unit cannot be null.");
            if (!double.IsFinite(value))
                throw new ArgumentException("Value must be finite (not NaN or Infinity).");

            this.value = value;
            this.unit = unit;
        }

        // ===== Conversion =====

        public double ConvertToBaseUnit()
            => unit.ConvertToBaseUnit(value);

        public Quantity<U> ConvertTo(U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");

            double baseValue = unit.ConvertToBaseUnit(value);
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            double rounded = Math.Round(converted, 2, MidpointRounding.AwayFromZero);
            return new Quantity<U>(rounded, targetUnit);
        }

        // ===== Addition =====

        /// <summary>UC10: Adds two quantities. Result unit = this unit.</summary>
        public Quantity<U> Add(Quantity<U> other)
            => AddAndConvert(other, unit);

        /// <summary>UC10: Adds two quantities. Result unit = specified target unit.</summary>
        public Quantity<U> Add(Quantity<U> other, U targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentException("Target unit cannot be null.");
            return AddAndConvert(other, targetUnit);
        }

        private Quantity<U> AddAndConvert(Quantity<U> other, U targetUnit)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Cannot add null Quantity.");

            double sumBase = unit.ConvertToBaseUnit(value) + other.unit.ConvertToBaseUnit(other.value);
            double result = targetUnit.ConvertFromBaseUnit(sumBase);
            double rounded = Math.Round(result, 2, MidpointRounding.AwayFromZero);
            return new Quantity<U>(rounded, targetUnit);
        }

        // ===== Equality =====

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;

            // Type check: ensures Quantity<LengthUnitM> != Quantity<WeightUnitM>
            if (obj.GetType() != GetType()) return false;

            var that = (Quantity<U>)obj;

            // Cross-category safety: unit types must match
            if (unit.GetType() != that.unit.GetType()) return false;

            double thisBase = unit.ConvertToBaseUnit(value);
            double thatBase = that.unit.ConvertToBaseUnit(that.value);

            return Math.Abs(thisBase - thatBase) < 0.0001;
        }

        public override int GetHashCode()
            => unit.ConvertToBaseUnit(value).GetHashCode();

        public override string ToString()
            => $"Quantity({value}, {unit.GetUnitName()})";
    }
}
