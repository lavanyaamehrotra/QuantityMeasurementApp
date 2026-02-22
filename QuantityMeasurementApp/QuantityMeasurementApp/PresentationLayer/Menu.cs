using System;
using QuantityMeasurementApp.BusinessLogicLayer;

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
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|        Quantity Measurement App          |");
                Console.WriteLine("+------------------------------------------+");
                Console.WriteLine("|                                          |");
                Console.WriteLine("|   1. UC1 - Feet Equality                 |");
                Console.WriteLine("|   2. UC2 - Feet & Inches Equality        |");
                Console.WriteLine("|   3. UC3 - Generic Length (Generics)     |");                
                Console.WriteLine("|   4. Exit                                |");
                Console.WriteLine("+------------------------------------------+");

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
            Console.Clear();

            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine("|   UC3 - Generic Length Equality          |");
            Console.WriteLine("+------------------------------------------+");
            Console.WriteLine();
            Console.WriteLine("Feature coming soon...");
            Console.WriteLine("This module will use Generics for Length.");
        }
    }
}