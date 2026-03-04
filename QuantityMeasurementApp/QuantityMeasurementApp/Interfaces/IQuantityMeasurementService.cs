using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Defines business operations for quantity measurement.
    /// UC1-UC2: Feet and Inches operations.
    /// UC3-UC7: Generic length operations.
    /// UC9: Weight measurement operations.
    /// UC10: Generic Quantity<U> operations via IMeasurable.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        // ================= UC1 =================
        Feet CreateFeet(double value);
        bool CompareFeet(Feet f1, Feet f2);

        // ================= UC2 =================
        Inches CreateInches(double value);
        bool CompareInches(Inches i1, Inches i2);
        string GetMeasurementAnalysis(double value1, double value2, string unit);

        // ================= UC3 =================
        QuantityLength CreateQuantity(double value, LengthUnit unit);
        bool CompareQuantity(QuantityLength q1, QuantityLength q2);

        // ================= UC5 =================
        double ConvertLength(double value, LengthUnit fromUnit, LengthUnit toUnit);
        QuantityLength ConvertLength(QuantityLength length, LengthUnit toUnit);

        // ================= UC6 =================
        QuantityLength AddLength(QuantityLength length1, QuantityLength length2);

        // ================= UC7 =================
        QuantityLength AddLength(QuantityLength length1, QuantityLength length2, LengthUnit targetUnit);

        // ================= UC9 =================
        QuantityWeight CreateWeight(double value, WeightUnit unit);
        bool CompareWeight(QuantityWeight w1, QuantityWeight w2);
        double ConvertWeight(double value, WeightUnit fromUnit, WeightUnit toUnit);
        QuantityWeight ConvertWeight(QuantityWeight weight, WeightUnit toUnit);
        QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2);
        QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2, WeightUnit targetUnit);

        // ================= UC10 =================
        /// <summary>Creates a generic Quantity for any IMeasurable unit type.</summary>
        Quantity<U> CreateGenericQuantity<U>(double value, U unit) where U : class, IMeasurable;

        /// <summary>Compares two generic Quantity instances of the same unit type.</summary>
        bool CompareGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable;

        /// <summary>Converts a generic Quantity to the specified target unit.</summary>
        Quantity<U> ConvertGenericQuantity<U>(Quantity<U> quantity, U targetUnit) where U : class, IMeasurable;

        /// <summary>Adds two generic Quantity instances. Result is in unit of first operand.</summary>
        Quantity<U> AddGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable;

        /// <summary>Adds two generic Quantity instances. Result is in specified target unit.</summary>
        Quantity<U> AddGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2, U targetUnit) where U : class, IMeasurable;

        /// <summary>
        /// UC10: Checks if two quantities of DIFFERENT unit types are equal.
        /// Always returns false — demonstrates cross-category prevention.
        /// </summary>
        bool CheckCrossCategory<U1, U2>(Quantity<U1> q1, Quantity<U2> q2)
            where U1 : class, IMeasurable
            where U2 : class, IMeasurable;
    }
}
