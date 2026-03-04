using QuantityMeasurementApp.Entities;

namespace QuantityMeasurementApp.Interfaces
{
    
    /// <summary>
    /// Defines business operations for quantity measurement.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        /// <summary>
        /// Creates validated Feet object.
        /// </summary>
        Feet CreateFeet(double value);

        /// <summary>
        /// Compares two feet measurements.
        /// </summary>
        bool CompareFeet(Feet f1, Feet f2);

         // UC2 (NEW)
        Inches CreateInches(double value);
        bool CompareInches(Inches i1, Inches i2);
        string GetMeasurementAnalysis(double value1, double value2, string unit);

        QuantityLength CreateQuantity(double value, LengthUnit unit);
        bool CompareQuantity(QuantityLength q1, QuantityLength q2);

        /// <summary>
        /// UC5: Converts length from one unit to another. Returns numeric value in target unit.
        /// </summary>
        double ConvertLength(double value, LengthUnit fromUnit, LengthUnit toUnit);

        /// <summary>
        /// UC5: Converts an existing QuantityLength to the target unit. Returns new instance.
        /// </summary>
        QuantityLength ConvertLength(QuantityLength length, LengthUnit toUnit);

        /// <summary>
        /// UC6: Adds two QuantityLength instances. Result is in unit of first operand.
        /// </summary>
        QuantityLength AddLength(QuantityLength length1, QuantityLength length2);

        /// <summary>
        /// UC7: Adds two QuantityLength instances. Result is in specified target unit.
        /// </summary>
        QuantityLength AddLength(QuantityLength length1, QuantityLength length2, LengthUnit targetUnit);

        // ================= UC9 - Weight Measurement =================

        /// <summary>UC9: Validates input and creates a QuantityWeight object.</summary>
        QuantityWeight CreateWeight(double value, WeightUnit unit);

        /// <summary>UC9: Compares two weight measurements for equality.</summary>
        bool CompareWeight(QuantityWeight w1, QuantityWeight w2);

        /// <summary>UC9: Converts a weight value from one unit to another. Returns numeric result.</summary>
        double ConvertWeight(double value, WeightUnit fromUnit, WeightUnit toUnit);

        /// <summary>UC9: Converts an existing QuantityWeight to the target unit. Returns new instance.</summary>
        QuantityWeight ConvertWeight(QuantityWeight weight, WeightUnit toUnit);

        /// <summary>UC9: Adds two weights. Result is in unit of first operand.</summary>
        QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2);

        /// <summary>UC9: Adds two weights. Result is in specified target unit.</summary>
        QuantityWeight AddWeight(QuantityWeight w1, QuantityWeight w2, WeightUnit targetUnit);
    }
    
}