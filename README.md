# 📏 Quantity Measurement Application

A progressive **.NET Console Application** for performing measurement comparison, conversion, and arithmetic operations across **Length, Weight, Volume, and Temperature measurements**.  
This project demonstrates incremental software development using clean architecture and SOLID principles.

🚀 The system evolves from simple equality checks to a fully generic, extensible, multi-category measurement framework.

---

# 🎯 Overview

The **Quantity Measurement Application** is built step-by-step through structured use cases.

From **UC1 → UC14**, the application grows from a simple measurement comparison system into a flexible quantity engine supporting:

✅ Equality comparison  
✅ Generic design  
✅ Multiple measurement units  
✅ Unit conversion  
✅ Arithmetic operations  
✅ Flexible output units  
✅ Weight, Volume, and Temperature categories  
✅ Selective arithmetic support per category  

---

## ✨ Key Highlights

- 📐 Length measurement system (Feet, Inch, Yard, Centimeter)
- ⚖ Weight measurement system (Kilogram, Gram, Pound)
- 🧪 Volume measurement system (Litre, Millilitre, Gallon)
- 🌡 Temperature measurement system (Celsius, Fahrenheit, Kelvin)
- 🔄 Cross-unit equality and conversion across all categories
- ➕ Arithmetic operations (Add, Subtract, Divide)
- 🚫 Selective arithmetic — temperature rejects unsupported operations
- 🧩 Generic Quantity class via IMeasurable interface
- ♻️ DRY principle via centralized arithmetic logic

---

# 🚀 Features by Use Case

---

## ✅ UC1: Feet Measurement Equality 👣

Basic implementation for comparing measurements.

```csharp
var feet1 = new Feet(1.0);
var feet2 = new Feet(1.0);

bool areEqual = feet1.Equals(feet2); // true
```

## ✨ Features
* ✔ Value-based equality
* ✔ Proper null handling
* ✔ Reflexive, symmetric & transitive comparison
* ✔ Foundation for future measurements

---

## ✅ UC2: Inch Measurement Equality 📏

Adds Inch measurement using the same equality rules.

```csharp
var inch1 = new Inch(1.0);
var inch2 = new Inch(1.0);

bool areEqual = inch1.Equals(inch2);
```

## ✨ Features
* ✔ Independent Inch class
* ✔ Same equality contract as Feet
* ✔ Introduced duplication (resolved in UC3)

---

## ✅ UC3: Generic Quantity Class 🧩

Removes duplication using a reusable class.

```csharp
public class Quantity
{
    private readonly double _value;
    private readonly LengthUnit _unit;

    public Quantity(double value, LengthUnit unit)
    {
        _value = value;
        _unit = unit;
    }
}
```

## ✨ Features
* ✔ Single reusable class
* ✔ Enum-based units
* ✔ DRY principle applied
* ✔ Cross-unit equality
* ✔ Supported Units: FEET, INCH

---

## ✅ UC4: Extended Unit Support 📐

Adds additional length units.

```csharp
public enum LengthUnit
{
    FEET,
    INCH,
    YARD,
    CENTIMETER
}
```

## ✨ Features
* ✔ Yard support
* ✔ Centimeter support
* ✔ Cross-unit comparison

| Unit | Conversion   |
| ---- | ------------ |
| 1 ft | 12 in        |
| 1 yd | 3 ft         |
| 1 cm | 0.0328084 ft |

1 yd = 3 ft = 36 in = 91.44 cm

---

## ✅ UC5: Unit Conversion 🔄

Convert quantities between units.

```csharp
var feet = new Quantity(1.0, LengthUnit.FEET);

var inches = feet.ConvertTo(LengthUnit.INCH);
var yards  = feet.ConvertTo(LengthUnit.YARD);
```

## ✨ Features
* ✔ ConvertTo() method
* ✔ Base unit normalization
* ✔ Bidirectional conversion

| From | To   | Formula    |
| ---- | ---- | ---------- |
| FEET | INCH | × 12       |
| INCH | FEET | ÷ 12       |
| YARD | FEET | × 3        |
| CM   | INCH | × 0.393701 |

---

## ✅ UC6: Addition Operations ➕

Supports arithmetic addition across units.

```csharp
var feet   = new Quantity(1.0,  LengthUnit.FEET);
var inches = new Quantity(12.0, LengthUnit.INCH);

var sum = feet.Add(inches); // 2 ft
```

