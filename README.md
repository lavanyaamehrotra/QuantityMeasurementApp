# 📏 Quantity Measurement Application

A progressive **.NET Console Application** for performing measurement comparison, conversion, and arithmetic operations across **Length, Weight, Volume, and Temperature measurements**.  
This project demonstrates incremental software development using clean architecture and SOLID principles.

🚀 The system evolves from simple equality checks to a fully layered, extensible measurement framework with Controller, Service, Repository, and Model separation.

---

# 🎯 Overview

The **Quantity Measurement Application** is built step-by-step through structured use cases.

From **UC1 → UC15**, the application grows from a simple measurement comparison system into a fully layered quantity engine supporting:

✅ Equality comparison  
✅ Generic design  
✅ Multiple measurement units  
✅ Unit conversion  
✅ Arithmetic operations  
✅ Flexible output units  
✅ Weight, Volume, and Temperature categories  
✅ Selective arithmetic support per category  
✅ Full layer separation — Controller, Service, Repository, Model  
✅ Operation history with persistent in-memory cache  

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
- 🏗 Full layer separation with Dependency Injection (UC15)
- 📦 QuantityDTO as layer-agnostic data contract (UC15)
- 🗂 Operation history via in-memory Repository (UC15)

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

Moves conversion responsibility into the `LengthUnit` enum itself using extension methods.
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

Introduces weight measurement. Base unit is **Kilogram**.
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

---

## ✅ UC10: Generic Quantity Class 🧩

Introduces the `IMeasurable` interface and a generic `Quantity<U>` class.
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

| Unit Class  | Constants                        | Base Unit |
| ----------- | -------------------------------- | --------- |
| LengthUnitM | FEET, INCHES, YARDS, CENTIMETERS | FEET      |
| WeightUnitM | KILOGRAM, GRAM, POUND            | KILOGRAM  |

---

## ✅ UC11: Volume Measurement 🧪

Extends the system with volume measurement. Base unit is **Litre**.
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
* ✔ VolumeUnitM class for use with generic Quantity

| Unit       | Conversion to Litre |
| ---------- | ------------------- |
| LITRE      | × 1.0               |
| MILLILITRE | × 0.001             |
| GALLON     | × 3.78541           |

---

## ✅ UC12: Subtraction and Division ➖➗

Extends `Quantity<U>` with subtraction and division.
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

| Operation    | Result              |
| ------------ | ------------------- |
| 10 ft - 2 ft | 8 ft                |
| 1 kg - 500 g | 0.5 kg              |
| 10 ft / 2 ft | 5.0 (scalar)        |
| x / 0        | ArithmeticException |

---

## ✅ UC13: Centralized Arithmetic Logic (DRY) ♻️

Internal refactoring of `Quantity<U>`. No new user-facing features — all public API identical to UC12.
```csharp
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
* ✔ ValidateArithmeticOperands() — single validation point
* ✔ PerformBaseArithmetic() — single conversion and compute point
* ✔ All UC12 test cases pass without modification

| Before UC13                   | After UC13                        |
| ----------------------------- | --------------------------------- |
| Validation duplicated x3      | Single ValidateArithmeticOperands |
| Base conversion duplicated x3 | Single PerformBaseArithmetic      |
| 3 separate compute blocks     | Single Compute() via enum         |

---

## ✅ UC14: Temperature Measurement 🌡

Introduces temperature measurement with selective arithmetic support.
```csharp
var c = new Quantity<TemperatureUnit>(0.0,  TemperatureUnit.CELSIUS);
var f = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.FAHRENHEIT);

bool equal = c.Equals(f); // true

var result = c.ConvertTo(TemperatureUnit.FAHRENHEIT); // Quantity(32.0, FAHRENHEIT)

