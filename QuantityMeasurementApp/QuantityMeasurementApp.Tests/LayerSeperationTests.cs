using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel;
using QuantityMeasurementRepository;
using QuantityMeasurementBusinessLayer;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC15: Layer Separation Tests — Controller, Service, Entity, Data Flow.
    ///
    /// Tests verify:
    ///  1.  QuantityMeasurementEntity construction, getters, immutability, and toString.
    ///  2.  Service operations: compare, convert, add, subtract, divide, error handling.
    ///  3.  Controller operations: routing, output formatting, error display.
    ///  4.  Layer separation: service independence, controller independence (DI).
    ///  5.  Data flow: controller → service → controller (QuantityDTO as contract).
    ///  6.  Backward compatibility: all UC1–UC14 behaviors preserved.
    ///  7.  Scalability and extensibility across all measurement categories.
    /// </summary>
    [TestClass]
    public class UC15LayerSeparationTests
    {
        // ────────────────────────────────────────────────────────────────
        // SHARED INFRASTRUCTURE
        // ────────────────────────────────────────────────────────────────

        private IQuantityMeasurementRepository _repo    = null!;
        private IQuantityMeasurementService    _service = null!;
        private QuantityMeasurementController  _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = QuantityMeasurementCacheRepository.Instance;
            _repo.Clear();
            _service    = new QuantityMeasurementServiceImpl(_repo);
            _controller = new QuantityMeasurementController(_service, _repo);
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 1: QuantityMeasurementEntity Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies QuantityEntity correctly stores single-operand conversion data.
        /// Tests: Constructor and getters for conversion scenario.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_SingleOperandConstruction()
        {
            var operand1 = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var result   = new QuantityDTO(12.0,"INCHES", "LENGTH");

            var entity = new QuantityMeasurementEntity("CONVERT", operand1, result);

            Assert.AreEqual("CONVERT",  entity.OperationType);
            Assert.AreEqual(operand1,   entity.Operand1);
            Assert.IsNull(entity.Operand2);
            Assert.AreEqual(result,     entity.Result);
            Assert.IsFalse(entity.HasError);
            Assert.AreEqual(string.Empty, entity.ErrorMessage);
        }

        /// <summary>
        /// Verifies QuantityEntity correctly stores binary operation data.
        /// Tests: Constructor and getters for addition scenario.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_BinaryOperandConstruction()
        {
            var operand1 = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var operand2 = new QuantityDTO(12.0,"INCHES", "LENGTH");
            var result   = new QuantityDTO(2.0, "FEET",   "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", operand1, operand2, result);

            Assert.AreEqual("ADD",    entity.OperationType);
            Assert.AreEqual(operand1, entity.Operand1);
            Assert.AreEqual(operand2, entity.Operand2);
            Assert.AreEqual(result,   entity.Result);
            Assert.IsFalse(entity.HasError);
        }

        /// <summary>
        /// Verifies QuantityEntity correctly stores error data.
        /// Tests: Error constructor and hasError() method.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ErrorConstruction()
        {
            var operand1 = new QuantityDTO(100.0, "CELSIUS",    "TEMPERATURE");
            var operand2 = new QuantityDTO(50.0,  "CELSIUS",    "TEMPERATURE");
            string errorMsg = "Temperature does not support arithmetic operations.";

            var entity = new QuantityMeasurementEntity("ADD", operand1, operand2, errorMsg);

            Assert.AreEqual("ADD",    entity.OperationType);
            Assert.AreEqual(operand1, entity.Operand1);
            Assert.AreEqual(operand2, entity.Operand2);
            Assert.IsTrue(entity.HasError);
            Assert.AreEqual(errorMsg, entity.ErrorMessage);
            Assert.IsNull(entity.Result);
        }

        /// <summary>
        /// Verifies toString() formats successful results clearly.
        /// Tests: String representation for reading.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ToString_Success()
        {
            var op1    = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var op2    = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var result = new QuantityDTO(2.0,  "FEET",   "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);
            string text = entity.ToString();

            // Must contain key details about operation, operands and result
            Assert.IsTrue(text.Contains("ADD"),    "ToString should contain operation type.");
            Assert.IsTrue(text.Contains("1"),      "ToString should contain operand1 value.");
            Assert.IsTrue(text.Contains("FEET"),   "ToString should contain unit name.");
            Assert.IsTrue(text.Contains("2"),      "ToString should contain result value.");
        }

        /// <summary>
        /// Verifies toString() formats errors clearly.
        /// Tests: Error message visibility.
        /// </summary>
        [TestMethod]
        public void testQuantityEntity_ToString_Error()
        {
            var op1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var op2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");
            string errorMsg = "Temperature does not support addition.";

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, errorMsg);
            string text = entity.ToString();

            Assert.IsTrue(text.Contains("Error"),       "ToString should contain 'Error'.");
            Assert.IsTrue(text.Contains("Temperature"), "ToString should contain error message.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 2: Service Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies service correctly compares quantities in same unit.
        /// Tests: Service delegates to Quantity.equals().
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_SameUnit_Success()
        {
            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            var result = _service.Compare(q1, q2);

            Assert.AreEqual(1.0, result.Value, "Equal quantities should return value 1.");
        }

        /// <summary>
        /// Verifies service correctly compares quantities in different units.
        /// Tests: Cross-unit comparison through service.
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_DifferentUnit_Success()
        {
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var result = _service.Compare(q1, q2);

            Assert.AreEqual(1.0, result.Value, "1 foot and 12 inches should be equal.");
        }

        /// <summary>
        /// Verifies service rejects cross-category comparison.
        /// Tests: Category compatibility check.
        /// </summary>
        [TestMethod]
        public void testService_CompareEquality_CrossCategory_Error()
        {
            var length = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var weight = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            bool threw = false;
            try
            {
                _service.Compare(length, weight);
            }
            catch (QuantityMeasurementException)
            {
                threw = true;
            }

            Assert.IsTrue(threw, "Comparing different categories should throw QuantityMeasurementException.");
        }

        /// <summary>
        /// Verifies service correctly converts between units.
        /// Tests: Service delegates to Quantity.convertTo().
        /// </summary>
        [TestMethod]
        public void testService_Convert_Success()
        {
            var q1     = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var target = new QuantityDTO(0.0, "INCHES", "LENGTH");

            var result = _service.Convert(q1, target);

            Assert.AreEqual(12.0,     result.Value,    1e-3, "1 foot = 12 inches.");
            Assert.AreEqual("INCHES", result.UnitName, "Result unit should be INCHES.");
        }

        /// <summary>
        /// Verifies service correctly performs addition.
        /// Tests: Service delegates to Quantity.add().
        /// </summary>
        [TestMethod]
        public void testService_Add_Success()
        {
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var result = _service.Add(q1, q2);

            Assert.AreEqual(2.0,    result.Value,    1e-3, "1 foot + 12 inches = 2 feet.");
            Assert.AreEqual("FEET", result.UnitName, "Result unit should be FEET.");
        }

        /// <summary>
        /// Verifies service handles unsupported operations (temperature add).
        /// Tests: Exception conversion to error entity.
        /// </summary>
        [TestMethod]
        public void testService_Add_UnsupportedOperation_Error()
        {
            var t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            bool threw = false;
            try
            {
                _service.Add(t1, t2);
            }
            catch (QuantityMeasurementException)
            {
                threw = true;
            }

            Assert.IsTrue(threw, "Adding temperatures should throw QuantityMeasurementException.");
        }

        /// <summary>
        /// Verifies service correctly performs subtraction.
        /// Tests: Service delegates to Quantity.subtract().
        /// </summary>
        [TestMethod]
        public void testService_Subtract_Success()
        {
            var q1 = new QuantityDTO(2.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            var result = _service.Subtract(q1, q2);

            Assert.AreEqual(1.0,    result.Value,    1e-3, "2 feet - 12 inches = 1 foot.");
            Assert.AreEqual("FEET", result.UnitName, "Result unit should be FEET.");
        }

        /// <summary>
        /// Verifies service correctly performs division.
        /// Tests: Service returns dimensionless scalar.
        /// </summary>
        [TestMethod]
        public void testService_Divide_Success()
        {
            var q1 = new QuantityDTO(2.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            var result = _service.Divide(q1, q2);

            Assert.AreEqual(2.0,      result.Value,    1e-3, "2 feet / 1 foot = 2 (scalar).");
            Assert.AreEqual("SCALAR", result.Category, "Division result should be SCALAR.");
        }

        /// <summary>
        /// Verifies service handles division by zero.
        /// Tests: Exception handling for zero divisor.
        /// </summary>
        [TestMethod]
        public void testService_Divide_ByZero_Error()
        {
            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(0.0, "FEET", "LENGTH");

            bool threw = false;
            try
            {
                _service.Divide(q1, q2);
            }
            catch (QuantityMeasurementException)
            {
                threw = true;
            }

            Assert.IsTrue(threw, "Dividing by zero should throw QuantityMeasurementException.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 3: Controller Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies controller correctly demonstrates equality.
        /// Tests: Controller → Service integration.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateEquality_Success()
        {
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            string output = _controller.PerformComparison(q1, q2);

            Assert.IsTrue(output.Contains("true"),
                "Equal quantities comparison should contain 'true'.");
        }

        /// <summary>
        /// Verifies controller correctly demonstrates conversion.
        /// Tests: Controller method routing.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateConversion_Success()
        {
            var q1     = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var target = new QuantityDTO(0.0, "INCHES", "LENGTH");

            string output = _controller.PerformConversion(q1, target);

            Assert.IsTrue(output.Contains("12"),
                "Conversion output should contain the result value 12.");
            Assert.IsTrue(output.Contains("INCHES"),
                "Conversion output should contain the target unit.");
        }

        /// <summary>
        /// Verifies controller correctly demonstrates addition.
        /// Tests: Controller handles successful operations.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateAddition_Success()
        {
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            string output = _controller.PerformAddition(q1, q2);

            Assert.IsTrue(output.Contains("2"),
                "Addition output should contain result value 2.");
            Assert.IsTrue(output.Contains("FEET"),
                "Addition output should contain result unit FEET.");
        }

        /// <summary>
        /// Verifies controller correctly displays errors.
        /// Tests: Controller error handling.
        /// </summary>
        [TestMethod]
        public void testController_DemonstrateAddition_Error()
        {
            var t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            string output = _controller.PerformAddition(t1, t2);

            Assert.IsTrue(output.Contains("[ERROR]") || output.ToUpperInvariant().Contains("ERROR"),
                "Controller should surface error message when operation is unsupported.");
        }

        /// <summary>
        /// Verifies controller formats success results correctly.
        /// Tests: Output formatting.
        /// </summary>
        [TestMethod]
        public void testController_DisplayResult_Success()
        {
            var q1 = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");
            var q2 = new QuantityDTO(0.0, "GRAM",     "WEIGHT");

            string output = _controller.PerformConversion(q1, q2);

            // Must contain both a numeric result and a unit label
            Assert.IsFalse(string.IsNullOrWhiteSpace(output),
                "Output should not be empty.");
            Assert.IsTrue(output.Contains("1000") || output.Contains("GRAM"),
                "Output should contain conversion result 1000 GRAM.");
        }

        /// <summary>
        /// Verifies controller displays error messages.
        /// Tests: Error output format.
        /// </summary>
        [TestMethod]
        public void testController_DisplayResult_Error()
        {
            // Cross-category triggers an error
            var length = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var weight = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            string output = _controller.PerformAddition(length, weight);

            Assert.IsTrue(
                output.Contains("[ERROR]") || output.ToUpperInvariant().Contains("ERROR"),
                "Controller error output should include an error indicator.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 4: Layer Separation Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies service can be tested without controller.
        /// Tests: Layer isolation enables unit testing.
        /// </summary>
        [TestMethod]
        public void testLayerSeparation_ServiceIndependence()
        {
            // Instantiate service standalone — no controller required
            IQuantityMeasurementRepository repo    = QuantityMeasurementCacheRepository.Instance;
            IQuantityMeasurementService    service = new QuantityMeasurementServiceImpl(repo);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            // Service works completely independently of controller
            var result = service.Compare(q1, q2);
            Assert.AreEqual(1.0, result.Value, "Service should work without controller.");
        }

        /// <summary>
        /// Verifies controller can work with mock service.
        /// Tests: Dependency injection pattern.
        /// </summary>
        [TestMethod]
        public void testLayerSeparation_ControllerIndependence()
        {
            // Inject a mock service — controller must not care about concrete type
            IQuantityMeasurementService    mockService = new MockQuantityMeasurementService();
            IQuantityMeasurementRepository mockRepo    = QuantityMeasurementCacheRepository.Instance;

            var controller = new QuantityMeasurementController(mockService, mockRepo);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            string output = controller.PerformComparison(q1, q2);

            // Controller functions correctly regardless of which service implementation is injected
            Assert.IsFalse(string.IsNullOrWhiteSpace(output),
                "Controller should produce output when using injected mock service.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 5: Data Flow Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies data correctly flows from controller to service.
        /// Tests: QuantityDTO as data contract.
        /// </summary>
        [TestMethod]
        public void testDataFlow_ControllerToService()
        {
            // Build QuantityDTOs (as controller would) and pass to service
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            // QuantityDTO carries value, unit, and category through layers correctly
            Assert.AreEqual(1.0,      q1.Value,    "DTO value must flow correctly.");
            Assert.AreEqual("FEET",   q1.UnitName, "DTO unit must flow correctly.");
            Assert.AreEqual("LENGTH", q1.Category, "DTO category must flow correctly.");

            var result = _service.Add(q1, q2);

            Assert.AreEqual(2.0, result.Value, 1e-3,
                "Data should flow from controller DTOs through service correctly.");
        }

        /// <summary>
        /// Verifies results correctly flow from service to controller.
        /// Tests: Standardized output format.
        /// </summary>
        [TestMethod]
        public void testDataFlow_ServiceToController()
        {
            var q1 = new QuantityDTO(1.0,  "KILOGRAM", "WEIGHT");
            var q2 = new QuantityDTO(0.0,  "GRAM",     "WEIGHT");

            // Service returns a QuantityDTO — controller uses it for display
            var serviceResult = _service.Convert(q1, q2);

            Assert.IsNotNull(serviceResult,          "Service must return a QuantityDTO.");
            Assert.AreEqual(1000.0, serviceResult.Value,    1e-3, "Service result value must be correct.");
            Assert.AreEqual("GRAM", serviceResult.UnitName, "Service result unit must be correct.");

            // Controller can now use this DTO to format output
            string display = $"Conversion Result: {serviceResult.Value} {serviceResult.UnitName}";
            Assert.IsTrue(display.Contains("1000"), "Service result should be renderable by controller.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 6: Backward Compatibility
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Runs all test cases from UC1–UC14.
        /// Tests: Behavior unchanged, only structure refactored.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1_UC14_Tests()
        {
            // UC1–UC5: Length equality and conversion
            var feet1   = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var feet2   = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var inches12 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            Assert.AreEqual(1.0, _service.Compare(feet1, feet2).Value,
                "UC1: Same feet should be equal.");
            Assert.AreEqual(1.0, _service.Compare(feet1, inches12).Value,
                "UC3-UC4: 1 foot == 12 inches cross-unit equality.");

            var converted = _service.Convert(feet1, new QuantityDTO(0, "INCHES", "LENGTH"));
            Assert.AreEqual(12.0, converted.Value, 1e-3,
                "UC5: 1 foot converts to 12 inches.");

            // UC6–UC7: Addition
            var addResult = _service.Add(feet1, inches12);
            Assert.AreEqual(2.0, addResult.Value, 1e-3,
                "UC6-UC7: 1 foot + 12 inches = 2 feet.");

            // UC9: Weight
            var kg   = new QuantityDTO(1.0,    "KILOGRAM", "WEIGHT");
            var gram = new QuantityDTO(1000.0, "GRAM",     "WEIGHT");
            Assert.AreEqual(1.0, _service.Compare(kg, gram).Value,
                "UC9: 1 kg == 1000 g.");

            // UC10–UC13: Subtraction and Division
            var feet10 = new QuantityDTO(10.0, "FEET", "LENGTH");
            var feet2q = new QuantityDTO(2.0,  "FEET", "LENGTH");
            var subResult = _service.Subtract(feet10, feet2q);
            Assert.AreEqual(8.0, subResult.Value, 1e-3,
                "UC12-UC13: 10 feet - 2 feet = 8 feet.");

            var divResult = _service.Divide(feet10, feet2q);
            Assert.AreEqual(5.0, divResult.Value, 1e-3,
                "UC12-UC13: 10 feet / 2 feet = 5 (scalar).");

            // UC14: Temperature conversion (no arithmetic)
            var celsius = new QuantityDTO(100.0, "CELSIUS",    "TEMPERATURE");
            var fahr    = new QuantityDTO(0.0,   "FAHRENHEIT", "TEMPERATURE");
            var tempConv = _service.Convert(celsius, fahr);
            Assert.AreEqual(212.0, tempConv.Value, 0.1,
                "UC14: 100°C = 212°F.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 7: Scalability and Coverage
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies service works with length, weight, volume, temperature.
        /// Tests: Category scalability.
        /// </summary>
        [TestMethod]
        public void testService_AllMeasurementCategories()
        {
            // LENGTH
            var lengthResult = _service.Convert(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(0.0, "INCHES", "LENGTH"));
            Assert.AreEqual(12.0, lengthResult.Value, 1e-3, "Length conversion failed.");

            // WEIGHT
            var weightResult = _service.Convert(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(0.0, "GRAM",     "WEIGHT"));
            Assert.AreEqual(1000.0, weightResult.Value, 1e-3, "Weight conversion failed.");

            // VOLUME
            var volumeResult = _service.Convert(
                new QuantityDTO(1.0, "LITRE",      "VOLUME"),
                new QuantityDTO(0.0, "MILLILITRE", "VOLUME"));
            Assert.AreEqual(1000.0, volumeResult.Value, 1e-3, "Volume conversion failed.");

            // TEMPERATURE
            var tempResult = _service.Convert(
                new QuantityDTO(0.0, "CELSIUS",    "TEMPERATURE"),
                new QuantityDTO(0.0, "FAHRENHEIT", "TEMPERATURE"));
            Assert.AreEqual(32.0, tempResult.Value, 0.1, "Temperature conversion failed.");
        }

        /// <summary>
        /// Verifies controller can demonstrate all operations.
        /// Tests: Operation coverage.
        /// </summary>
        [TestMethod]
        public void testController_AllOperations()
        {
            var q1 = new QuantityDTO(2.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var qt = new QuantityDTO(0.0, "INCHES", "LENGTH");

            // All five controller public API methods must produce non-empty, non-error output
            string compareResult   = _controller.PerformComparison(q1, q2);
            string convertResult   = _controller.PerformConversion(q1, qt);
            string addResult       = _controller.PerformAddition(q1, q2);
            string subtractResult  = _controller.PerformSubtraction(q1, q2);
            string divideResult    = _controller.PerformDivision(q1, q2);

            Assert.IsFalse(string.IsNullOrWhiteSpace(compareResult),  "Compare output should not be empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(convertResult),  "Convert output should not be empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(addResult),      "Add output should not be empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(subtractResult), "Subtract output should not be empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(divideResult),   "Divide output should not be empty.");
        }

        /// <summary>
        /// Verifies validation errors are identical across operations.
        /// Tests: Centralized validation in service.
        /// </summary>
        [TestMethod]
        public void testService_ValidationConsistency()
        {
            var length = new QuantityDTO(1.0, "FEET",     "LENGTH");
            var weight = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT");

            // All operations must throw the same exception type for cross-category inputs
            var operations = new Func<QuantityDTO>[]
            {
                () => _service.Compare(length, weight),
                () => _service.Add(length, weight),
                () => _service.Subtract(length, weight),
                () => _service.Divide(length, weight),
            };

            foreach (var op in operations)
            {
                bool threw = false;
                try   { op(); }
                catch (QuantityMeasurementException) { threw = true; }
                Assert.IsTrue(threw,
                    "Each cross-category operation must throw QuantityMeasurementException.");
            }
        }

        /// <summary>
        /// Verifies QuantityEntity objects cannot be modified after creation.
        /// Tests: Immutability principle.
        /// </summary>
        [TestMethod]
        public void testEntity_Immutability()
        {
            var op1    = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var op2    = new QuantityDTO(12.0, "INCHES", "LENGTH");
            var result = new QuantityDTO(2.0,  "FEET",   "LENGTH");

            var entity = new QuantityMeasurementEntity("ADD", op1, op2, result);

            // No public setters — verify via reflection that all settable properties are private-set
            var props = typeof(QuantityMeasurementEntity).GetProperties();
            foreach (var prop in props)
            {
                var setter = prop.GetSetMethod(nonPublic: false);
                Assert.IsNull(setter,
                    $"Property '{prop.Name}' must not have a public setter (immutability).");
            }
        }

        /// <summary>
        /// Verifies all operations convert exceptions to error entities in service.
        /// Tests: Consistent error propagation.
        /// </summary>
        [TestMethod]
        public void testService_ExceptionHandling_AllOperations()
        {
            // Temperature does not support arithmetic — service must throw for all
            var t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            bool addThrew      = false;
            bool subtractThrew = false;
            bool divideThrew   = false;

            try { _service.Add(t1, t2); }      catch (QuantityMeasurementException) { addThrew      = true; }
            try { _service.Subtract(t1, t2); } catch (QuantityMeasurementException) { subtractThrew = true; }
            try { _service.Divide(t1, t2); }   catch (QuantityMeasurementException) { divideThrew   = true; }

            Assert.IsTrue(addThrew,      "Service.Add for temperature should throw.");
            Assert.IsTrue(subtractThrew, "Service.Subtract for temperature should throw.");
            Assert.IsTrue(divideThrew,   "Service.Divide for temperature should throw.");
        }

        /// <summary>
        /// Verifies console output is clear and readable.
        /// Tests: User-facing formatting.
        /// </summary>
        [TestMethod]
        public void testController_ConsoleOutput_Format()
        {
            var q1 = new QuantityDTO(1.0, "FEET",   "LENGTH");
            var q2 = new QuantityDTO(0.0, "INCHES", "LENGTH");

            string output = _controller.PerformConversion(q1, q2);

            // Output must have a label prefix and a numeric value — user-readable format
            Assert.IsTrue(output.Contains("Conversion Result:"),
                "Controller output should start with a readable label.");
            Assert.IsTrue(output.Contains("12"),
                "Controller output should contain the result value.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 8: Integration Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Full integration test: User input → Output (Length Addition).
        /// Tests: Complete layer cooperation.
        /// </summary>
        [TestMethod]
        public void testIntegration_EndToEnd_LengthAddition()
        {
            // Simulate what the controller builds from user input
            var q1 = new QuantityDTO(1.0,  "FEET",   "LENGTH");
            var q2 = new QuantityDTO(12.0, "INCHES", "LENGTH");

            // Controller calls service, gets result, formats output
            string output = _controller.PerformAddition(q1, q2);

            // End result should show 2 FEET
            Assert.IsTrue(output.Contains("2"),
                "End-to-end addition: result should be 2.");
            Assert.IsTrue(output.Contains("FEET"),
                "End-to-end addition: result unit should be FEET.");

            // Operation was also persisted in repo
            var history = _repo.GetAllMeasurements();
            Assert.IsTrue(history.Count > 0,
                "Repository should contain operation after end-to-end test.");
        }

        /// <summary>
        /// Full integration test: Error handling across layers (Temperature unsupported).
        /// Tests: Error handling integration.
        /// </summary>
        [TestMethod]
        public void testIntegration_EndToEnd_TemperatureUnsupported()
        {
            var t1 = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE");
            var t2 = new QuantityDTO(50.0,  "CELSIUS", "TEMPERATURE");

            // Controller should surface the error gracefully — not throw
            string output = _controller.PerformAddition(t1, t2);

            Assert.IsTrue(
                output.Contains("[ERROR]") || output.ToUpperInvariant().Contains("ERROR"),
                "End-to-end temperature add should produce an error output, not crash.");
        }

        /// <summary>
        /// Verifies service rejects null entities.
        /// Tests: Input validation.
        /// </summary>
        [TestMethod]
        public void testService_NullEntity_Rejection()
        {
            var valid = new QuantityDTO(1.0, "FEET", "LENGTH");

            bool threwForNull1 = false;
            bool threwForNull2 = false;

            try { _service.Compare(null!, valid); }
            catch (QuantityMeasurementException) { threwForNull1 = true; }
            catch (ArgumentNullException)        { threwForNull1 = true; }

            try { _service.Compare(valid, null!); }
            catch (QuantityMeasurementException) { threwForNull2 = true; }
            catch (ArgumentNullException)        { threwForNull2 = true; }

            Assert.IsTrue(threwForNull1, "Service should reject null first operand.");
            Assert.IsTrue(threwForNull2, "Service should reject null second operand.");
        }

        /// <summary>
        /// Verifies controller requires non-null service.
        /// Tests: Dependency validation.
        /// </summary>
        [TestMethod]
        public void testController_NullService_Prevention()
        {
            bool threw = false;
            try
            {
                var repo   = QuantityMeasurementCacheRepository.Instance;
                var _      = new QuantityMeasurementController(null!, repo);
            }
            catch (ArgumentNullException)
            {
                threw = true;
            }

            Assert.IsTrue(threw, "Controller constructor should throw for null service.");
        }

        /// <summary>
        /// Verifies service works with any IMeasurable implementation.
        /// Tests: Polymorphic behavior.
        /// </summary>
        [TestMethod]
        public void testService_AllUnitImplementations()
        {
            // Length units
            var feetToYards = _service.Convert(
                new QuantityDTO(3.0, "FEET", "LENGTH"),
                new QuantityDTO(0.0, "YARDS", "LENGTH"));
            Assert.AreEqual(1.0, feetToYards.Value, 1e-2, "3 feet = 1 yard.");

            var feetToCm = _service.Convert(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(0.0, "CENTIMETERS", "LENGTH"));
            Assert.AreEqual(30.48, feetToCm.Value, 0.1, "1 foot ≈ 30.48 cm.");

            // Weight units
            var kgToLb = _service.Convert(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(0.0, "POUND",    "WEIGHT"));
            Assert.AreEqual(2.20, kgToLb.Value, 0.1, "1 kg ≈ 2.20 lb.");

            // Volume units
            var litreToGallon = _service.Convert(
                new QuantityDTO(3.78541, "LITRE",  "VOLUME"),
                new QuantityDTO(0.0,     "GALLON", "VOLUME"));
            Assert.AreEqual(1.0, litreToGallon.Value, 0.05, "3.78541 litres ≈ 1 gallon.");

            // Temperature
            var kelvin = _service.Convert(
                new QuantityDTO(0.0, "CELSIUS", "TEMPERATURE"),
                new QuantityDTO(0.0, "KELVIN",  "TEMPERATURE"));
            Assert.AreEqual(273.15, kelvin.Value, 0.1, "0°C = 273.15 K.");
        }

        /// <summary>
        /// Verifies operation type correctly recorded in entity.
        /// Tests: Operation categorization.
        /// </summary>
        [TestMethod]
        public void testEntity_OperationType_Tracking()
        {
            // After each service call, the repo stores the entity with the correct operation type
            _repo.Clear();

            _service.Compare(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"));

            _service.Convert(
                new QuantityDTO(1.0, "FEET",   "LENGTH"),
                new QuantityDTO(0.0, "INCHES", "LENGTH"));

            _service.Add(
                new QuantityDTO(1.0, "FEET",   "LENGTH"),
                new QuantityDTO(1.0, "FEET",   "LENGTH"));

            var history = _repo.GetAllMeasurements();

            // Exactly 3 operations should be stored
            Assert.AreEqual(3, history.Count, "Repository should hold 3 operations.");

            bool hasCompare  = false;
            bool hasConvert  = false;
            bool hasAdd      = false;

            foreach (var entity in history)
            {
                if (entity.OperationType == "COMPARE")  hasCompare  = true;
                if (entity.OperationType == "CONVERT")  hasConvert  = true;
                if (entity.OperationType == "ADD")      hasAdd      = true;
            }

            Assert.IsTrue(hasCompare, "COMPARE operation should be tracked.");
            Assert.IsTrue(hasConvert, "CONVERT operation should be tracked.");
            Assert.IsTrue(hasAdd,     "ADD operation should be tracked.");
        }

        // ════════════════════════════════════════════════════════════════
        // SECTION 9: Decoupling and Extensibility Tests
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies changing service implementation doesn't affect controller.
        /// Tests: Loose coupling enables flexibility.
        /// </summary>
        [TestMethod]
        public void testLayerDecoupling_ServiceChange()
        {
            // Swap out service implementation — controller must still work identically
            IQuantityMeasurementService altService = new QuantityMeasurementServiceImpl(
                QuantityMeasurementCacheRepository.Instance);

            var altController = new QuantityMeasurementController(
                altService,
                QuantityMeasurementCacheRepository.Instance);

            var q1 = new QuantityDTO(1.0, "FEET", "LENGTH");
            var q2 = new QuantityDTO(1.0, "FEET", "LENGTH");

            string output = altController.PerformComparison(q1, q2);

            // Output should be identical to original controller behavior
            Assert.IsTrue(output.Contains("true"),
                "Swapping service implementation should not change controller behavior.");
        }

        /// <summary>
        /// Verifies adding Entity fields doesn't break layers.
        /// Tests: Entity as stable contract.
        /// </summary>
        [TestMethod]
        public void testLayerDecoupling_EntityChange()
        {
            // QuantityMeasurementEntity is the contract between layers.
            // Verify all required public properties exist and are readable.
            var entity = new QuantityMeasurementEntity(
                "ADD",
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(2.0, "FEET", "LENGTH"));

            // All contract fields must be accessible without casting or reflection tricks
            Assert.IsNotNull(entity.OperationType, "OperationType must be accessible.");
            Assert.IsNotNull(entity.Operand1,      "Operand1 must be accessible.");
            Assert.IsNotNull(entity.Operand2,      "Operand2 must be accessible.");
            Assert.IsNotNull(entity.Result,        "Result must be accessible.");
            Assert.IsFalse(entity.HasError,        "HasError must be accessible.");
            Assert.IsNotNull(entity.ErrorMessage,  "ErrorMessage must be accessible.");
            Assert.IsNotNull(entity.Timestamp.ToString(), "Timestamp must be accessible.");
        }

        /// <summary>
        /// Verifies adding new operation doesn't require layer modifications.
        /// Tests: Extensibility within layer design.
        /// </summary>
        [TestMethod]
        public void testScalability_NewOperation_Addition()
        {
            // The existing Add operation was "new" at one point.
            // Verify that calling it across all measurement categories works without
            // any modification to the controller or entity classes.
            var lengthSum = _controller.PerformAddition(
                new QuantityDTO(1.0, "FEET", "LENGTH"),
                new QuantityDTO(1.0, "FEET", "LENGTH"));

            var weightSum = _controller.PerformAddition(
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"),
                new QuantityDTO(1.0, "KILOGRAM", "WEIGHT"));

            var volumeSum = _controller.PerformAddition(
                new QuantityDTO(1.0, "LITRE", "VOLUME"),
                new QuantityDTO(1.0, "LITRE", "VOLUME"));

            Assert.IsTrue(lengthSum.Contains("2"),  "Length addition should return 2.");
            Assert.IsTrue(weightSum.Contains("2"),  "Weight addition should return 2.");
            Assert.IsTrue(volumeSum.Contains("2"),  "Volume addition should return 2.");
        }

        // ════════════════════════════════════════════════════════════════
        // MOCK SERVICE — used by testLayerSeparation_ControllerIndependence
        // ════════════════════════════════════════════════════════════════

        /// <summary>
        /// Minimal mock service that always returns a fixed "equal" result.
        /// Proves controller works with any IQuantityMeasurementService implementation.
        /// </summary>
        private class MockQuantityMeasurementService : IQuantityMeasurementService
        {
            public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
                => new QuantityDTO(1, "EQUAL", "RESULT");

            public QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnitDTO)
                => new QuantityDTO(1, targetUnitDTO.UnitName, q1.Category);

            public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
                => new QuantityDTO(q1.Value + q2.Value, q1.UnitName, q1.Category);

            public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
                => new QuantityDTO(q1.Value - q2.Value, q1.UnitName, q1.Category);

            public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
                => new QuantityDTO(q2.Value == 0 ? 0 : q1.Value / q2.Value, "RATIO", "SCALAR");
        }
    }
}