## ✨ Features
* ✔ Cross-unit addition
* ✔ Result in first operand unit
* ✔ Immutable operations
* ✔ Returns new object

| Operation    | Result |
| ------------ | ------ |
| 1 ft + 2 ft  | 3 ft   |
| 1 ft + 12 in | 2 ft   |
| 12 in + 1 ft | 24 in  |

---

## ✅ UC7: Addition with Target Unit 🎯

Allows specifying desired output unit.

```csharp
var sum = feet.Add(inches, LengthUnit.YARD);
```

## ✨ Features
* ✔ Overloaded Add() method
* ✔ Flexible output unit

| Operation    | Target | Result   |
| ------------ | ------ | -------- |
| 1 ft + 12 in | FEET   | 2 ft     |
| 1 ft + 12 in | INCH   | 24 in    |
| 1 ft + 12 in | YARD   | 0.667 yd |

---

## ✅ UC8: Unit Enum Refactoring 🔧

Moves conversion responsibility into the `LengthUnit` enum itself using extension methods. Before UC8, conversion logic lived inside `QuantityLength`. After UC8, each unit knows how to convert itself.

```csharp
double feet = LengthUnit.INCH.ConvertToBaseUnit(12.0);  // 1.0 ft
double inch = LengthUnit.FEET.ConvertFromBaseUnit(1.0); // 12.0 in
```

## ✨ Features
* ✔ Extension methods on LengthUnit enum
* ✔ ConvertToBaseUnit() — value in unit to feet
* ✔ ConvertFromBaseUnit() — feet back to unit
* ✔ Single Responsibility — unit owns its conversion
* ✔ All existing UC3–UC7 behaviour preserved

| Unit       | Factor to Feet |
| ---------- | -------------- |
| FEET       | × 1.0          |
| INCH       | × 0.0833       |
| YARD       | × 3.0          |
| CENTIMETER | × 0.0328084    |

---

## ✅ UC9: Weight Measurement ⚖

Introduces weight measurement using a dedicated `WeightUnit` enum and `QuantityWeight` class. Follows the same conversion pattern as length. Base unit is **Kilogram**.

```csharp
var kg = new QuantityWeight(1.0,    WeightUnit.KILOGRAM);
var g  = new QuantityWeight(1000.0, WeightUnit.GRAM);

bool equal = kg.Equals(g); // true

var sum = kg.Add(g); // 2.0 KILOGRAM
```

## ✨ Features
* ✔ WeightUnit enum: KILOGRAM, GRAM, POUND
* ✔ Cross-unit equality (1 kg == 1000 g)
* ✔ ConvertTo() — returns new QuantityWeight
* ✔ Add() — implicit and explicit target unit
* ✔ Immutable operations — originals unchanged

| Unit     | Conversion to KG |
| -------- | ---------------- |
| KILOGRAM | × 1.0            |
| GRAM     | × 0.001          |
| POUND    | × 0.453592       |

| Operation     | Result  |
| ------------- | ------- |
| 1 kg + 1000 g | 2 kg    |
| 500 g + 1 kg  | 1500 g  |
| 1 lb → kg     | 0.45 kg |

---

## ✅ UC10: Generic Quantity Class 🧩

Introduces the `IMeasurable` interface and a generic `Quantity<U>` class that works with any measurement category. Eliminates the need for separate `QuantityLength`, `QuantityWeight` classes for generic operations.

```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();
}

var feet   = new Quantity<LengthUnitM>(1.0,  LengthUnitM.FEET);
var inches = new Quantity<LengthUnitM>(12.0, LengthUnitM.INCHES);

bool equal = feet.Equals(inches);                // true
var  sum   = feet.Add(inches);                   // Quantity(2.0, FEET)
var  conv  = feet.ConvertTo(LengthUnitM.INCHES); // Quantity(12.0, INCHES)
```

## ✨ Features
* ✔ IMeasurable interface as common contract
* ✔ Class-based unit constants (LengthUnitM, WeightUnitM)
* ✔ Generic Quantity works with any IMeasurable unit
* ✔ Cross-unit equality via base unit normalization
* ✔ Cross-category prevention — length cannot equal weight
* ✔ Add() with implicit and explicit target unit