c.Add(f); // throws NotSupportedException
```

## ✨ Features
* ✔ TemperatureUnit: CELSIUS, FAHRENHEIT, KELVIN
* ✔ Base unit is Kelvin — all comparisons normalize through Kelvin
* ✔ Non-linear conversion formulas via Func lambda expressions
* ✔ IMeasurable refactored with default methods (backward compatible)
* ✔ SupportsArithmetic() — returns false for temperature
* ✔ ValidateOperationSupport() — throws NotSupportedException for temperature

### Temperature Conversion Formulas

| From       | To         | Formula                 |
| ---------- | ---------- | ----------------------- |
| Celsius    | Fahrenheit | (C × 9/5) + 32          |
| Fahrenheit | Celsius    | (F − 32) × 5/9          |
| Celsius    | Kelvin     | C + 273.15              |
| Kelvin     | Celsius    | K − 273.15              |
| Fahrenheit | Kelvin     | (F − 32) × 5/9 + 273.15 |

### Equality Examples

| Input           | Output |
| --------------- | ------ |
| 0°C == 32°F     | true   |
| 100°C == 212°F  | true   |
| −40°C == −40°F  | true   |
| 0°C == 273.15 K | true   |

### IMeasurable Interface — UC14 Evolution
```csharp
public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();

    bool SupportsArithmetic() => true;                   // default
    void ValidateOperationSupport(string operation) { }  // default no-op
}
```

---

## ✅ UC15: Layer Separation — Controller, Service, Repository, Model 🏗

Introduces full architectural layer separation. The application is restructured into 5 distinct projects, each with a single responsibility. All communication between layers uses `QuantityDTO` as a string-based, layer-agnostic data contract. Every operation is automatically persisted to a singleton in-memory repository for history tracking.
```csharp
// Program.cs — wires all layers together via Dependency Injection
IQuantityMeasurementRepository repo       = QuantityMeasurementCacheRepository.Instance;
IQuantityMeasurementService    service    = new QuantityMeasurementServiceImpl(repo);
QuantityMeasurementController  controller = new QuantityMeasurementController(service, repo);

controller.Start();
```
```csharp
// Controller — builds QuantityDTO from user input, calls service, displays result
var q1     = new QuantityDTO(1.0, "FEET",   "LENGTH");
var target = new QuantityDTO(0.0, "INCHES", "LENGTH");

string output = controller.PerformConversion(q1, target);
// → "Conversion Result: 12 INCHES"
```
```csharp
// Service — accepts QuantityDTO, returns QuantityDTO, saves entity to repo
QuantityDTO result = service.Convert(q1, target);
// internally: DTO → QuantityModel → Quantity<U> → business logic → DTO
```

## ✨ Features
* ✔ `QuantityDTO` — immutable data contract between all layers (Value, UnitName, Category)
* ✔ `IQuantityMeasurementService` — interface with Compare, Convert, Add, Subtract, Divide
* ✔ `QuantityMeasurementServiceImpl` — validates, executes, saves, and returns DTO
* ✔ `QuantityMeasurementController` — menu-driven UI, zero business logic
* ✔ `QuantityMeasurementCacheRepository` — singleton in-memory history store
* ✔ `QuantityMeasurementEntity` — immutable record of each operation (stored in repo)
* ✔ `QuantityModel<U>` — internal service-layer wrapper for Quantity values
* ✔ `QuantityMeasurementException` — domain exception wrapping all service errors
* ✔ Dependency Injection — service and repo injected into controller via constructor
* ✔ Factory Pattern — `Program.cs` creates and wires all dependencies
* ✔ Unit aliases accepted (ft, cm, kg, ml, etc.) — case-insensitive input
* ✔ Validation — rejects negative values and values > 1,000,000
* ✔ Operation history viewable from the main menu

### UC15 Data Flow
```
User Input
    ↓
Controller (builds QuantityDTO from input)
    ↓
Service (DTO → QuantityModel → Quantity<U> → business logic)
    ↓
Repository (saves QuantityMeasurementEntity)
    ↓
Service (returns result QuantityDTO)
    ↓
