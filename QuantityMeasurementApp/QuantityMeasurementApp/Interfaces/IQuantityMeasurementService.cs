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
    }
    
}