| Unit Class  | Constants                        | Base Unit |
| ----------- | -------------------------------- | --------- |
| LengthUnitM | FEET, INCHES, YARDS, CENTIMETERS | FEET      |
| WeightUnitM | KILOGRAM, GRAM, POUND            | KILOGRAM  |

---

## ✅ UC11: Volume Measurement 🧪

Extends the system with volume measurement using `VolumeUnit` enum and `QuantityVolume` class. Follows the exact same pattern as weight. Base unit is **Litre**.

```csharp
var litre = new QuantityVolume(1.0,    VolumeUnit.LITRE);
var ml    = new QuantityVolume(1000.0, VolumeUnit.MILLILITRE);

bool equal  = litre.Equals(ml); // true
var  result = litre.Add(ml);    // 2.0 LITRE
```

## ✨ Features
* ✔ VolumeUnit enum: LITRE, MILLILITRE, GALLON
* ✔ Cross-unit equality (1 L == 1000 mL)
* ✔ ConvertTo() — returns new QuantityVolume
* ✔ Add() — implicit and explicit target unit
* ✔ VolumeUnitM class for use with generic Quantity

| Unit       | Conversion to Litre |
| ---------- | ------------------- |
| LITRE      | × 1.0               |
| MILLILITRE | × 0.001             |
| GALLON     | × 3.78541           |

| Operation         | Result  |
| ----------------- | ------- |
| 1 L + 1000 mL     | 2 L     |
| 1 gallon → litres | 3.79 L  |
| 500 mL + 1 L      | 1500 mL |

---

## ✅ UC12: Subtraction and Division ➖➗

Extends `Quantity<U>` with two new arithmetic operations. Division returns a dimensionless scalar (ratio of two quantities). Both operations work across all categories that implement IMeasurable.

```csharp
var q1 = new Quantity<LengthUnitM>(10.0, LengthUnitM.FEET);
var q2 = new Quantity<LengthUnitM>(2.0,  LengthUnitM.FEET);

var    diff  = q1.Subtract(q2); // Quantity(8.0, FEET)
double ratio = q1.Divide(q2);   // 5.0 (dimensionless)
```

## ✨ Features
* ✔ Subtract() — implicit and explicit target unit
* ✔ Divide() — returns dimensionless double scalar
* ✔ Division by zero throws ArithmeticException
* ✔ Negative results valid for subtraction
* ✔ Cross-category operations blocked by generics
* ✔ Works across Length, Weight, and Volume

| Operation    | Result              |
| ------------ | ------------------- |
| 10 ft - 2 ft | 8 ft                |
| 1 kg - 500 g | 0.5 kg              |
| 10 ft / 2 ft | 5.0 (scalar)        |
| 1 L / 500 mL | 2.0 (scalar)        |
| x / 0        | ArithmeticException |

---

## ✅ UC13: Centralized Arithmetic Logic (DRY) ♻️

Internal refactoring of `Quantity<U>`. No new user-facing features — all public API and behaviour stays identical to UC12. Eliminates code duplication across Add, Subtract, and Divide by introducing a centralized `ArithmeticOperation` enum and two private helper methods.

```csharp
// UC13 internal design — public API unchanged
private enum ArithmeticOperation { ADD, SUBTRACT, DIVIDE }

private static double Compute(ArithmeticOperation operation, double a, double b)
{
    return operation switch
    {
        ArithmeticOperation.ADD      => a + b,
        ArithmeticOperation.SUBTRACT => a - b,
        ArithmeticOperation.DIVIDE   => a / b,
        _ => throw new InvalidOperationException()
    };
}
```

## ✨ Features
* ✔ ArithmeticOperation enum dispatches all operations
* ✔ ValidateArithmeticOperands() — single validation point for all operations
* ✔ PerformBaseArithmetic() — single conversion and compute point
* ✔ Adding a new operation requires only one new enum case
* ✔ All UC12 test cases pass without modification
* ✔ No change to public interface or behaviour

| Before UC13                   | After UC13                        |
| ----------------------------- | --------------------------------- |
| Validation duplicated x3      | Single ValidateArithmeticOperands |
| Base conversion duplicated x3 | Single PerformBaseArithmetic      |
| 3 separate compute blocks     | Single Compute() via enum         |

---

## ✅ UC14: Temperature Measurement 🌡

