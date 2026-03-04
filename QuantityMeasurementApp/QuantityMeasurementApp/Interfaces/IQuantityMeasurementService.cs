using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// Defines business operations for quantity measurement.
    /// UC1-UC2:  Feet and Inches operations.
    /// UC3-UC7:  Generic length operations.
    /// UC9:      Weight measurement operations.
    /// UC10:     Generic Quantity&lt;U&gt; operations via IMeasurable.
    /// UC11:     Volume measurement operations (Litre, Millilitre, Gallon).
    /// UC12:     Subtraction and Division operations on generic Quantity&lt;U&gt;.
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

        // ================= UC11 =================
        /// <summary>Creates and validates a QuantityVolume.</summary>
        QuantityVolume CreateVolume(double value, VolumeUnit unit);

        /// <summary>Compares two QuantityVolume instances (equality by base unit - litre).</summary>
        bool CompareVolume(QuantityVolume v1, QuantityVolume v2);

        /// <summary>Converts a raw volume value from one unit to another. Returns the converted double.</summary>
        double ConvertVolume(double value, VolumeUnit fromUnit, VolumeUnit toUnit);

        /// <summary>Converts a QuantityVolume instance to the target unit. Returns new QuantityVolume.</summary>
        QuantityVolume ConvertVolume(QuantityVolume volume, VolumeUnit toUnit);

        /// <summary>Adds two volumes. Result is in unit of the first operand.</summary>
        QuantityVolume AddVolume(QuantityVolume v1, QuantityVolume v2);

        /// <summary>Adds two volumes. Result is in the specified target unit.</summary>
        QuantityVolume AddVolume(QuantityVolume v1, QuantityVolume v2, VolumeUnit targetUnit);

        // ================= UC12 =================

        /// <summary>
        /// UC12: Subtracts q2 from q1. Result is in unit of q1 (implicit target).
        /// Cross-category subtraction is prevented by the generic type parameter.
        /// </summary>
        Quantity<U> SubtractGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable;

        /// <summary>
        /// UC12: Subtracts q2 from q1. Result is expressed in the specified target unit.
        /// </summary>
        Quantity<U> SubtractGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2, U targetUnit) where U : class, IMeasurable;

        /// <summary>
        /// UC12: Divides q1 by q2, returning a dimensionless scalar (double).
        /// Both quantities are normalised to base unit before dividing.
        /// Throws ArithmeticException if q2 resolves to zero.
        /// </summary>
        double DivideGenericQuantity<U>(Quantity<U> q1, Quantity<U> q2) where U : class, IMeasurable;

        // ================= UC14 =================

        /// <summary>UC14: Creates and validates a temperature Quantity.</summary>
        Quantity<TemperatureUnit> CreateTemperature(double value, TemperatureUnit unit);

        /// <summary>UC14: Compares two temperature quantities by normalizing to Kelvin.</summary>
        bool CompareTemperature(Quantity<TemperatureUnit> t1, Quantity<TemperatureUnit> t2);

        /// <summary>UC14: Converts a temperature quantity to the specified target unit.</summary>
        Quantity<TemperatureUnit> ConvertTemperature(Quantity<TemperatureUnit> temperature, TemperatureUnit targetUnit);
    }
}
