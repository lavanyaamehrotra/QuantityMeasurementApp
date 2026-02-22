using System;

namespace uc1_feet_equality.DataAccessLayer
{
    /// <summary>
    /// Responsible for logging application activities.
    /// Represents Data Access responsibility.
    /// </summary>
    public class MeasurementLogger
    {
        /// <summary>
        /// Writes log message to console.
        /// </summary>
        public void Log(string message)
        {
            // Logging format
            Console.WriteLine($"[Output]: {message}");
        }
    }
}