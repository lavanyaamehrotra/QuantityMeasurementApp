using uc1_feet_equality.Entities;

namespace uc1_feet_equality.Interfaces
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
    }
}