namespace AutoCad_pot.UnitTests
{
    using System;
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

        [TestCase(20, "Геттер некорректно возвращает значение.",
            TestName = "Позитивный тест геттера свойства MinValue.")]
        public void Test_MinValue_Get_CorrectValue(double expectedMin, string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.MinValue = expectedMin;
            var actual = tmpParameter.MinValue;
            Assert.AreEqual(expectedMin, actual, message);
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

        [TestCase(1000,  "Сеттер некорректно записал данные.",
            TestName =
                "Позитивный тест сеттера свойства Value: "
                + "Задать значение, превышающее максимально допустимое.")]
        [TestCase(-10,  "Сеттер некорректно записал данные.",
            TestName =
                "Позитивный тест сеттера свойства Value: "
                + "Задать значение, меньше минимально допустимое.")]
        public void Test_Value_Set_CorrectValue(double value,
            string message)
        {
            var parameter = Parameter;
            var expectedMessage =
                $" is not in the range {parameter.MinValue}-{parameter.MaxValue}.\n";

            // Assert & Act
            var exception = Assert.Throws<ArgumentException>(
                () => parameter.Value = value);
            Assert.AreEqual(expectedMessage, exception.Message);
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
            var expectedMessage =
                $" is not in the range {tmpParameter.MinValue}-{tmpParameter.MaxValue}.\n";

            var exception = Assert.Throws<ArgumentException>(
                () => tmpParameter.Value = value);
            Assert.AreEqual(expectedMessage, exception.Message);

        }

        [TestCase(
            30,
            "Ошибка при сравнении.",
            TestName = "Негативный тест метода Validate.")]
        public void Test_Parameter_EqualMinMax(
            double value,
            string message)
        {
            var tmpParameter = Parameter;
            var excpectedMessage =
                $": Main parameter entered incorrectly.\n";

            var exception = Assert.Throws<ArgumentException>(
                () =>
                {
                    tmpParameter.MinValue = 0;
                    tmpParameter.MaxValue = 0;
                    tmpParameter.Value = value;
                });
            Assert.AreEqual(excpectedMessage, exception.Message);

        }

        [TestCase(
            "Произошла ошибка при сравнении объектов.",
            TestName = "Позитивный тест метода Equals.")]
        public void Test_EqualsParameter_CorrectValue(string message)
        {
            var expected = 30;
            var tmpParameter = Parameter;
            tmpParameter.Value = expected;
            var actual = tmpParameter.Value;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}