Controller (formats and displays output)
```

### QuantityDTO Contract
```csharp
public class QuantityDTO
{
    public double Value    { get; }
    public string UnitName { get; }   // normalized to UPPERCASE
    public string Category { get; }   // LENGTH | WEIGHT | VOLUME | TEMPERATURE
}
```

### Service Interface
```csharp
public interface IQuantityMeasurementService
{
    QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2);
    QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnitDTO);
    QuantityDTO Add(QuantityDTO q1, QuantityDTO q2);
    QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2);
    QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2);
}
```

### Controller Public API
```csharp
string PerformComparison(QuantityDTO q1, QuantityDTO q2)
string PerformConversion(QuantityDTO q1, QuantityDTO targetUnit)
string PerformAddition(QuantityDTO q1, QuantityDTO q2)
string PerformSubtraction(QuantityDTO q1, QuantityDTO q2)
string PerformDivision(QuantityDTO q1, QuantityDTO q2)
```

### Supported Unit Aliases (Case-Insensitive)

| Category    | Full Name   | Short Form   |
| ----------- | ----------- | ------------ |
| Length      | feet        | ft           |
| Length      | inches      | in           |
| Length      | yards       | yd           |
| Length      | centimeters | cm           |
| Weight      | kilogram    | kg           |
| Weight      | gram        | g, gr        |
| Weight      | pound       | lb, lbs      |
| Volume      | litre       | l, lt, ltr   |
| Volume      | millilitre  | ml           |
| Volume      | gallon      | gal          |
| Temperature | celsius     | c, cel       |
| Temperature | fahrenheit  | f, fah, fahr |
| Temperature | kelvin      | k, kel       |

### Input Validation

| Rule               | Behaviour                                 |
| ------------------ | ----------------------------------------- |
| Negative values    | Rejected — `QuantityMeasurementException` |
| Value > 1,000,000  | Rejected — `QuantityMeasurementException` |
| Null operand       | Rejected — `QuantityMeasurementException` |
| Cross-category op  | Rejected — `QuantityMeasurementException` |
| Temperature arith. | Rejected — `QuantityMeasurementException` |
| Division by zero   | Rejected — `QuantityMeasurementException` |

---

# 🏗 Architecture

## Project Structure (UC15)
```
QuantityMeasurementApp.slnx
│
├── QuantityMeasurementApp/               ← Presentation Layer (Entry Point + Controller)
│   ├── Program.cs                        ← Wires layers via DI (Factory Pattern)
│   ├── QuantityMeasurementController.cs  ← Menu UI, builds DTOs, displays results
│   └── QuantityMeasurementApp.csproj
│
├── QuantityMeasurementBusinessLayer/     ← Business Logic Layer
│   ├── Interface/
│   │   └── IQuantityMeasurementService.cs
│   ├── Service/
│   │   └── QuantityMeasurementServiceImpl.cs
│   ├── Exception/
│   │   └── QuantityMeasurementException.cs
│   └── QuantityMeasurementBusinessLayer.csproj
│
├── QuantityMeasurementModel/             ← Domain Model Layer
│   ├── Dto/
│   │   └── QuantityDTO.cs               ← Data contract between layers
│   ├── Entities/
│   │   ├── Quantity.cs                  ← Generic Quantity<U>
│   │   ├── LengthUnitMeasurable.cs      ← LengthUnitM
│   │   ├── WeightUnitMeasurable.cs      ← WeightUnitM
│   │   ├── VolumeUnitM.cs
│   │   ├── TemperatureUnit.cs
│   │   ├── Feet.cs / Inches.cs
│   │   ├── LengthUnit.cs / WeightUnit.cs / VolumeUnit.cs
│   │   └── QuantityLength.cs / QuantityWeight.cs / QuantityVolume.cs
│   ├── Interfaces/
│   │   └── IMeasurable.cs
│   ├── QuantityModel.cs                 ← Internal service-layer model wrapper
│   └── QuantityMeasurementModel.csproj
│
├── QuantityMeasurementRepository/        ← Data Access Layer
│   ├── Interface/
│   │   └── IQuantityMeasurementRepository.cs
│   ├── Repository/
│   │   └── QuantityMeasurementCacheRepository.cs  ← Singleton in-memory cache
│   ├── QuantityMeasurementEntity.cs      ← Immutable operation record
│   └── QuantityMeasurementRepository.csproj
│
└── QuantityMeasurementApp.Tests/         ← Test Layer
    ├── (UC1–UC14 test files)
    ├── LayerSeperationTests.cs           ← UC15 tests (40 test cases)
    └── QuantityMeasurementApp.Tests.csproj
