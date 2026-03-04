namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC10: Common contract for all measurement unit enums.
    /// Enables generic Quantity<U> to work with any measurement category.
    /// </summary>
    public interface IMeasurable
    {
        /// <summary>Returns the conversion factor relative to the base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a value from the base unit to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns a readable name for this unit.</summary>
        string GetUnitName();
    }
}