Introduces temperature measurement and reveals a fundamental limitation in the current IMeasurable design — not all measurement categories support arithmetic. Refactors IMeasurable with default interface methods so temperature can support equality and conversion only, while Length, Weight, and Volume continue working unchanged.

```csharp
// Equality across units
var c = new Quantity<TemperatureUnit>(0.0,  TemperatureUnit.CELSIUS);
var f = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

bool equal = c.Equals(f); // true

// Conversion
var result = c.ConvertTo(TemperatureUnit.FAHRENHEIT); // Quantity(32.0, FAHRENHEIT)

// Arithmetic is blocked
c.Add(f); // throws NotSupportedException
```

## ✨ Features
* ✔ TemperatureUnit: CELSIUS, FAHRENHEIT, KELVIN
* ✔ Base unit is Kelvin — all comparisons normalize through Kelvin
* ✔ Non-linear conversion formulas via Func lambda expressions
* ✔ IMeasurable refactored with default methods (backward compatible)
* ✔ SupportsArithmetic() — returns false for temperature
* ✔ ValidateOperationSupport() — throws NotSupportedException for temperature
* ✔ Cross-category prevention — temperature cannot equal length, weight, or volume
* ✔ All UC1–UC13 tests pass without modification

### Temperature Conversion Formulas

| From       | To         | Formula                 |
| ---------- | ---------- | ----------------------- |
| Celsius    | Fahrenheit | (C x 9/5) + 32          |
| Fahrenheit | Celsius    | (F - 32) x 5/9          |
| Celsius    | Kelvin     | C + 273.15              |
| Kelvin     | Celsius    | K - 273.15              |
| Fahrenheit | Kelvin     | (F - 32) x 5/9 + 273.15 |

### Equality Examples

| Input                                | Output |
| ------------------------------------ | ------ |
| 0 degree Celsius == 32 Fahrenheit    | true   |
| 100 degree Celsius == 212 Fahrenheit | true   |
| -40 degree Celsius == -40 Fahrenheit | true   |
| 0 degree Celsius == 273.15 Kelvin    | true   |

### Operation Support

| Category    | SupportsArithmetic() |
| ----------- | -------------------- |
| Length      | true                 |
| Weight      | true                 |
| Volume      | true                 |
| Temperature | false                |

### IMeasurable Interface — UC14 Evolution

```csharp
public interface IMeasurable
{
    // Mandatory — all categories implement
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();

    // Default — optional override added in UC14
    bool SupportsArithmetic() => true;
    void ValidateOperationSupport(string operation) { } // no-op default
}
```

---

## 🏗 Architecture

```text
-----------------------------------------
|  PresentationLayer                    |
|    Menu.cs                            |
-----------------------------------------
|  BusinessLogicLayer                   |
|    QuantityMeasurementService.cs      |
|    IQuantityMeasurementService.cs     |
-----------------------------------------
|  Entities                             |
|    Quantity<U>      IMeasurable       |
|    LengthUnitM      WeightUnitM       |
|    VolumeUnitM      TemperatureUnit   |
|    QuantityLength   QuantityWeight    |
|    QuantityVolume                     |
-----------------------------------------
|  DataAccessLayer                      |
|    MeasurementLogger.cs               |
-----------------------------------------
```

### Dependency Direction

| Layer              | Depends On                                    |
| ------------------ | --------------------------------------------- |
| Presentation       | BusinessLogicLayer, Entities, Interfaces      |
| BusinessLogicLayer | Entities, Interfaces, DataAccessLayer         |
| Entities           | Interfaces only                               |
| DataAccessLayer    | Nothing (System only)                         |

---

## Principles Applied

| Principle             | Implementation                                       |
| --------------------- | ---------------------------------------------------- |
| DRY                   | Centralized ArithmeticOperation enum (UC13)          |
| SRP                   | Each unit owns its own conversion logic              |
| Open/Closed           | New categories added without modifying existing ones |
| Interface Segregation | IMeasurable default methods for optional arithmetic  |
| Dependency Inversion  | Service depends on IQuantityMeasurementService       |
| Immutability          | All operations return new objects                    |

---

## 📁 Project Structure

