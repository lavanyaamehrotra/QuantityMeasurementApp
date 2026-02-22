using System;
using uc1_feet_equality.BusinessLogicLayer;

namespace uc1_feet_equality.PresentationLayer
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
                Console.WriteLine("|   3. Exit                                |");
                Console.WriteLine("|                                          |");
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
            Console.WriteLine("Feature coming soon...");
        }
    }
}