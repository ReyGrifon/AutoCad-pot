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

        private Parameter Parameter => new Parameter(MinValue, MaxValue, Value);

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
            var tmpParameter = Parameter;
            tmpParameter.MaxValue = value;
            Assert.AreEqual(expectedValue, tmpParameter.MaxValue, message);
        }

        [TestCase(100, "Геттер возвращает некорректное значение.",
            TestName = "Позитивный тест геттера свойства MaxValue.")]
        public void Test_MaxValue_Get_CorrectValue(double expectedValue, string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.MaxValue = expectedValue;
            var actual = tmpParameter.MaxValue;
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
            var tmpParameter = Parameter;
            tmpParameter.Value = value;
            Assert.AreEqual(expectedValue, tmpParameter.Value, message);
        }

        [TestCase(Value, "Геттер возвращает некорректное значение.",
            TestName = "Позитивный тест геттера свойства Value.")]
        public void Test_Value_Get_CorrectValue(double expectedValue, string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.Value = expectedValue;
            var actual = tmpParameter.Value;
            Assert.AreEqual(expectedValue, actual, message);
        }

        [TestCase(
            9999,
            "Ошибка при сравнении.",
            TestName = "Негативный тест метода Equals.")]
        public void Test_EqualsParameter_WrongValue(
            double value,
            string message)
        {
            var tmpParameter = Parameter;
            Assert.Catch(tmpParameter.Validate(value), message);
            Assert.IsFalse(tmpParameter.Validate(value), message);
        }

        [TestCase(
            "Произошла ошибка при сравнении объектов.",
            TestName = "Позитивный тест метода Equals.")]
        public void Test_EqualsParameter_CorrectValue(string message)
        {
            var tmpParameter = Parameter;
            Assert.IsTrue(tmpParameter.Validate(Value), message);
        }
    }
}