```text
QuantityMeasurementApp/
|
|-- QuantityMeasurementApp/
|   |-- BusinessLogicLayer/
|   |   `-- QuantityMeasurementService.cs
|   |
|   |-- DataAccessLayer/
|   |   `-- MeasurementLogger.cs
|   |
|   |-- Entities/
|   |   |-- Feet.cs
|   |   |-- Inches.cs
|   |   |-- LengthUnit.cs
|   |   |-- LengthUnitMeasurable.cs
|   |   |-- Quantity.cs
|   |   |-- QuantityLength.cs
|   |   |-- QuantityVolume.cs
|   |   |-- QuantityWeight.cs
|   |   |-- TemperatureUnit.cs
|   |   |-- VolumeUnit.cs
|   |   |-- VolumeUnitM.cs
|   |   |-- WeightUnit.cs
|   |   `-- WeightUnitMeasurable.cs
|   |
|   |-- Interfaces/
|   |   |-- IMeasurable.cs
|   |   `-- IQuantityMeasurementService.cs
|   |
|   |-- PresentationLayer/
|   |   `-- Menu.cs
|   |
|   |-- Program.cs
|   `-- QuantityMeasurementApp.csproj
|
`-- QuantityMeasurementApp.Tests/
    |-- FeetEqualityTests.cs
    |-- InchesEqualityTests.cs
    |-- QuantityLengthTests.cs
    |-- ExtendedUnitSupportMSTests.cs
    |-- UnitConversionTests.cs
    |-- AdditionTests.cs
    |-- AdditionWithTargetUnitTests.cs
    |-- LengthUnitRefactoringTests.cs
    |-- WeightMeasurementTests.cs
    |-- GenericQuantityTests.cs
    |-- VolumeMeasurementTest.cs
    |-- SubtractionDivisionTests.cs
    |-- CentralizedArithmeticLogicTests.cs
    `-- TemperatureMeasurementTests.cs
```

---

## 🖥 Console UI Menu

```text
+--------------------------------------------------+
|           Quantity Measurement App               |
+--------------------------------------------------+
|                                                  |
|   1.  Feet Equality                              |
|   2.  Feet & Inches Equality                     |
|   3.  Generic Length (Generics)                  |
|   4.  Extended Unit Support                      |
|   5.  Unit Conversion                            |
|   6.  Addition of Two Length Units               |
|   7.  Addition With Target                       |
|   8.  Weight Measurement                         |
|   9.  Generic Quantity (Multi-Category)          |
|  10.  Volume Measurement                         |
|  11.  Subtraction & Division Operations          |
|  12.  Temperature Measurement                    |
|  13.  Exit                                       |
|                                                  |
+--------------------------------------------------+
```

---

## 📊 Test Summary

| Test File                          | UC Coverage | Tests   |
| ---------------------------------- | ----------- | ------- |
| FeetEqualityTests.cs               | UC1         | 14      |
| InchesEqualityTests.cs             | UC2         | 10      |
| QuantityLengthTests.cs             | UC3         | 12      |
| ExtendedUnitSupportMSTests.cs      | UC4         | 17      |
| UnitConversionTests.cs             | UC5         | 12      |
| AdditionTests.cs                   | UC6         | 12      |
| AdditionWithTargetUnitTests.cs     | UC7         | 14      |
| LengthUnitRefactoringTests.cs      | UC8         | 25      |
| WeightMeasurementTests.cs          | UC9         | 26      |
| GenericQuantityTests.cs            | UC10        | 35      |
| VolumeMeasurementTest.cs           | UC11        | 50      |
| SubtractionDivisionTests.cs        | UC12        | 39      |
| CentralizedArithmeticLogicTests.cs | UC13        | 48      |
| TemperatureMeasurementTests.cs     | UC14        | 41      |
| **Total**                          |             | **355** |

---

## 📥 Clone Repository

```bash
git clone https://github.com/lavanyaamehrotra/QuantityMeasurementApp.git
cd QuantityMeasurementApp
```

## 🏗 Build

```bash
dotnet build
```

## ▶ Run

```bash
dotnet run
```

---

| Metric            | Value                               |
| ----------------- | ----------------------------------- |
| Use Cases         | 14 (UC1 to UC14)                    |
| Measurement Types | Length, Weight, Volume, Temperature |
| Units Supported   | 11 across 4 categories              |
| Arithmetic        | Add, Subtract, Divide               |
| Conversion        | All categories                      |
| Equality          | All categories including cross-unit |
| Test Cases        | 355 (all passing)                   |