```

## Layer Dependency Direction
```
QuantityMeasurementApp (Controller)
        │
        ├──▶ QuantityMeasurementBusinessLayer (Service)
        │           │
        │           ├──▶ QuantityMeasurementModel (DTO + Entities)
        │           └──▶ QuantityMeasurementRepository (Cache)
        │
        ├──▶ QuantityMeasurementModel
        └──▶ QuantityMeasurementRepository
```

| Layer            | Depends On                            |
| ---------------- | ------------------------------------- |
| App (Controller) | BusinessLayer, Repository, Model      |
| BusinessLayer    | Model, Repository                     |
| Repository       | Model (QuantityDTO)                   |
| Model            | Nothing (System only)                 |
| Tests            | App, BusinessLayer, Repository, Model |

---

## Principles Applied

| Principle             | Implementation                                                |
| --------------------- | ------------------------------------------------------------- |
| DRY                   | Centralized ArithmeticOperation enum (UC13)                   |
| SRP                   | Each layer has exactly one responsibility                     |
| Open/Closed           | New categories added without modifying existing ones          |
| Interface Segregation | IMeasurable default methods; IQuantityMeasurementService      |
| Dependency Inversion  | Controller depends on IQuantityMeasurementService interface   |
| Immutability          | All operations return new objects; QuantityDTO has no setters |
| Singleton             | QuantityMeasurementCacheRepository.Instance                   |
| Factory Pattern       | Program.cs creates and injects all dependencies               |

---

# 🖥 Console UI

## Main Menu
```
+------------------------------------------+
|     Quantity Measurement App             |
+------------------------------------------+
|  1.  Length Operations                   |
|  2.  Weight Operations                   |
|  3.  Volume Operations                   |
|  4.  Temperature Operations              |
|  5.  Operation History                   |
|  6.  Exit                                |
+------------------------------------------+
```

## Category Sub-Menu (Length / Weight / Volume)
```
+------------------------------------------+
|         Length Operations                |
+------------------------------------------+
|  1. Compare                              |
|  2. Convert                              |
|  3. Add                                  |
|  4. Subtract                             |
|  5. Divide                               |
+------------------------------------------+
```

## Temperature Sub-Menu
```
+------------------------------------------+
|       Temperature Operations             |
+------------------------------------------+
|  1. Compare                              |
|  2. Convert                              |
|  3. Add    (not supported)               |
|  4. Subtract (not supported)             |
|  5. Divide   (not supported)             |
+------------------------------------------+
```

---

# 📊 Test Summary

| Test File                          | UC Coverage | Tests   |
| ---------------------------------- | ----------- | ------- |
| FeetEqualityTests.cs               | UC1         | 14      |
| InchesEqualityTests.cs             | UC2         | 10      |
| QuantityLengthTests.cs             | UC3         | 12      |
| ExtendedUnitSupportMSTests.cs      | UC4         | 17      |
| UnitConversionTests.cs             | UC5         | 12      |
| AdditionTests.cs                   | UC6         | 12      |
| AdditionWithTargetUnitTests.cs     | UC7         | 14      |
| LengthUnitRefactoringTests.cs      | UC8         | 24      |
| WeightMeasurementTests.cs          | UC9         | 26      |
| GenericQuantityTests.cs            | UC10        | 35      |
| VolumeMeasurementTest.cs           | UC11        | 50      |
| SubtractionDivisionTests.cs        | UC12        | 39      |
| CentralizedArithmeticLogicTests.cs | UC13        | 48      |
| TemperatureMeasurementTests.cs     | UC14        | 41      |
| LayerSeperationTests.cs            | UC15        | 40      |
| **Total**                          |             | **394** |

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
dotnet run --project QuantityMeasurementApp/QuantityMeasurementApp/QuantityMeasurementApp.csproj
```

---

| Metric            | Value                                  |
| ----------------- | -------------------------------------- |
| Use Cases         | 15 (UC1 to UC15)                       |
| Measurement Types | Length, Weight, Volume, Temperature    |
| Units Supported   | 11 across 4 categories                 |
| Arithmetic        | Add, Subtract, Divide                  |
| Conversion        | All categories                         |
| Equality          | All categories including cross-unit    |
| Layers            | Controller, Service, Repository, Model |
| Test Cases        | 394 (all passing)                      |
