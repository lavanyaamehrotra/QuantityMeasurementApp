# 📏 Quantity Measurement Application (UC1 – UC7)

A progressive **.NET Console Application** for performing measurement comparison, conversion, and arithmetic operations across **Length measurements**.  
This project demonstrates incremental software development using clean architecture and SOLID principles.

🚀 The system evolves from simple equality checks to a reusable and extensible quantity framework.

---
# 🎯 Overview

The **Quantity Measurement Application** is built step-by-step through structured use cases.

From **UC1 → UC7**, the application grows from a simple measurement comparison system into a flexible quantity engine supporting:

✅ Equality comparison  
✅ Generic design  
✅ Multiple measurement units  
✅ Unit conversion  
✅ Arithmetic operations  
✅ Flexible output units  

---

## ✨ Key Highlights

- 📐 Length measurement system
- 🔢 4 supported units
- 🔄 Cross-unit equality
- 🔁 Unit conversion engine
- ➕ Arithmetic operations
- 🧩 Extensible architecture
- ♻️ DRY principle implementation

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
## ✨ Feature
✔ Value-based equality
✔ Proper null handling
✔ Reflexive, symmetric & transitive comparison
✔ Foundation for future measurements

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

## ✅ UC3: Generic Quantity Class 🧩

Removes duplication using a reusable class.
```
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
* ✔ Supported Units:FEET ,INCH

## ✅ UC4: Extended Unit Support 📐

Adds additional length units.
```
public enum LengthUnit
{
    FEET,
    INCH,
    YARD,
    CENTIMETER
}
```
## ✨ Features
✔ Yard support
✔ Centimeter support
✔ Cross-unit comparison

| Unit | Conversion   |
| ---- | ------------ |
| 1 ft | 12 in        |
| 1 yd | 3 ft         |
| 1 cm | 0.0328084 ft |
1 yd = 3 ft = 36 in = 91.44 cm

## ✅ UC5: Unit Conversion 🔄

Convert quantities between units.
```
var feet = new Quantity(1.0, LengthUnit.FEET);

var inches = feet.ConvertTo(LengthUnit.INCH);
var yards = feet.ConvertTo(LengthUnit.YARD);

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

## ✅ UC6: Addition Operations ➕

Supports arithmetic addition across units.
```
var feet = new Quantity(1.0, LengthUnit.FEET);
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

## ✅ UC7: Addition with Target Unit 🎯

Allows specifying desired output unit.
```
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

## 🏗 Architecture

```text
---------------------------------
│ 🖥 Console UI                 │
---------------------------------
│ ⚙ Application Logic           │
|-------------------------------|
│ 📦 Quantity Domain            │
│   • Quantity Class            │
│   • LengthUnit Enum           │
|-------------------------------|
│ 🔧 Core                       │
---------------------------------
```

## Principle Applied
| Principle    | Implementation       |
| ------------ | -------------------- |
| DRY          | Generic Quantity     |
| SRP          | Unit separation      |
| Open/Closed  | Easy extension       |
| Immutability | New objects returned |

## 📥 Clone Repository
git clone https://github.com/lavanyaamehrotra/QuantityMeasurementApp.git
cd QuantityMeasurementApp
## 🏗 Build
dotnet build
## ▶ Run
dotnet run

## 🎮 Usage Guide
## Conversion Example
5 ft = 60 in

## Addition Example
1 ft + 12 in = 2 ft

| Metric              | Value      |
| ------------------- | --------   |
| 📘 Use Cases        | 7          |
| 📐 Measurement Type | Length     |
| 🔢 Units Supported  | 4          |
| ➕ Arithmetic       | Addition   |
| 🔄 Conversion       | ✅        |
| ⚖ Equality          | ✅        |

## Project Structure 
```text
QuantityMeasurementApp/
│
├── QuantityMeasurementApp/
│   ├── BusinessLogicLayer/
│   │   └── QuantityMeasurementService.cs
│   │
│   ├── DataAccessLayer/
│   │   └── MeasurementLogger.cs
│   │
│   ├── Entities/
│   │   ├── Feet.cs
│   │   ├── Inches.cs
│   │   ├── LengthUnit.cs
│   │   └── QuantityLength.cs
│   │
│   ├── Interfaces/
│   │   └── IQuantityMeasurementService.cs
│   │
│   ├── PresentationLayer/
│   │   ├── Menu.cs
│   │   └── Program.cs
│   │
│   └── QuantityMeasurementApp.csproj
│
└── QuantityMeasurementApp.Tests/
    ├── FeetEqualityTests.cs
    ├── InchesEqualityTests.cs
    ├── QuantityLengthTests.cs
    ├── ExtendedUnitSupportMSTests.cs
    ├── UC5_UnitConversionTests.cs
    ├── UC6_AdditionTests.cs
    └── UC7_AdditionWithTargetUnitTests.cs
