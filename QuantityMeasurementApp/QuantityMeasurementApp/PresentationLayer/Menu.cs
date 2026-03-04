using System;
using QuantityMeasurementApp.BusinessLogicLayer;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Interfaces;

namespace QuantityMeasurementApp.PresentationLayer
{
    /// <summary>
    /// Presentation Layer - handles all user interaction.
    /// ALL operations go through IQuantityMeasurementService (BLL).
    /// No entity is created or compared directly here.
    /// </summary>
    public class Menu
    {
        // Depends on interface, not concrete class (Dependency Inversion Principle).
        // Makes this class ASP.NET DI-ready and unit-testable.
        private readonly IQuantityMeasurementService service;

        /// <summary>Default constructor for console app usage.</summary>
        public Menu() : this(new QuantityMeasurementService()) { }

        /// <summary>DI constructor for ASP.NET / unit tests.</summary>
        public Menu(IQuantityMeasurementService service)
        {
            this.service = service;
        }

        public void Start()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine("|             Quantity Measurement App           |");
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine("|                                                |");
                Console.WriteLine("|   1. Feet Equality                             |");
                Console.WriteLine("|   2. Feet & Inches Equality                    |");
                Console.WriteLine("|   3. Generic Length (Generics)                 |");
                Console.WriteLine("|   4. Extended Unit Support                     |");
                Console.WriteLine("|   5. Unit Conversion                           |");
                Console.WriteLine("|   6. Addition of Two Length Units              |");
                Console.WriteLine("|   7. Addition With Target                      |");
                Console.WriteLine("|   8. Weight Measurement                        |");
                Console.WriteLine("|   9. Generic Quantity (Multi-Category)         |");
                Console.WriteLine("|  10. Volume Measurement                        |");
                Console.WriteLine("|  11. Subtraction & Division Operations         |");
                Console.WriteLine("|  12. Temperature Measurement                   |");
                Console.WriteLine("|  13. Exit                                      |");
                Console.WriteLine("+------------------------------------------------+");
                Console.Write("\nEnter your choice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":  RunUC1();  break;
                    case "2":  RunUC2();  break;
                    case "3":  RunUC3();  break;
                    case "4":  RunUC4();  break;
                    case "5":  RunUC5();  break;
                    case "6":  RunUC6();  break;
                    case "7":  RunUC7();  break;
                    case "8":  RunUC9();  break;
                    case "9":  RunUC10(); break;
                    case "10": RunUC11(); break;
                    case "11": RunUC12(); break;
                    // case "12" (UC13) removed
                    case "12": RunUC14(); break;
                    case "13":
                        running = false;
                        Console.WriteLine("\nThank you For Using Quantity Measurement App");
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        // ================= UC1 - Feet Equality =================
        private void RunUC1()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|               Feet Equality              |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                Console.Write("Enter first feet value: ");
                string? input1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input1))
                    throw new ArgumentException("Invalid input: First value cannot be empty.");
                if (!double.TryParse(input1, out double v1))
                    throw new ArgumentException("Invalid input: First value must be numeric.");

                // BLL: validate + create
                var f1 = service.CreateFeet(v1);

