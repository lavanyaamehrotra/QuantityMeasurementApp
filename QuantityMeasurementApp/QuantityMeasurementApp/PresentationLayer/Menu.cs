using System;
using QuantityMeasurementApp.BusinessLogicLayer;
using QuantityMeasurementApp.Entities;
namespace QuantityMeasurementApp.PresentationLayer
{
    // Menu class handles all user interaction
    // This is the Presentation Layer of the application
    public class Menu
    {
        // Service layer object
        // All validations MUST go through service layer
        //readonly-The value can be assigned only once
        //private-can be accessed only within the class
        //This class will always use the SAME service object.
        private readonly QuantityMeasurementService service = new QuantityMeasurementService();

        // Application start menu
        public void Start()
        {
            bool running = true;
            while (running)
            {
                // ===== MENU BOX =====
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine("|             Quantity Measurement App           |");
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine("|                                                |");
                Console.WriteLine("|   1. UC1 - Feet Equality                       |");
                Console.WriteLine("|   2. UC2 - Feet & Inches Equality              |");
                Console.WriteLine("|   3. UC3 - Generic Length (Generics)           |");
                Console.WriteLine("|   4. UC4 - Extended Unit Support               |");
                Console.WriteLine("|   5. UC5 - Unit Conversion                     |");
                Console.WriteLine("|   6. UC6 - Addition of Two Length Units        |");
                Console.WriteLine("|   7. UC7 - Addition With Target                |");
                Console.WriteLine("|   8. UC8 - Refactoring Unit Enum Standalone    |");
                Console.WriteLine("|   9. Exit                                      |");
                Console.WriteLine("+------------------------------------------------+");

                Console.Write("\nEnter your choice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunUC1();
                        break;

                    case "2":
                        RunUC2();
                        break;
                    
                    case "3":
                        RunUC3();
                        break;
                    
                    case "4":
                        RunUC4(); 
                        break;
                    
                    case "5":
                        RunUC5();
                        break;

                    case "6":
                        RunUC6();
                        break;

                    case "7":
                        RunUC7();
                        break;

                    case "8":
                        RunUC8();
                        break;

                    case "9":
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
                    Console.ReadKey();//waits until the user presses any key on the keyboard
                    Console.Clear(); // clear ONLY when returning to menu
                }
            }
        }
        // ================= UC1 =================
        // UC1 execution method
        private void RunUC1()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|            UC1 - Feet Equality           |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                // FIRST INPUT
                Console.Write("Enter first feet value: ");
                string? input1 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input1)){
                    throw new ArgumentException("Invalid input: First value cannot be empty.");
                }
                if (!double.TryParse(input1, out double v1)){
                    throw new ArgumentException("Invalid input: First value must be numeric.");
                }
                var f1 = service.CreateFeet(v1);
                // SECOND INPUT
                Console.Write("Enter second feet value: ");
                string? input2 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input2)){
                    throw new ArgumentException("Invalid input: Second value cannot be empty.");
                }
                //Try converting safely and give me converted number if possible.
                if (!double.TryParse(input2, out double v2)){
                    throw new ArgumentException("Invalid input: Second value must be numeric.");
                }
                var f2 = service.CreateFeet(v2);

                // COMPARISON
                bool result = service.CompareFeet(f1, f2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|            Result : Equal (" + result + ")         |");
                Console.WriteLine("+------------------------------------------+");

                // EXTRA FEATURE
                double difference = Math.Abs(v1 - v2);

                Console.WriteLine($"Measurement Difference: {difference} ft");

                if (difference == 0){
                    Console.WriteLine("Both measurements are exactly identical.");
                }else if (difference < 1){
                    Console.WriteLine("Measurements are very close.");
                }else{
                    Console.WriteLine("Measurements differ significantly.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (OverflowException)
            {
                Console.WriteLine("Invalid input: Number too large.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error occurred.");
            }
        }

        // ================= UC2 =================
        private void RunUC2()
        {
            Console.Clear();

            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine("|     UC2 - Feet & Inches Equality         |");
            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine();

            // ================= FEET INPUT =================
            try
            {
                Console.WriteLine("Feet Equality Check");

                Console.Write("Enter first feet value: ");
                string? fInput1 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(fInput1)){
                    throw new ArgumentException("Invalid input: First feet value empty.");
                }
                if (!double.TryParse(fInput1, out double f1Value)){
                    throw new ArgumentException("Invalid input: Feet must be numeric.");
                }
                var f1 = service.CreateFeet(f1Value);

                Console.Write("Enter second feet value: ");
                string? fInput2 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(fInput2)){
                    throw new ArgumentException("Invalid input: Second feet value empty.");
                }
                if (!double.TryParse(fInput2, out double f2Value)){
                    throw new ArgumentException("Invalid input: Feet must be numeric.");
                }
                var f2 = service.CreateFeet(f2Value);

                bool feetResult = service.CompareFeet(f1, f2);
                Console.WriteLine($"\nFeet Equal ({feetResult})");

                // ===== EXTRA FEATURE (FEET) =====
                Console.WriteLine(service.GetMeasurementAnalysis(f1Value, f2Value, "ft"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Feet comparison skipped.");
            }

            Console.WriteLine();

            // ================= INCHES INPUT =================
            try
            {
                Console.WriteLine("Inches Equality Check");

                Console.Write("Enter first inches value: ");
                string? iInput1 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(iInput1)){
                    throw new ArgumentException("Invalid input: First inches value empty.");
                }
                if (!double.TryParse(iInput1, out double i1Value)){
                    throw new ArgumentException("Invalid input: Inches must be numeric.");
                }
                var i1 = service.CreateInches(i1Value);

                Console.Write("Enter second inches value: ");
                string? iInput2 = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(iInput2)){
                    throw new ArgumentException("Invalid input: Second inches value empty.");
                }
                if (!double.TryParse(iInput2, out double i2Value)){
                    throw new ArgumentException("Invalid input: Inches must be numeric.");
                }
                var i2 = service.CreateInches(i2Value);

                bool inchResult = service.CompareInches(i1, i2);

                Console.WriteLine($"\nInches Equal ({inchResult})");

                // ===== EXTRA FEATURE (INCHES) =====
                Console.WriteLine(service.GetMeasurementAnalysis(i1Value, i2Value, "in"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Inches comparison skipped.");
            }
        }
        
        // ================= UC3 =================
        
        private void RunUC3()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|     UC3 - Generic Quantity Equality      |");
                Console.WriteLine("+------------------------------------------+");

                Console.Write("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter unit (feet/inch): ");
                LengthUnit u1 =Enum.Parse<LengthUnit>(Console.ReadLine()!, true);

                Console.Write("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter unit (feet/inches): ");
                LengthUnit u2 =Enum.Parse<LengthUnit>(Console.ReadLine()!, true);

                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                bool result = service.CompareQuantity(q1, q2);

                Console.WriteLine($"\nEqual ({result})");

                double diff =Math.Abs(v1 * u1.ToFeetFactor()- v2 * u2.ToFeetFactor());

                Console.WriteLine($"Measurement Difference: {diff} ft");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        // ================= UC4 =================
        // Extended Unit Support (Future Enhancement)
        private void RunUC4()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|     UC4 - Extended Unit Support          |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                // FIRST INPUT
                Console.Write("Enter first value: ");
                double value1 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit unit1 = ParseUnit(Console.ReadLine());

                // SECOND INPUT
                Console.Write("\nEnter second value: ");
                double value2 = double.Parse(Console.ReadLine()!);

                Console.Write("Enter unit (feet/inch/yard/cm): ");
                LengthUnit unit2 = ParseUnit(Console.ReadLine());

                // Create quantities
                var q1 = new QuantityLength(value1, unit1);
                var q2 = new QuantityLength(value2, unit2);

                // Equality comparison
                bool result = q1.Equals(q2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine($"|        Result : Equal ({result})          |");
                Console.WriteLine("+------------------------------------------+");

                // ================= EXTRA FEATURE (UC4) =================
                // Convert both values into FEET
                double value1InFeet = q1.ConvertToFeet();
                double value2InFeet = q2.ConvertToFeet();

                double difference = Math.Abs(value1InFeet - value2InFeet);

                Console.WriteLine("\nMeasurement Difference: " + difference + " ft");

                if (difference == 0)
                {
                    Console.WriteLine("Both measurements are exactly identical.");
                }
                else if (difference < 1)
                {
                    Console.WriteLine("Measurements are very close.");
                }
                else
                {
                    Console.WriteLine("Measurements differ significantly.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private LengthUnit ParseUnit(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Unit cannot be empty.");

            input = input.Trim().ToLower();

            return input switch
            {
                "feet" => LengthUnit.FEET,
                "inch" => LengthUnit.INCH,
                "inches" => LengthUnit.INCH,
                "yard" => LengthUnit.YARD,
                "yards" => LengthUnit.YARD,
                "cm" => LengthUnit.CENTIMETER,
                "centimeter" => LengthUnit.CENTIMETER,
                "centimeters" => LengthUnit.CENTIMETER,
                _ => throw new ArgumentException("Invalid unit entered.")
            };
        }
        // ================= UC5 =================
        // UC5 - Unit to Unit Conversion (Same Measurement Type)
        private void RunUC5()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|       UC5 - Unit To Unit Conversion      |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();

                // VALUE INPUT
                Console.Write("Enter length value to convert: ");
                string? valueInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(valueInput))
                    throw new ArgumentException("Invalid input: Value cannot be empty.");

                if (!double.TryParse(valueInput, out double value))
                    throw new ArgumentException("Invalid input: Value must be numeric.");

                if (!double.IsFinite(value))
                    throw new ArgumentException("Invalid input: Value must be finite (not NaN or Infinity).");

                // SOURCE UNIT
                Console.Write("Enter source unit (feet/inch/yard/cm): ");
                LengthUnit fromUnit = ParseUnit(Console.ReadLine());

                // TARGET UNIT
                Console.Write("Enter target unit (feet/inch/yard/cm): ");
                LengthUnit toUnit = ParseUnit(Console.ReadLine());

                // Perform conversion via service
                double result = service.ConvertLength(value, fromUnit, toUnit);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine($"|  {value} {fromUnit} = {result} {toUnit}  |");
                Console.WriteLine("+------------------------------------------+");

                // ===== EXTRA FEATURE (UC5) =====
                // Instance conversion - method overload demonstration
                var quantity = new QuantityLength(value, fromUnit);
                var convertedQuantity = quantity.ConvertTo(toUnit);
                Console.WriteLine($"\nInstance conversion: {quantity} -> {convertedQuantity}");

                double valueInFeet = quantity.ConvertToFeet();
                Console.WriteLine($"Physical length in base unit: {valueInFeet} ft");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|  Error: " + ex.Message + " |");
                Console.WriteLine("+------------------------------------------+");
            }
            catch (OverflowException)
            {
                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|  Error: Number too large.                 |");
                Console.WriteLine("+------------------------------------------+");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|  Error: " + ex.Message);
                Console.WriteLine("+------------------------------------------+");
            }
        }

        // ================= UC6 =================
        // UC6 - Addition of Two Length Units
        private void RunUC6()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|     UC6 - Addition of Two Length Units   |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("Add two length measurements. Result is in unit of first operand.");
                Console.WriteLine();

                // FIRST LENGTH
                Console.Write("Enter first value: ");
                string? input1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input1) || !double.TryParse(input1, out double v1))
                    throw new ArgumentException("Invalid input: First value must be numeric.");
                Console.Write("Enter first unit (feet/inch/yard/cm): ");
                LengthUnit u1 = ParseUnit(Console.ReadLine()!);

                // SECOND LENGTH
                Console.Write("\nEnter second value: ");
                string? input2 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input2) || !double.TryParse(input2, out double v2))
                    throw new ArgumentException("Invalid input: Second value must be numeric.");
                Console.Write("Enter second unit (feet/inch/yard/cm): ");
                LengthUnit u2 = ParseUnit(Console.ReadLine()!);

                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                QuantityLength result = service.AddLength(q1, q2);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|     UC6 - Addition Result                |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine($"|  {q1}  +  {q2}  =  {result}  |");
                Console.WriteLine("+------------------------------------------+");

                // ===== EXTRA FEATURE (UC6) =====
                double sumInFeet = result.ConvertToFeet();
                Console.WriteLine($"\n  Physical length in base unit: {sumInFeet} ft");
                QuantityLength inInches = result.ConvertTo(LengthUnit.INCH);
                QuantityLength inYards = result.ConvertTo(LengthUnit.YARD);
                QuantityLength inCm = result.ConvertTo(LengthUnit.CENTIMETER);
                Console.WriteLine($"  Equivalent to: {inInches} | {inYards} | {inCm}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }
        }

        // ================= UC7 =================
        // UC7 - Addition With Target Unit Specification
        private void RunUC7()
        {
            try
            {
                Console.Clear();

                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|     UC7 - Addition With Target           |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine();
                Console.WriteLine("Add two lengths. Result in your chosen target unit.");
                Console.WriteLine();

                // FIRST LENGTH
                Console.Write("Enter first value: ");
                string? input1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input1) || !double.TryParse(input1, out double v1))
                    throw new ArgumentException("Invalid input: First value must be numeric.");
                Console.Write("Enter first unit (feet/inch/yard/cm): ");
                LengthUnit u1 = ParseUnit(Console.ReadLine()!);

                // SECOND LENGTH
                Console.Write("\nEnter second value: ");
                string? input2 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input2) || !double.TryParse(input2, out double v2))
                    throw new ArgumentException("Invalid input: Second value must be numeric.");
                Console.Write("Enter second unit (feet/inch/yard/cm): ");
                LengthUnit u2 = ParseUnit(Console.ReadLine()!);

                // TARGET UNIT
                Console.Write("\nEnter target unit for result (feet/inch/yard/cm): ");
                LengthUnit targetUnit = ParseUnit(Console.ReadLine()!);

                var q1 = service.CreateQuantity(v1, u1);
                var q2 = service.CreateQuantity(v2, u2);

                QuantityLength result = service.AddLength(q1, q2, targetUnit);

                Console.WriteLine("\n+------------------------------------------+");
                Console.WriteLine("|     UC7 - Addition Result                |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine($"|  {q1}  +  {q2}  =  {result}  |");
                Console.WriteLine("+------------------------------------------+");

                // EXTRA FEATURE
                double sumInFeet = result.ConvertToFeet();
                Console.WriteLine($"\n  Physical length in base unit: {sumInFeet} ft");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
            }
        }

        // ================= UC8 =================
        private void RunUC8()
        {
            Console.Clear();
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine("|     UC8  Refactoring-Unit-Enum-To-Standalone  |");
            Console.WriteLine("+-----------------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("  Feature coming soon...");
        }
    }
}