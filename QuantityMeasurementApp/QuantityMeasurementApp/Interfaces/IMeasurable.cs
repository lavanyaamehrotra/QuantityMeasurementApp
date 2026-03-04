namespace QuantityMeasurementApp.Interfaces
{
    /// <summary>
    /// UC10: Common contract for all measurement unit enums.
    /// Enables generic Quantity&lt;U&gt; to work with any measurement category.
    ///
    /// UC14: Refactored to support selective arithmetic via default interface methods.
    ///       - SupportsArithmetic(): default returns true; TemperatureUnit overrides to false.
    ///       - ValidateOperationSupport(): default no-op; TemperatureUnit overrides to throw.
    ///       - Existing units (Length, Weight, Volume) require NO changes — defaults apply.
    ///       - Adheres to Interface Segregation Principle: categories opt in to arithmetic.
    ///       - C# equivalent of Java @FunctionalInterface SupportsArithmetic pattern.
    /// </summary>
    public interface IMeasurable
    {
        // ===== Mandatory Conversion Methods (all categories must implement) =====

        /// <summary>Returns the conversion factor relative to the base unit.</summary>
        double GetConversionFactor();

        /// <summary>Converts a value in this unit to the base unit.</summary>
        double ConvertToBaseUnit(double value);

        /// <summary>Converts a value from the base unit to this unit.</summary>
        double ConvertFromBaseUnit(double baseValue);

        /// <summary>Returns a readable name for this unit.</summary>
        string GetUnitName();

        // ===== UC14: Optional Arithmetic Operation Support (default methods) =====

        /// <summary>
        /// UC14: Indicates whether this unit supports arithmetic operations (add/subtract/divide).
        /// Default returns true — all existing units (Length, Weight, Volume) inherit this.
        /// TemperatureUnit overrides to return false.
        /// C# equivalent of Java @FunctionalInterface SupportsArithmetic lambda: () -> true.
        /// </summary>
        bool SupportsArithmetic() => true;

        /// <summary>
        /// UC14: Validates that the named arithmetic operation is supported for this unit.
        /// Default is a no-op — all existing units pass silently (backward compatible).
        /// TemperatureUnit overrides to throw NotSupportedException with a clear message.
        /// Called by Quantity&lt;U&gt;.ValidateArithmeticOperands() before any arithmetic.
        /// </summary>
        void ValidateOperationSupport(string operation)
        {
            // Default: no-op. Length, Weight, Volume units support all operations.
            // TemperatureUnit overrides this to throw NotSupportedException.
        }
    }
}
