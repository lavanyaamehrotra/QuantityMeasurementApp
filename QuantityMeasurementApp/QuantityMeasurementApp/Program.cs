using QuantityMeasurementApp.PresentationLayer;

namespace QuantityMeasurementApp
{
    // <summary>
    // Application entry point.
    // Initializes and starts menu system.
    // </summary>
    class Program
    {
        static void Main()
        {
            // Start menu-driven application
            Menu menu = new Menu();
            menu.Start();
        }
    }
}