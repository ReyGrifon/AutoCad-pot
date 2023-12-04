namespace AutoCad_pot.UnitTests
{
    using AutoCad_pot.Model;
    using NUnit.Framework;

    /// <summary>
    /// Тестирование класса Parameter.
    /// </summary>
    [TestFixture(Description = "Модульные тесты класса Parameter.")]
    public class ParameterTests
    {
        private const double MinValue = 10;
        private const double MaxValue = 99;
        private const double Value = 30;

        private Parameter _parameter;

        /// <summary>
        /// Предустановка параметра.
        /// </summary>
        [SetUp]
        public void CreateTestParameter()
        {
            _parameter = new Parameter(MinValue, MaxValue, Value);
        }

        [TestCase(
            MinValue,
            "Сеттер некорректно записал данные.",
            TestName = "Позитивный тест сеттера свойства MinValue: "
                       + "Задать значение при создании объекта.")]
        public void Test_MinValue_Set_CorrectValue(double expectedValue, string message)
        {
            var parameter = new Parameter(expectedValue, MaxValue, Value);
            Assert.AreEqual(expectedValue, parameter.MinValue, message);
        }

        [TestCase(MinValue, "Геттер некорректно возвращает значение.",
            TestName = "Позитивный тест геттера свойства MinValue.")]
        public void Test_MinValue_Get_CorrectValue(double expectedMin, string message)
        {
            var parameter = new Parameter(expectedMin, MaxValue, Value);
            var actual = parameter.MinValue;
            Assert.AreEqual(expectedMin, actual, message);
        }

        [TestCase(MaxValue, MaxValue,
            "Сеттер некорректно записал данные.",
            TestName = "Позитивный тест сеттера свойства MaxValue: "
                       + "Задать максимальное значение объекта.")]
        [TestCase(-10, MinValue,
            "Сеттер записал значение меньше допустимого.",
            TestName = "Позитивный тест сеттера свойства MaxValue: "
                       + "Попытка присвоить значение меньше минимально допустимого.")]
        public void Test_MaxValue_Set_WrongValue(double value, double expectedValue,
            string message)
        {
            _parameter.MaxValue = value;
            Assert.AreEqual(expectedValue, _parameter.MaxValue, message);
        }

        [TestCase(100, "Геттер возвращает некорректное значение.",
            TestName = "Позитивный тест геттера свойства MaxValue.")]
        public void Test_MaxValue_Get_CorrectValue(double expectedValue, string message)
        {
            _parameter.MaxValue = expectedValue;
            var actual = _parameter.MaxValue;
            Assert.AreEqual(expectedValue, actual, message);
        }

        [TestCase(Value, Value, "Сеттер некорректно записал данные.",
            TestName = "Позитивный тест сеттера свойства Value: "
                       + "Задать новое значение.")]
        [TestCase(1000, Value, "Сеттер некорректно записал данные.",
            TestName =
                "Позитивный тест сеттера свойства Value: "
                + "Задать значение, превышающее максимально допустимое.")]
        [TestCase(-10, Value, "Сеттер некорректно записал данные.",
            TestName =
                "Позитивный тест сеттера свойства Value: "
                + "Задать значение, меньше минимально допустимое.")]
        public void Test_Value_Set_CorrectValue(double value, double expectedValue,
            string message)
        {
            _parameter.Value = value;
            Assert.AreEqual(expectedValue, _parameter.Value, message);
        }

        [TestCase(Value, "Геттер возвращает некорректное значение.",
            TestName = "Позитивный тест геттера свойства Value.")]
        public void Test_Value_Get_CorrectValue(double expectedValue, string message)
        {
            _parameter.Value = expectedValue;
            var actual = _parameter.Value;
            Assert.AreEqual(expectedValue, actual, message);
        }

        [TestCase(
            MaxValue,
            MinValue,
            MaxValue,
            "Ошибка при сравнении.",
            TestName = "Негативный тест метода Equals.")]
        public void Test_EqualsParameter_WrongValue(
            double minValue, double maxValue, double value, string message)
        {
            var actualParameter = new Parameter(minValue, maxValue, value);
            Assert.IsFalse(actualParameter.IsCorrect(Value), message);
        }

        [TestCase(
            "Произошла ошибка при сравнении объектов.",
            TestName = "Позитивный тест метода Equals.")]
        public void Test_EqualsParameter_CorrectValue(string message)
        {
            var actualParameter = new Parameter(MinValue, MaxValue, Value);
            Assert.IsTrue(actualParameter.IsCorrect(Value), message);
        }
    }
}