                Console.Write("Enter second feet value: ");
                string? input2 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input2))
                    throw new ArgumentException("Invalid input: Second value cannot be empty.");
                if (!double.TryParse(input2, out double v2))
                    throw new ArgumentException("Invalid input: Second value must be numeric.");

                // BLL: validate + create
                var f2 = service.CreateFeet(v2);

                // BLL: compare
                bool result = service.CompareFeet(f1, f2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|        Result : Equal (" + result + ")           |");
                Console.WriteLine("+------------------------------------------+");

                // BLL: analysis
                Console.WriteLine(service.GetMeasurementAnalysis(v1, v2, "ft"));
            }
            catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
            catch (OverflowException)    { Console.WriteLine("Invalid input: Number too large."); }
            catch (Exception)            { Console.WriteLine("Unexpected error occurred."); }
        }

        // ================= UC2 - Feet & Inches Equality =================
        private void RunUC2()
        {
            Console.Clear();
            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine("|         Feet & Inches Equality           |");
            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine();

            // --- Feet ---
            try
            {
                Console.WriteLine("Feet Equality Check");
                Console.Write("Enter first feet value: ");
                string? fInput1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fInput1))
                    throw new ArgumentException("Invalid input: First feet value empty.");
                if (!double.TryParse(fInput1, out double f1Value))
                    throw new ArgumentException("Invalid input: Feet must be numeric.");

                // BLL: validate + create
                var f1 = service.CreateFeet(f1Value);

                Console.Write("Enter second feet value: ");
                string? fInput2 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fInput2))
                    throw new ArgumentException("Invalid input: Second feet value empty.");
                if (!double.TryParse(fInput2, out double f2Value))
                    throw new ArgumentException("Invalid input: Feet must be numeric.");

                // BLL: validate + create
                var f2 = service.CreateFeet(f2Value);

                // BLL: compare
                bool feetResult = service.CompareFeet(f1, f2);
                Console.WriteLine($"\nFeet Equal ({feetResult})");

                // BLL: analysis
                Console.WriteLine(service.GetMeasurementAnalysis(f1Value, f2Value, "ft"));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("Feet comparison skipped."); }

            Console.WriteLine();

            // --- Inches ---
            try
            {
                Console.WriteLine("Inches Equality Check");
                Console.Write("Enter first inches value: ");
                string? iInput1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(iInput1))
                    throw new ArgumentException("Invalid input: First inches value empty.");
                if (!double.TryParse(iInput1, out double i1Value))
                    throw new ArgumentException("Invalid input: Inches must be numeric.");

                // BLL: validate + create
                var i1 = service.CreateInches(i1Value);

                Console.Write("Enter second inches value: ");
                string? iInput2 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(iInput2))
                    throw new ArgumentException("Invalid input: Second inches value empty.");
                if (!double.TryParse(iInput2, out double i2Value))
                    throw new ArgumentException("Invalid input: Inches must be numeric.");

                // BLL: validate + create
                var i2 = service.CreateInches(i2Value);

                // BLL: compare
                bool inchResult = service.CompareInches(i1, i2);
                Console.WriteLine($"\nInches Equal ({inchResult})");

                // BLL: analysis
                Console.WriteLine(service.GetMeasurementAnalysis(i1Value, i2Value, "in"));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("Inches comparison skipped."); }
        }

        // ================= UC3 - Generic Length Equality =================
        private void RunUC3()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|          Generic Quantity Equality       |");
                Console.WriteLine("+------------------------------------------+");

                Console.Write("Enter first value: ");
                double v1 = ParseDouble(Console.ReadLine());
                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit u1 = ParseUnit(Console.ReadLine());

                Console.Write("Enter second value: ");
                double v2 = ParseDouble(Console.ReadLine());
                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit u2 = ParseUnit(Console.ReadLine());

                // BLL: create
                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                // BLL: compare
                bool result = service.CompareQuantity(q1, q2);
                Console.WriteLine($"\nEqual ({result})");

                // BLL: analysis (using base unit feet for difference)
                Console.WriteLine(service.GetMeasurementAnalysis(
                    v1 * u1.ToFeetFactor(), v2 * u2.ToFeetFactor(), "ft"));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        // ================= UC4 - Extended Unit Support =================
        private void RunUC4()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|           Extended Unit Support          |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                Console.Write("Enter first value: ");
                double value1 = ParseDouble(Console.ReadLine());
                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit unit1 = ParseUnit(Console.ReadLine());

                Console.Write("\nEnter second value: ");
                double value2 = ParseDouble(Console.ReadLine());
                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit unit2 = ParseUnit(Console.ReadLine());

                // BLL: create (validates value + unit)
                var q1 = service.CreateQuantity(value1, unit1);
                var q2 = service.CreateQuantity(value2, unit2);

                // BLL: compare
                bool result = service.CompareQuantity(q1, q2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine($"|        Result : Equal ({result})          |");
                Console.WriteLine("+------------------------------------------+");

                // BLL: analysis (convert both to feet first)
                Console.WriteLine(service.GetMeasurementAnalysis(
                    value1 * unit1.ToFeetFactor(), value2 * unit2.ToFeetFactor(), "ft"));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        // ================= UC5 - Unit To Unit Conversion =================
        private void RunUC5()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|          Unit To Unit Conversion         |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                Console.Write("Enter length value to convert: ");
                string? valueInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valueInput))
                    throw new ArgumentException("Invalid input: Value cannot be empty.");
                if (!double.TryParse(valueInput, out double value))
                    throw new ArgumentException("Invalid input: Value must be numeric.");

                Console.Write("Enter source unit (feet/inch/yard/cm): ");
                LengthUnit fromUnit = ParseUnit(Console.ReadLine());

                Console.Write("Enter target unit (feet/inch/yard/cm): ");
                LengthUnit toUnit = ParseUnit(Console.ReadLine());

                // BLL: convert (numeric)
                double result = service.ConvertLength(value, fromUnit, toUnit);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine($"|  {value} {fromUnit} = {result} {toUnit}  |");
                Console.WriteLine("+------------------------------------------+");

                // BLL: instance conversion (overload)
                var quantity    = service.CreateQuantity(value, fromUnit);
                var converted   = service.ConvertLength(quantity, toUnit);
                Console.WriteLine($"\nInstance conversion: {quantity} -> {converted}");
                Console.WriteLine($"Physical length in base unit: {service.ConvertLength(value, fromUnit, LengthUnit.FEET)} ft");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|  Error: " + ex.Message + " |");
                Console.WriteLine("+------------------------------------------+");
            }
            catch (Exception ex) { Console.WriteLine("\nError: " + ex.Message); }
        }

        // ================= UC6 - Addition of Two Length Units =================
        private void RunUC6()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|          Addition of Two Length Units    |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("Result is in unit of first operand.");
                Console.WriteLine();

                Console.Write("Enter first value: ");
                double v1 = ParseDouble(Console.ReadLine());
                Console.Write("Enter first unit (feet/inch/yard/cm): ");
                LengthUnit u1 = ParseUnit(Console.ReadLine());

                Console.Write("\nEnter second value: ");
                double v2 = ParseDouble(Console.ReadLine());
                Console.Write("Enter second unit (feet/inch/yard/cm): ");
                LengthUnit u2 = ParseUnit(Console.ReadLine());

                // BLL: create
                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                // BLL: add
                var result = service.AddLength(q1, q2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|           Addition Result                |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine($"|  {q1}  +  {q2}  =  {result}  |");
                Console.WriteLine("+------------------------------------------+");

                // BLL: show equivalent conversions
                Console.WriteLine($"\n  Physical length in base unit: {service.ConvertLength(result, LengthUnit.FEET)} ft");
                Console.WriteLine($"  Equivalent: {service.ConvertLength(result, LengthUnit.INCH)} | " +
                                  $"{service.ConvertLength(result, LengthUnit.YARD)} | " +
                                  $"{service.ConvertLength(result, LengthUnit.CENTIMETER)}");
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("\nError: " + ex.Message); }
        }

        // ================= UC7 - Addition With Target Unit =================
        private void RunUC7()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|           Addition With Target           |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("Add two lengths. Result in your chosen target unit.");
                Console.WriteLine();

                Console.Write("Enter first value: ");
                double v1 = ParseDouble(Console.ReadLine());
                Console.Write("Enter first unit (feet/inch/yard/cm): ");
                LengthUnit u1 = ParseUnit(Console.ReadLine());

                Console.Write("\nEnter second value: ");
                double v2 = ParseDouble(Console.ReadLine());
                Console.Write("Enter second unit (feet/inch/yard/cm): ");
                LengthUnit u2 = ParseUnit(Console.ReadLine());

                Console.Write("\nEnter target unit for result (feet/inch/yard/cm): ");
                LengthUnit targetUnit = ParseUnit(Console.ReadLine());

                // BLL: create
                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                // BLL: add with target
                var result = service.AddLength(q1, q2, targetUnit);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|             Addition Result              |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine($"|  {q1}  +  {q2}  =  {result}  |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine($"\n  Physical length in base unit: {service.ConvertLength(result, LengthUnit.FEET)} ft");
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("\nError: " + ex.Message); }
        }

        // ================= UC9 - Weight Measurement =================
        private void RunUC9()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|         Weight Measurement (kg/g/lb)          |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("1. Equality comparison");
                Console.WriteLine("2. Unit conversion");
                Console.WriteLine("3. Addition (implicit target unit)");
                Console.WriteLine("4. Addition (explicit target unit)");
                Console.WriteLine();
                Console.Write("Enter choice (1-4): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunWeightEquality();          break;
                    case "2": RunWeightConversion();        break;
                    case "3": RunWeightAdditionImplicit();  break;
                    case "4": RunWeightAdditionExplicit();  break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-4."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunWeightEquality()
        {
            Console.Write("\nEnter first weight value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (kg/g/lb): ");
            WeightUnit u1 = ParseWeightUnit(Console.ReadLine());

            Console.Write("\nEnter second weight value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (kg/g/lb): ");
            WeightUnit u2 = ParseWeightUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateWeight(v1, u1);
            var q2 = service.CreateWeight(v2, u2);

            // BLL: compare
            bool equal = service.CompareWeight(q1, q2);
            Console.WriteLine($"\nEquality result: {q1} == {q2} ? {equal}");
            // Route to BLL via service.ConvertWeight — never call entity methods directly in Presentation Layer
            double q1InKg = service.ConvertWeight(v1, u1, WeightUnit.KILOGRAM);
            double q2InKg = service.ConvertWeight(v2, u2, WeightUnit.KILOGRAM);
            Console.WriteLine(service.GetMeasurementAnalysis(q1InKg, q2InKg, "kg"));
        }

        private void RunWeightConversion()
        {
            Console.Write("\nEnter weight value: ");
            double value = ParseDouble(Console.ReadLine());
            Console.Write("Enter source unit (kg/g/lb): ");
            WeightUnit fromUnit = ParseWeightUnit(Console.ReadLine());
            Console.Write("Enter target unit (kg/g/lb): ");
            WeightUnit toUnit = ParseWeightUnit(Console.ReadLine());

            // BLL: create
            var q = service.CreateWeight(value, fromUnit);

            // BLL: convert
            var result = service.ConvertWeight(q, toUnit);
            Console.WriteLine($"\nConverted: {q} -> {result}");
        }

        private void RunWeightAdditionImplicit()
        {
            Console.Write("\nEnter first weight value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (kg/g/lb): ");
            WeightUnit u1 = ParseWeightUnit(Console.ReadLine());

            Console.Write("\nEnter second weight value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (kg/g/lb): ");
            WeightUnit u2 = ParseWeightUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateWeight(v1, u1);
            var q2 = service.CreateWeight(v2, u2);

            // BLL: add (result in unit of first operand)
            var result = service.AddWeight(q1, q2);
            Console.WriteLine($"\nSum (implicit target = first unit): {q1} + {q2} = {result}");
        }

        private void RunWeightAdditionExplicit()
        {
            Console.Write("\nEnter first weight value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (kg/g/lb): ");
            WeightUnit u1 = ParseWeightUnit(Console.ReadLine());

            Console.Write("\nEnter second weight value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (kg/g/lb): ");
            WeightUnit u2 = ParseWeightUnit(Console.ReadLine());

            Console.Write("\nEnter target unit for result (kg/g/lb): ");
            WeightUnit targetUnit = ParseWeightUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateWeight(v1, u1);
            var q2 = service.CreateWeight(v2, u2);

            // BLL: add with explicit target
            var result = service.AddWeight(q1, q2, targetUnit);
            Console.WriteLine($"\nSum (explicit target): {q1} + {q2} = {result}");
        }

        // ================= UC10 - Generic Quantity with IMeasurable =================
        private void RunUC10()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|   UC10: Generic Quantity (Multi-Category)     |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("  1. Length Operations");
                Console.WriteLine("  2. Weight Operations");
                Console.WriteLine("  3. Cross-Category Prevention");
                Console.WriteLine("  4. Generic Demonstration Methods");
                Console.WriteLine();
                Console.Write("Enter choice (1-4): ");
                string? sub = Console.ReadLine();

                switch (sub)
                {
                    case "1": RunUC10_LengthOperations();       break;
                    case "2": RunUC10_WeightOperations();       break;
                    case "3": RunUC10_CrossCategoryPrevention(); break;
                    case "4": RunUC10_GenericDemonstration();   break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-4."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunUC10_LengthOperations()
        {
            Console.Clear();
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine("|         Length Operations (UC10)              |");
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine("\n  1. Equality\n  2. Conversion\n  3. Addition\n");
            Console.Write("Enter choice (1-3): ");
            string? sub = Console.ReadLine();

            switch (sub)
            {
                case "1":
                    Console.Write("\nEnter first length value: ");
                    double v1 = ReadDouble();
                    LengthUnitM u1 = ParseLengthUnitM();
                    Console.Write("Enter second length value: ");
                    double v2 = ReadDouble();
                    LengthUnitM u2 = ParseLengthUnitM();
                    // BLL: create + compare
                    var q1 = service.CreateGenericQuantity(v1, u1);
                    var q2 = service.CreateGenericQuantity(v2, u2);
                    bool eq = service.CompareGenericQuantity(q1, q2);
                    Console.WriteLine($"\n  {q1} == {q2} -> {eq}");
                    break;

                case "2":
                    Console.Write("\nEnter length value: ");
                    double val = ReadDouble();
                    LengthUnitM fromU = ParseLengthUnitM();
                    Console.Write("Enter target unit (feet/inches/yards/cm): ");
                    LengthUnitM toU = ReadLengthUnitM();
                    // BLL: create + convert
                    var src = service.CreateGenericQuantity(val, fromU);
                    var converted = service.ConvertGenericQuantity(src, toU);
                    Console.WriteLine($"\n  {src} -> {converted}");
                    break;

                case "3":
                    Console.Write("\nEnter first length value: ");
                    double a1 = ReadDouble();
                    LengthUnitM au1 = ParseLengthUnitM();
                    Console.Write("Enter second length value: ");
                    double a2 = ReadDouble();
                    LengthUnitM au2 = ParseLengthUnitM();
                    Console.Write("Enter target unit for result (feet/inches/yards/cm): ");
                    LengthUnitM target = ReadLengthUnitM();
                    // BLL: create + add
                    var qa1 = service.CreateGenericQuantity(a1, au1);
                    var qa2 = service.CreateGenericQuantity(a2, au2);
                    var sum = service.AddGenericQuantity(qa1, qa2, target);
                    Console.WriteLine($"\n  {qa1} + {qa2} = {sum}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunUC10_WeightOperations()
        {
            Console.Clear();
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine("|         Weight Operations (UC10)              |");
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine("\n  1. Equality\n  2. Conversion\n  3. Addition\n");
            Console.Write("Enter choice (1-3): ");
            string? sub = Console.ReadLine();

            switch (sub)
            {
                case "1":
                    Console.Write("\nEnter first weight value: ");
                    double v1 = ReadDouble();
                    WeightUnitM u1 = ParseWeightUnitM();
                    Console.Write("Enter second weight value: ");
                    double v2 = ReadDouble();
                    WeightUnitM u2 = ParseWeightUnitM();
                    // BLL: create + compare
                    var w1 = service.CreateGenericQuantity(v1, u1);
                    var w2 = service.CreateGenericQuantity(v2, u2);
                    bool eq = service.CompareGenericQuantity(w1, w2);
                    Console.WriteLine($"\n  {w1} == {w2} ? {eq}");
                    break;

                case "2":
                    Console.Write("\nEnter weight value: ");
                    double val = ReadDouble();
                    WeightUnitM fromU = ParseWeightUnitM();
                    Console.Write("Enter target unit (kg/g/lb): ");
                    WeightUnitM toU = ReadWeightUnitM();
                    // BLL: create + convert
                    var src = service.CreateGenericQuantity(val, fromU);
                    var converted = service.ConvertGenericQuantity(src, toU);
                    Console.WriteLine($"\n  {src} -> {converted}");
                    break;

                case "3":
                    Console.Write("\nEnter first weight value: ");
                    double a1 = ReadDouble();
                    WeightUnitM au1 = ParseWeightUnitM();
                    Console.Write("Enter second weight value: ");
                    double a2 = ReadDouble();
                    WeightUnitM au2 = ParseWeightUnitM();
                    Console.Write("Enter target unit for result (kg/g/lb): ");
                    WeightUnitM target = ReadWeightUnitM();
                    // BLL: create + add
                    var wa1 = service.CreateGenericQuantity(a1, au1);
                    var wa2 = service.CreateGenericQuantity(a2, au2);
                    var wsum = service.AddGenericQuantity(wa1, wa2, target);
                    Console.WriteLine($"\n  {wa1} + {wa2} = {wsum}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunUC10_CrossCategoryPrevention()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|     UC10: Cross-Category Prevention           |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("\n  1. Compare Length vs Weight\n  2. Compare Length vs Length\n  3. Compare Weight vs Weight\n");
                Console.Write("Enter choice (1-3): ");
                string? sub = Console.ReadLine();

                switch (sub)
                {
                    case "1":
                        Console.Write("\n  Length value: ");
                        double lv = ReadDouble();
                        LengthUnitM lu = ParseLengthUnitM();
                        Console.Write("  Weight value: ");
                        double wv = ReadDouble();
                        WeightUnitM wu = ParseWeightUnitM();
                        // BLL: create both
                        var lengthQ = service.CreateGenericQuantity(lv, lu);
                        var weightQ = service.CreateGenericQuantity(wv, wu);
                        Console.WriteLine($"\n  Length: {lengthQ}  |  Weight: {weightQ}");
                        // Cross-category equals — must be false. Routed through BLL.
                        bool crossResult = service.CheckCrossCategory(lengthQ, weightQ);
                        Console.WriteLine($"  Result: {crossResult}  (false = cross-category correctly blocked)");
                        break;

                    case "2":
                        Console.Write("\n  First length value: ");
                        double lv1 = ReadDouble();
                        LengthUnitM lu1 = ParseLengthUnitM();
                        Console.Write("  Second length value: ");
                        double lv2 = ReadDouble();
                        LengthUnitM lu2 = ParseLengthUnitM();
                        // BLL: create + compare
                        var lq1 = service.CreateGenericQuantity(lv1, lu1);
                        var lq2 = service.CreateGenericQuantity(lv2, lu2);
                        bool lResult = service.CompareGenericQuantity(lq1, lq2);
                        Console.WriteLine($"\n  {lq1} == {lq2} ? {lResult}");
                        break;

                    case "3":
                        Console.Write("\n  First weight value: ");
                        double wv1 = ReadDouble();
                        WeightUnitM wu1 = ParseWeightUnitM();
                        Console.Write("  Second weight value: ");
                        double wv2 = ReadDouble();
                        WeightUnitM wu2 = ParseWeightUnitM();
                        // BLL: create + compare
                        var wq1 = service.CreateGenericQuantity(wv1, wu1);
                        var wq2 = service.CreateGenericQuantity(wv2, wu2);
                        bool wResult = service.CompareGenericQuantity(wq1, wq2);
                        Console.WriteLine($"\n  {wq1} == {wq2} ? {wResult}");
                        break;

                    default: Console.WriteLine("\nInvalid choice. Enter 1-3."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunUC10_GenericDemonstration()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|     UC10: Generic Demonstration Methods       |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("\n  1. Equality\n  2. Conversion\n  3. Addition\n");
                Console.Write("Enter choice (1-3): ");
                string? sub = Console.ReadLine();

                switch (sub)
                {
                    case "1":
                        Console.WriteLine("\n  Choose category: 1. Length  2. Weight");
                        Console.Write("Enter choice: ");
                        string? eqCat = Console.ReadLine();
                        if (eqCat == "1")
                        {
                            Console.Write("  First length value: ");
                            double v1 = ReadDouble(); LengthUnitM u1 = ParseLengthUnitM();
                            Console.Write("  Second length value: ");
                            double v2 = ReadDouble(); LengthUnitM u2 = ParseLengthUnitM();
                            // BLL: create
                            var q1 = service.CreateGenericQuantity(v1, u1);
                            var q2 = service.CreateGenericQuantity(v2, u2);
                            DemonstrateEquality(q1, q2);
                        }
                        else if (eqCat == "2")
                        {
                            Console.Write("  First weight value: ");
                            double v1 = ReadDouble(); WeightUnitM u1 = ParseWeightUnitM();
                            Console.Write("  Second weight value: ");
                            double v2 = ReadDouble(); WeightUnitM u2 = ParseWeightUnitM();
                            // BLL: create
                            var q1 = service.CreateGenericQuantity(v1, u1);
                            var q2 = service.CreateGenericQuantity(v2, u2);
                            DemonstrateEquality(q1, q2);
                        }
                        else Console.WriteLine("\nInvalid choice.");
                        break;

                    case "2":
                        Console.WriteLine("\n  Choose category: 1. Length  2. Weight");
                        Console.Write("Enter choice: ");
                        string? convCat = Console.ReadLine();
                        if (convCat == "1")
                        {
                            Console.Write("  Length value: ");
                            double v = ReadDouble(); LengthUnitM u = ParseLengthUnitM();
                            Console.Write("  Target unit (feet/inches/yards/cm): ");
                            LengthUnitM target = ReadLengthUnitM();
                            // BLL: create + convert
                            var q = service.CreateGenericQuantity(v, u);
                            DemonstrateConversion(q, target);
                        }
                        else if (convCat == "2")
                        {
                            Console.Write("  Weight value: ");
                            double v = ReadDouble(); WeightUnitM u = ParseWeightUnitM();
                            Console.Write("  Target unit (kg/g/lb): ");
                            WeightUnitM target = ReadWeightUnitM();
                            // BLL: create + convert
                            var q = service.CreateGenericQuantity(v, u);
                            DemonstrateConversion(q, target);
                        }
                        else Console.WriteLine("\nInvalid choice.");
                        break;

                    case "3":
                        Console.WriteLine("\n  Choose category: 1. Length  2. Weight");
                        Console.Write("Enter choice: ");
                        string? addCat = Console.ReadLine();
                        if (addCat == "1")
                        {
                            Console.Write("  First length value: ");
                            double v1 = ReadDouble(); LengthUnitM u1 = ParseLengthUnitM();
                            Console.Write("  Second length value: ");
                            double v2 = ReadDouble(); LengthUnitM u2 = ParseLengthUnitM();
                            Console.Write("  Target unit (feet/inches/yards/cm): ");
                            LengthUnitM target = ReadLengthUnitM();
                            // BLL: create + add
                            var q1 = service.CreateGenericQuantity(v1, u1);
                            var q2 = service.CreateGenericQuantity(v2, u2);
                            DemonstrateAddition(q1, q2, target);
                        }
                        else if (addCat == "2")
                        {
                            Console.Write("  First weight value: ");
                            double v1 = ReadDouble(); WeightUnitM u1 = ParseWeightUnitM();
                            Console.Write("  Second weight value: ");
                            double v2 = ReadDouble(); WeightUnitM u2 = ParseWeightUnitM();
                            Console.Write("  Target unit (kg/g/lb): ");
                            WeightUnitM target = ReadWeightUnitM();
                            // BLL: create + add
                            var q1 = service.CreateGenericQuantity(v1, u1);
                            var q2 = service.CreateGenericQuantity(v2, u2);
                            DemonstrateAddition(q1, q2, target);
                        }
                        else Console.WriteLine("\nInvalid choice.");
                        break;

                    default: Console.WriteLine("\nInvalid choice. Enter 1-3."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
        }

        // ================= UC11 - Volume Measurement =================
        private void RunUC11()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|         Volume Measurement (L/mL/gal/ft³)     |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("  1. Equality comparison");
                Console.WriteLine("  2. Unit conversion");
                Console.WriteLine("  3. Addition (implicit target unit)");
                Console.WriteLine("  4. Addition (explicit target unit)");
                Console.WriteLine();
                Console.Write("Enter choice (1-4): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunVolumeEquality();          break;
                    case "2": RunVolumeConversion();        break;
                    case "3": RunVolumeAdditionImplicit();  break;
                    case "4": RunVolumeAdditionExplicit();  break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-4."); break;
                }
            }
            catch (ArgumentException ex) { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)         { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunVolumeEquality()
        {
            Console.Write("\nEnter first volume value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u1 = ParseVolumeUnit(Console.ReadLine());

            Console.Write("\nEnter second volume value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u2 = ParseVolumeUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateVolume(v1, u1);
            var q2 = service.CreateVolume(v2, u2);

            // BLL: compare
            bool equal = service.CompareVolume(q1, q2);
            Console.WriteLine($"\nEquality result: {q1} == {q2} ? {equal}");

            // BLL: analysis (convert both to litres via service)
            double q1InL = service.ConvertVolume(v1, u1, VolumeUnit.LITRE);
            double q2InL = service.ConvertVolume(v2, u2, VolumeUnit.LITRE);
            Console.WriteLine(service.GetMeasurementAnalysis(q1InL, q2InL, "L"));
        }

        private void RunVolumeConversion()
        {
            Console.Write("\nEnter volume value: ");
            double value = ParseDouble(Console.ReadLine());
            Console.Write("Enter source unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit fromUnit = ParseVolumeUnit(Console.ReadLine());
            Console.Write("Enter target unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit toUnit = ParseVolumeUnit(Console.ReadLine());

            // BLL: create
            var q = service.CreateVolume(value, fromUnit);

            // BLL: convert (instance overload)
            var result = service.ConvertVolume(q, toUnit);
            Console.WriteLine($"\nConverted: {q} -> {result}");

            // BLL: show base unit for reference
            double inLitres = service.ConvertVolume(value, fromUnit, VolumeUnit.LITRE);
            Console.WriteLine($"Physical volume in base unit: {inLitres} LITRE");
        }

        private void RunVolumeAdditionImplicit()
        {
            Console.Write("\nEnter first volume value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u1 = ParseVolumeUnit(Console.ReadLine());

            Console.Write("\nEnter second volume value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u2 = ParseVolumeUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateVolume(v1, u1);
            var q2 = service.CreateVolume(v2, u2);

            // BLL: add (result in unit of first operand)
            var result = service.AddVolume(q1, q2);
            Console.WriteLine($"\nSum (target = first unit): {q1} + {q2} = {result}");
        }

        private void RunVolumeAdditionExplicit()
        {
            Console.Write("\nEnter first volume value: ");
            double v1 = ParseDouble(Console.ReadLine());
            Console.Write("Enter first unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u1 = ParseVolumeUnit(Console.ReadLine());

            Console.Write("\nEnter second volume value: ");
            double v2 = ParseDouble(Console.ReadLine());
            Console.Write("Enter second unit (litre/ml/gallon/cubicfeet): ");
            VolumeUnit u2 = ParseVolumeUnit(Console.ReadLine());

            Console.Write("\nEnter target unit for result (litre/ml/gallon/cubicfeet): ");
            VolumeUnit targetUnit = ParseVolumeUnit(Console.ReadLine());

            // BLL: create
            var q1 = service.CreateVolume(v1, u1);
            var q2 = service.CreateVolume(v2, u2);

            // BLL: add with explicit target
            var result = service.AddVolume(q1, q2, targetUnit);
            Console.WriteLine($"\nSum (explicit target): {q1} + {q2} = {result}");
        }

        // ================= UC12 - Subtraction & Division Operations =================
        private void RunUC12()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine("|    UC12: Subtraction & Division Operations    |");
                Console.WriteLine("+-----------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("  1. Subtraction (implicit target unit)");
                Console.WriteLine("  2. Subtraction (explicit target unit)");
                Console.WriteLine("  3. Division (dimensionless ratio)");
                Console.WriteLine();
                Console.Write("Enter choice (1-3): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunSubtractionImplicit();  break;
                    case "2": RunSubtractionExplicit();  break;
                    case "3": RunDivision();             break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-3."); break;
                }
            }
            catch (ArgumentException ex)  { Console.WriteLine("\nError: " + ex.Message); }
            catch (ArithmeticException ex) { Console.WriteLine("\nArithmetic Error: " + ex.Message); }
            catch (Exception ex)           { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunSubtractionImplicit()
        {
            Console.WriteLine("\n  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  First length value:  "); double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Second length value: "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    var lResult = service.SubtractGenericQuantity(lq1, lq2);
                    Console.WriteLine($"\n  {lq1} - {lq2} = {lResult}");
                    break;

                case "2":
                    Console.Write("\n  First weight value:  "); double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Second weight value: "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    var wResult = service.SubtractGenericQuantity(wq1, wq2);
                    Console.WriteLine($"\n  {wq1} - {wq2} = {wResult}");
                    break;

                case "3":
                    Console.Write("\n  First volume value:  "); double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Second volume value: "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    var vResult = service.SubtractGenericQuantity(vq1, vq2);
                    Console.WriteLine($"\n  {vq1} - {vq2} = {vResult}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunSubtractionExplicit()
        {
            Console.WriteLine("\n  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  First length value:  "); double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Second length value: "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    Console.Write("  Target unit (feet/inches/yards/cm): ");
                    LengthUnitM lTarget = ReadLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    var lResult = service.SubtractGenericQuantity(lq1, lq2, lTarget);
                    Console.WriteLine($"\n  {lq1} - {lq2} = {lResult}");
                    break;

                case "2":
                    Console.Write("\n  First weight value:  "); double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Second weight value: "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    Console.Write("  Target unit (kg/g/lb): ");
                    WeightUnitM wTarget = ReadWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    var wResult = service.SubtractGenericQuantity(wq1, wq2, wTarget);
                    Console.WriteLine($"\n  {wq1} - {wq2} = {wResult}");
                    break;

                case "3":
                    Console.Write("\n  First volume value:  "); double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Second volume value: "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    Console.Write("  Target unit (litre/ml/gallon): ");
                    VolumeUnitM vTarget = ReadVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    var vResult = service.SubtractGenericQuantity(vq1, vq2, vTarget);
                    Console.WriteLine($"\n  {vq1} - {vq2} = {vResult}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunDivision()
        {
            Console.WriteLine("\n  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  Dividend length value: "); double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Divisor length value:  "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    double lRatio = service.DivideGenericQuantity(lq1, lq2);
                    Console.WriteLine($"\n  {lq1} ÷ {lq2} = {lRatio} (dimensionless ratio)");
                    break;

                case "2":
                    Console.Write("\n  Dividend weight value: "); double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Divisor weight value:  "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    double wRatio = service.DivideGenericQuantity(wq1, wq2);
                    Console.WriteLine($"\n  {wq1} ÷ {wq2} = {wRatio} (dimensionless ratio)");
                    break;

                case "3":
                    Console.Write("\n  Dividend volume value: "); double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Divisor volume value:  "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    double vRatio = service.DivideGenericQuantity(vq1, vq2);
                    Console.WriteLine($"\n  {vq1} ÷ {vq2} = {vRatio} (dimensionless ratio)");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        // ================= UC13 - Centralized Arithmetic Logic (DRY) =================
        private void RunUC13()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+-------------------------------------------------------+");
                Console.WriteLine("|  UC13: Centralized Arithmetic Logic - DRY Refactoring |");
                Console.WriteLine("+-------------------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("  Internal refactoring: ArithmeticOperation enum +");
                Console.WriteLine("  ValidateArithmeticOperands + PerformBaseArithmetic.");
                Console.WriteLine("  Public API unchanged. All UC12 behaviors preserved.");
                Console.WriteLine();
                Console.WriteLine("  1. Addition Operations (via centralized helper)");
                Console.WriteLine("  2. Subtraction Operations (via centralized helper)");
                Console.WriteLine("  3. Division Operations (via centralized helper)");
                Console.WriteLine("  4. Demonstrate DRY - All Operations on All Categories");
                Console.WriteLine();
                Console.Write("Enter choice (1-4): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunUC13_AdditionDemo();        break;
                    case "2": RunUC13_SubtractionDemo();     break;
                    case "3": RunUC13_DivisionDemo();        break;
                    case "4": RunUC13_AllCategoriesDemo();   break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-4."); break;
                }
            }
            catch (ArgumentException ex)  { Console.WriteLine("\nError: " + ex.Message); }
            catch (ArithmeticException ex) { Console.WriteLine("\nArithmetic Error: " + ex.Message); }
            catch (Exception ex)           { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunUC13_AdditionDemo()
        {
            Console.WriteLine("\n--- UC13: Addition via Centralized Helper ---");
            Console.WriteLine("  Internal: Add() -> ValidateArithmeticOperands() -> PerformBaseArithmetic(ADD)");
            Console.WriteLine();

            Console.WriteLine("  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  First length value: ");  double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Second length value: "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    var lImplicit = service.AddGenericQuantity(lq1, lq2);
                    Console.WriteLine($"\n  Implicit target: {lq1} + {lq2} = {lImplicit}");
                    Console.Write("  Target unit for explicit result (feet/inches/yards/cm): ");
                    LengthUnitM lTarget = ReadLengthUnitM();
                    var lExplicit = service.AddGenericQuantity(lq1, lq2, lTarget);
                    Console.WriteLine($"  Explicit target: {lq1} + {lq2} = {lExplicit}");
                    break;

                case "2":
                    Console.Write("\n  First weight value: ");  double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Second weight value: "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    var wImplicit = service.AddGenericQuantity(wq1, wq2);
                    Console.WriteLine($"\n  Implicit target: {wq1} + {wq2} = {wImplicit}");
                    Console.Write("  Target unit for explicit result (kg/g/lb): ");
                    WeightUnitM wTarget = ReadWeightUnitM();
                    var wExplicit = service.AddGenericQuantity(wq1, wq2, wTarget);
                    Console.WriteLine($"  Explicit target: {wq1} + {wq2} = {wExplicit}");
                    break;

                case "3":
                    Console.Write("\n  First volume value: ");  double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Second volume value: "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    var vImplicit = service.AddGenericQuantity(vq1, vq2);
                    Console.WriteLine($"\n  Implicit target: {vq1} + {vq2} = {vImplicit}");
                    Console.Write("  Target unit for explicit result (litre/ml/gallon): ");
                    VolumeUnitM vTarget = ReadVolumeUnitM();
                    var vExplicit = service.AddGenericQuantity(vq1, vq2, vTarget);
                    Console.WriteLine($"  Explicit target: {vq1} + {vq2} = {vExplicit}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunUC13_SubtractionDemo()
        {
            Console.WriteLine("\n--- UC13: Subtraction via Centralized Helper ---");
            Console.WriteLine("  Internal: Subtract() -> ValidateArithmeticOperands() -> PerformBaseArithmetic(SUBTRACT)");
            Console.WriteLine();

            Console.WriteLine("  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  First length value: ");  double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Second length value: "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    var lImplicit = service.SubtractGenericQuantity(lq1, lq2);
                    Console.WriteLine($"\n  Implicit target: {lq1} - {lq2} = {lImplicit}");
                    Console.Write("  Target unit for explicit result (feet/inches/yards/cm): ");
                    LengthUnitM lTarget = ReadLengthUnitM();
                    var lExplicit = service.SubtractGenericQuantity(lq1, lq2, lTarget);
                    Console.WriteLine($"  Explicit target: {lq1} - {lq2} = {lExplicit}");
                    break;

                case "2":
                    Console.Write("\n  First weight value: ");  double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Second weight value: "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    var wResult = service.SubtractGenericQuantity(wq1, wq2);
                    Console.WriteLine($"\n  {wq1} - {wq2} = {wResult}");
                    break;

                case "3":
                    Console.Write("\n  First volume value: ");  double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Second volume value: "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    var vResult = service.SubtractGenericQuantity(vq1, vq2);
                    Console.WriteLine($"\n  {vq1} - {vq2} = {vResult}");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunUC13_DivisionDemo()
        {
            Console.WriteLine("\n--- UC13: Division via Centralized Helper ---");
            Console.WriteLine("  Internal: Divide() -> ValidateArithmeticOperands() -> PerformBaseArithmetic(DIVIDE)");
            Console.WriteLine("  Returns dimensionless scalar (double). No rounding applied.");
            Console.WriteLine();

            Console.WriteLine("  Category: 1. Length  2. Weight  3. Volume");
            Console.Write("Enter choice: ");
            string? cat = Console.ReadLine();

            switch (cat)
            {
                case "1":
                    Console.Write("\n  Dividend length value: "); double lv1 = ReadDouble();
                    LengthUnitM lu1 = ParseLengthUnitM();
                    Console.Write("  Divisor length value:  "); double lv2 = ReadDouble();
                    LengthUnitM lu2 = ParseLengthUnitM();
                    var lq1 = service.CreateGenericQuantity(lv1, lu1);
                    var lq2 = service.CreateGenericQuantity(lv2, lu2);
                    double lRatio = service.DivideGenericQuantity(lq1, lq2);
                    Console.WriteLine($"\n  {lq1} ÷ {lq2} = {lRatio} (dimensionless ratio)");
                    break;

                case "2":
                    Console.Write("\n  Dividend weight value: "); double wv1 = ReadDouble();
                    WeightUnitM wu1 = ParseWeightUnitM();
                    Console.Write("  Divisor weight value:  "); double wv2 = ReadDouble();
                    WeightUnitM wu2 = ParseWeightUnitM();
                    var wq1 = service.CreateGenericQuantity(wv1, wu1);
                    var wq2 = service.CreateGenericQuantity(wv2, wu2);
                    double wRatio = service.DivideGenericQuantity(wq1, wq2);
                    Console.WriteLine($"\n  {wq1} ÷ {wq2} = {wRatio} (dimensionless ratio)");
                    break;

                case "3":
                    Console.Write("\n  Dividend volume value: "); double vv1 = ReadDouble();
                    VolumeUnitM vu1 = ParseVolumeUnitM();
                    Console.Write("  Divisor volume value:  "); double vv2 = ReadDouble();
                    VolumeUnitM vu2 = ParseVolumeUnitM();
                    var vq1 = service.CreateGenericQuantity(vv1, vu1);
                    var vq2 = service.CreateGenericQuantity(vv2, vu2);
                    double vRatio = service.DivideGenericQuantity(vq1, vq2);
                    Console.WriteLine($"\n  {vq1} ÷ {vq2} = {vRatio} (dimensionless ratio)");
                    break;

                default: Console.WriteLine("\nInvalid choice."); break;
            }
        }

        private void RunUC13_AllCategoriesDemo()
        {
            Console.WriteLine("\n--- UC13: DRY Demonstration - All Operations Across All Categories ---");
            Console.WriteLine("  All operations share ValidateArithmeticOperands + PerformBaseArithmetic.");
            Console.WriteLine();

            // Length demo
            var feet1  = service.CreateGenericQuantity(1.0,  LengthUnitM.FEET);
            var inch12 = service.CreateGenericQuantity(12.0, LengthUnitM.INCHES);
            var feet10 = service.CreateGenericQuantity(10.0, LengthUnitM.FEET);
            var inch6  = service.CreateGenericQuantity(6.0,  LengthUnitM.INCHES);
            var feet2  = service.CreateGenericQuantity(2.0,  LengthUnitM.FEET);

            Console.WriteLine("  [Length]");
            Console.WriteLine($"    Add:      {feet1}  + {inch12} = {service.AddGenericQuantity(feet1, inch12)}  (expected: Quantity(2, FEET))");
            Console.WriteLine($"    Subtract: {feet10} - {inch6}  = {service.SubtractGenericQuantity(feet10, inch6)}  (expected: Quantity(9.5, FEET))");
            Console.WriteLine($"    Divide:   {feet10} ÷ {feet2}  = {service.DivideGenericQuantity(feet10, feet2)}  (expected: 5.0)");

            // Weight demo
            var kg10   = service.CreateGenericQuantity(10.0,   WeightUnitM.KILOGRAM);
            var g5000  = service.CreateGenericQuantity(5000.0, WeightUnitM.GRAM);
            var kg5    = service.CreateGenericQuantity(5.0,    WeightUnitM.KILOGRAM);

            Console.WriteLine("\n  [Weight]");
            Console.WriteLine($"    Add:      {kg10} + {g5000} = {service.AddGenericQuantity(kg10, g5000)}  (expected: Quantity(15, KILOGRAM))");
            Console.WriteLine($"    Subtract: {kg10} - {kg5}   = {service.SubtractGenericQuantity(kg10, kg5)}  (expected: Quantity(5, KILOGRAM))");
            Console.WriteLine($"    Divide:   {kg10} ÷ {kg5}   = {service.DivideGenericQuantity(kg10, kg5)}  (expected: 2.0)");

            // Volume demo
            var l5  = service.CreateGenericQuantity(5.0, VolumeUnitM.LITRE);
            var l2  = service.CreateGenericQuantity(2.0, VolumeUnitM.LITRE);
            var ml500 = service.CreateGenericQuantity(500.0, VolumeUnitM.MILLILITRE);

            Console.WriteLine("\n  [Volume]");
            Console.WriteLine($"    Add:      {l5} + {l2}    = {service.AddGenericQuantity(l5, l2)}  (expected: Quantity(7, LITRE))");
            Console.WriteLine($"    Subtract: {l5} - {ml500} = {service.SubtractGenericQuantity(l5, ml500)}  (expected: Quantity(4.5, LITRE))");
            Console.WriteLine($"    Divide:   {l5} ÷ {l2}   = {service.DivideGenericQuantity(l5, l2)}  (expected: 2.5)");

            Console.WriteLine("\n  ✓ All operations use same centralized validation + computation helper.");
            Console.WriteLine("  ✓ DRY principle enforced - validation code appears once.");
        }

        // ================= UC14 - Temperature Measurement =================

        private void RunUC14()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine("|  UC14: Temperature Measurement                           |");
                Console.WriteLine("|        Selective Arithmetic + IMeasurable Refactoring    |");
                Console.WriteLine("+----------------------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("  1. Temperature Equality");
                Console.WriteLine("  2. Temperature Conversion");
                Console.WriteLine("  3. Demonstrate Unsupported Operations");
                Console.WriteLine("  4. Full Demonstration (all features)");
                Console.WriteLine("  5. Cross-Category Prevention");
                Console.WriteLine();
                Console.Write("Enter choice (1-5): ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunTemperatureEquality();        break;
                    case "2": RunTemperatureConversion();      break;
                    case "3": RunTemperatureUnsupported();     break;
                    case "4": RunTemperatureFullDemo();        break;
                    case "5": RunTemperatureCrossCategory();   break;
                    default:  Console.WriteLine("\nInvalid choice. Enter 1-5."); break;
                }
            }
            catch (NotSupportedException ex) { Console.WriteLine("\nUnsupported: " + ex.Message); }
            catch (ArgumentException ex)     { Console.WriteLine("\nError: " + ex.Message); }
            catch (Exception ex)             { Console.WriteLine("\nError: " + ex.Message); }
        }

        private void RunTemperatureEquality()
        {
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|              Temperature Equality                        |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine();
            Console.Write("Enter first temperature value: ");
            double v1 = ReadDouble();
            TemperatureUnit u1 = ParseTemperatureUnit();
            Console.Write("Enter second temperature value: ");
            double v2 = ReadDouble();
            TemperatureUnit u2 = ParseTemperatureUnit();

            var t1 = service.CreateTemperature(v1, u1);
            var t2 = service.CreateTemperature(v2, u2);
            bool result = service.CompareTemperature(t1, t2);

            Console.WriteLine($"\n  {t1} == {t2} ? {result}");
            Console.WriteLine($"  ({v1}°{u1} in Kelvin = {u1.ConvertToBaseUnit(v1):F4} K)");
            Console.WriteLine($"  ({v2}°{u2} in Kelvin = {u2.ConvertToBaseUnit(v2):F4} K)");
        }

        private void RunTemperatureConversion()
        {
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|              Temperature Conversion                      |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine();
            Console.Write("Enter temperature value: ");
            double val = ReadDouble();
            TemperatureUnit fromUnit = ParseTemperatureUnit();
            Console.Write("Enter target unit (celsius/fahrenheit/kelvin): ");
            TemperatureUnit toUnit = ReadTemperatureUnit();

            var temp   = service.CreateTemperature(val, fromUnit);
            var result = service.ConvertTemperature(temp, toUnit);

            Console.WriteLine($"\n  {temp} -> {result}");
        }

        private void RunTemperatureUnsupported()
        {
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|   Temperature: Unsupported Arithmetic Operations         |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("  Temperature does not support add, subtract, or divide.");
            Console.WriteLine("  Enter two temperatures to see the error handling.");
            Console.WriteLine();

            Console.Write("Enter first temperature value: ");
            double v1 = ReadDouble();
            TemperatureUnit u1 = ParseTemperatureUnit();

            Console.Write("Enter second temperature value: ");
            double v2 = ReadDouble();
            TemperatureUnit u2 = ParseTemperatureUnit();

            var t1 = service.CreateTemperature(v1, u1);
            var t2 = service.CreateTemperature(v2, u2);

            Console.WriteLine();

            try   { t1.Add(t2);      Console.WriteLine("  Add      : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Add      : Not supported for temperature"); }

            try   { t1.Subtract(t2); Console.WriteLine("  Subtract : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Subtract : Not supported for temperature"); }

            try   { t1.Divide(t2);   Console.WriteLine("  Divide   : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Divide   : Not supported for temperature"); }

            Console.WriteLine();
            Console.WriteLine("  All arithmetic operations are correctly rejected for temperature.");
        }

        private void RunTemperatureFullDemo()
        {
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|   UC14: Full Temperature Demonstration                   |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine();

            // ---- Equality ----
            Console.WriteLine("  Section 1: Equality");
            Console.Write("  Enter first temperature value: ");
            double eq1v = ReadDouble();
            TemperatureUnit eq1u = ParseTemperatureUnit();
            Console.Write("  Enter second temperature value: ");
            double eq2v = ReadDouble();
            TemperatureUnit eq2u = ParseTemperatureUnit();
            var eqT1 = service.CreateTemperature(eq1v, eq1u);
            var eqT2 = service.CreateTemperature(eq2v, eq2u);
            Console.WriteLine($"  {eq1v} degree {eq1u.GetUnitName()} == {eq2v} degree {eq2u.GetUnitName()} ? {service.CompareTemperature(eqT1, eqT2)}");

            // ---- Conversion ----
            Console.WriteLine();
            Console.WriteLine("  Section 2: Conversion");
            Console.Write("  Enter temperature value to convert: ");
            double convV = ReadDouble();
            TemperatureUnit convFrom = ParseTemperatureUnit();
            Console.Write("  Enter target unit (celsius/fahrenheit/kelvin): ");
            TemperatureUnit convTo = ReadTemperatureUnit();
            var convTemp   = service.CreateTemperature(convV, convFrom);
            var convResult = service.ConvertTemperature(convTemp, convTo);
            Console.WriteLine($"  {convV} degree {convFrom.GetUnitName()} = {convResult.Value} degree {convTo.GetUnitName()}");

            // ---- Operation Support ----
            Console.WriteLine();
            Console.WriteLine("  Section 3: Operation Support Check");
            Console.Write("  Enter a temperature unit to check (celsius/fahrenheit/kelvin): ");
            TemperatureUnit checkUnit = ReadTemperatureUnit();
            Console.WriteLine($"  Does {checkUnit.GetUnitName()} support arithmetic : {checkUnit.SupportsArithmetic()}");
            Console.WriteLine($"  Does FEET support arithmetic                      : {((IMeasurable)LengthUnitM.FEET).SupportsArithmetic()}");

            // ---- Unsupported Operations ----
            Console.WriteLine();
            Console.WriteLine("  Section 4: Unsupported Arithmetic");
            Console.Write("  Enter first temperature value: ");
            double ar1v = ReadDouble();
            TemperatureUnit ar1u = ParseTemperatureUnit();
            Console.Write("  Enter second temperature value: ");
            double ar2v = ReadDouble();
            TemperatureUnit ar2u = ParseTemperatureUnit();
            var arT1 = service.CreateTemperature(ar1v, ar1u);
            var arT2 = service.CreateTemperature(ar2v, ar2u);

            Console.WriteLine();
            try   { arT1.Add(arT2);      Console.WriteLine("  Add      : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Add      : Not supported for temperature"); }

            try   { arT1.Subtract(arT2); Console.WriteLine("  Subtract : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Subtract : Not supported for temperature"); }

            try   { arT1.Divide(arT2);   Console.WriteLine("  Divide   : No exception (unexpected)"); }
            catch (NotSupportedException) { Console.WriteLine("  Divide   : Not supported for temperature"); }

            Console.WriteLine();
            Console.WriteLine("  Section 5: Cross-Category Prevention");
            Console.Write("  Enter a temperature value: ");
            double crossV = ReadDouble();
            TemperatureUnit crossU = ParseTemperatureUnit();
            var crossTemp   = service.CreateTemperature(crossV, crossU);
            var crossLength = service.CreateGenericQuantity(crossV, LengthUnitM.FEET);
            var crossWeight = service.CreateGenericQuantity(crossV, WeightUnitM.KILOGRAM);
            Console.WriteLine($"  {crossV} degree {crossU.GetUnitName()} == {crossV} FEET     : {service.CheckCrossCategory(crossTemp, crossLength)}");
            Console.WriteLine($"  {crossV} degree {crossU.GetUnitName()} == {crossV} KILOGRAM : {service.CheckCrossCategory(crossTemp, crossWeight)}");

            Console.WriteLine();
            Console.WriteLine("  Demo complete.");
        }

        private void RunTemperatureCrossCategory()
        {
            Console.Clear();
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine("|   Temperature: Cross-Category Prevention                 |");
            Console.WriteLine("+----------------------------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("  A temperature value can never equal a length, weight,");
            Console.WriteLine("  or volume value even if the numbers are the same.");
            Console.WriteLine("  Enter a temperature and then pick another category to compare.");
            Console.WriteLine();

            Console.Write("Enter temperature value: ");
            double tempVal = ReadDouble();
            TemperatureUnit tempUnit = ParseTemperatureUnit();
            var tempQ = service.CreateTemperature(tempVal, tempUnit);

            Console.WriteLine();
            Console.WriteLine("  Select a category to compare against:");
            Console.WriteLine("  1. Length (Feet)");
            Console.WriteLine("  2. Weight (Kilogram)");
            Console.WriteLine("  3. Volume (Litre)");
            Console.WriteLine();
            Console.Write("Enter choice (1-3): ");
            string? cat = Console.ReadLine();

            Console.WriteLine();
            switch (cat)
            {
                case "1":
                    var lengthQ = service.CreateGenericQuantity(tempVal, LengthUnitM.FEET);
                    Console.WriteLine($"  {tempVal} degree {tempUnit.GetUnitName()} compared to {tempVal} FEET");
                    Console.WriteLine($"  Are they equal : {service.CheckCrossCategory(tempQ, lengthQ)}");
                    break;
                case "2":
                    var weightQ = service.CreateGenericQuantity(tempVal, WeightUnitM.KILOGRAM);
                    Console.WriteLine($"  {tempVal} degree {tempUnit.GetUnitName()} compared to {tempVal} KILOGRAM");
                    Console.WriteLine($"  Are they equal : {service.CheckCrossCategory(tempQ, weightQ)}");
                    break;
                case "3":
                    var volumeQ = service.CreateGenericQuantity(tempVal, VolumeUnitM.LITRE);
                    Console.WriteLine($"  {tempVal} degree {tempUnit.GetUnitName()} compared to {tempVal} LITRE");
                    Console.WriteLine($"  Are they equal : {service.CheckCrossCategory(tempQ, volumeQ)}");
                    break;
                default:
                    Console.WriteLine("  Invalid choice.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("  Temperature and other categories are always different types.");
        }

        private static TemperatureUnit ParseTemperatureUnit()
        {
            Console.Write("Enter unit (celsius/fahrenheit/kelvin): ");
            return ReadTemperatureUnit();
        }

        private static TemperatureUnit ReadTemperatureUnit()
        {
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
            return input switch
            {
                "celsius"    or "c" => TemperatureUnit.CELSIUS,
                "fahrenheit" or "f" => TemperatureUnit.FAHRENHEIT,
                "kelvin"     or "k" => TemperatureUnit.KELVIN,
                _ => throw new ArgumentException($"Unknown temperature unit '{input}'. Use celsius/fahrenheit/kelvin.")
            };
        }

        // ===== Generic demonstration helpers (Presentation only — calls BLL internally via service) =====
        private void DemonstrateEquality<U>(Quantity<U> a, Quantity<U> b) where U : class, IMeasurable
            => Console.WriteLine($"  Equality: {a} == {b} ? {service.CompareGenericQuantity(a, b)}");

        private void DemonstrateConversion<U>(Quantity<U> q, U target) where U : class, IMeasurable
            => Console.WriteLine($"  Conversion: {q} -> {service.ConvertGenericQuantity(q, target)}");

        private void DemonstrateAddition<U>(Quantity<U> a, Quantity<U> b, U target) where U : class, IMeasurable
            => Console.WriteLine($"  Addition: {a} + {b} = {service.AddGenericQuantity(a, b, target)}");

        // ===== Input parsing helpers =====

        private static double ParseDouble(string? input)
        {
            if (string.IsNullOrWhiteSpace(input) || !double.TryParse(input, out double val))
                throw new ArgumentException("Invalid input: Value must be numeric.");
            return val;
        }

        private static LengthUnit ParseUnit(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");
            return input.Trim().ToLower() switch
            {
                "feet"        => LengthUnit.FEET,
                "foot"        => LengthUnit.FEET,
                "inch"        => LengthUnit.INCH,
                "inches"      => LengthUnit.INCH,
                "yard"        => LengthUnit.YARD,
                "yards"       => LengthUnit.YARD,
                "cm"          => LengthUnit.CENTIMETER,
                "centimeter"  => LengthUnit.CENTIMETER,
                "centimeters" => LengthUnit.CENTIMETER,
                _             => throw new ArgumentException("Invalid unit entered.")
            };
        }

        private static WeightUnit ParseWeightUnit(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Invalid unit entered.");
            return input.Trim().ToLowerInvariant() switch
            {
                "kg" or "kilogram" or "kilograms" => WeightUnit.KILOGRAM,
                "g"  or "gram"     or "grams"     => WeightUnit.GRAM,
                "lb" or "lbs" or "pound" or "pounds" => WeightUnit.POUND,
                _ => throw new ArgumentException("Invalid weight unit entered.")
            };
        }

        private static double ReadDouble()
        {
            string? input = Console.ReadLine();
            if (!double.TryParse(input, out double val))
                throw new ArgumentException("Invalid numeric input.");
            return val;
        }

        private static LengthUnitM ParseLengthUnitM()
        {
            Console.Write("Enter unit (feet/inches/yards/cm): ");
            return ReadLengthUnitM();
        }

        private static LengthUnitM ReadLengthUnitM()
        {
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
            return input switch
            {
                "feet" or "foot" or "ft"          => LengthUnitM.FEET,
                "inches" or "inch" or "in"         => LengthUnitM.INCHES,
                "yards" or "yard" or "yd"          => LengthUnitM.YARDS,
                "centimeters" or "centimeter" or "cm" => LengthUnitM.CENTIMETERS,
                _ => throw new ArgumentException($"Unknown length unit '{input}'. Use feet/inches/yards/cm.")
            };
        }

        private static WeightUnitM ParseWeightUnitM()
        {
            Console.Write("Enter unit (kg/g/lb): ");
            return ReadWeightUnitM();
        }

        private static WeightUnitM ReadWeightUnitM()
        {
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
            return input switch
            {
                "kg" or "kilogram" or "kilograms" => WeightUnitM.KILOGRAM,
                "g"  or "gram"     or "grams"     => WeightUnitM.GRAM,
                "lb" or "lbs" or "pound" or "pounds" => WeightUnitM.POUND,
                _ => throw new ArgumentException($"Unknown weight unit '{input}'. Use kg/g/lb.")
            };
        }

        /// <summary>Parses a volume unit string into VolumeUnit enum.</summary>
        private static VolumeUnit ParseVolumeUnit(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");

            return input.Trim().ToLowerInvariant() switch
            {
                "litre"      or "liter"      or "l"   => VolumeUnit.LITRE,
                "millilitre" or "milliliter" or "ml"  => VolumeUnit.MILLILITRE,
                "gallon"     or "gallons"    or "gal" => VolumeUnit.GALLON,
                _ => throw new ArgumentException($"Invalid volume unit '{input}'. Use litre/ml/gallon.")
            };
        }

        private static VolumeUnitM ParseVolumeUnitM()
        {
            Console.Write("Enter unit (litre/ml/gallon): ");
            return ReadVolumeUnitM();
        }

        private static VolumeUnitM ReadVolumeUnitM()
        {
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
            return input switch
            {
                "litre"      or "liter"      or "l"   => VolumeUnitM.LITRE,
                "millilitre" or "milliliter" or "ml"  => VolumeUnitM.MILLILITRE,
                "gallon"     or "gallons"    or "gal" => VolumeUnitM.GALLON,
                _ => throw new ArgumentException($"Unknown volume unit '{input}'. Use litre/ml/gallon.")
            };
        }
    }
}
