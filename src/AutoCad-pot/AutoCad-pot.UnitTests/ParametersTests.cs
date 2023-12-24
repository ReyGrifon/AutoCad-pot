namespace AutoCad_pot.UnitTests
{
    using System;
    using System.Security.Cryptography;
    using AutoCad_pot.Model;
    using NUnit.Framework;

    /// <summary>
    /// Тестирование класса Parameters
    /// </summary>
    [TestFixture(Description = "Модульные тесты класса Parameter.")]
    public class ParametersTests
    {
        /// <summary>
        /// Создание объекта параметры с минимальными параметрами.
        /// </summary>
        private Parameters Parameters => new Parameters();

        [TestCase(ParameterType.PotHeight, Parameters.MinPotHeight,
            "Метод возвращает некорректное минимальное значение параметра PotHeight.",
            TestName =
                "Тест метода GetMinValue: Получить минимальное значение параметра 'PotHeight'.")]
        public void Test_GetMinValue_CorrectValue(ParameterType parameterType,
            double expectedValue, string message)
        {
            var tmpParameters = Parameters;
            var actualValue = tmpParameters.GetMinValue(parameterType);
            Assert.AreEqual(expectedValue, actualValue, message);
        }

        [TestCase(ParameterType.PotHeight, Parameters.MaxPotHeight,
            "Метод возвращает некорректное минимальное значение параметра PotHeight.",
            TestName =
                "Тест метода GetMaxValue: Получить минимальное значение параметра 'PotHeight'.")]
        public void Test_GetMaxValue_CorrectValue(ParameterType parameterType,
            double expectedValue, string message)
        {
            var tmpParameters = Parameters;
            var actualValue = tmpParameters.GetMaxValue(parameterType);
            Assert.AreEqual(expectedValue, actualValue, message);
        }
        [TestCase(3,15, "Метод возвращает некорректное максимальное "
                       + "значение параметра HandlesHeight.",
            TestName =
            "Тест метода UpdateMaxHandlesHeight: Получить обновлённое значение"
            + " параметра 'HandlesHeight'.")]
        public void Test_UpdateMaxHandlesHeight_CorrectValue(
            double value,
            double exceptedValue,
            string message)
        {
            var tmpParameters = Parameters;
            tmpParameters.SetValue(ParameterType.HandlesThickness, value);
            var actualValue = tmpParameters.GetMaxValue(ParameterType.HandlesHeight);
            Assert.AreEqual(exceptedValue, actualValue, message);
        }

        [TestCase(ParameterType.PotHeight, 170,
            "Метод задает некорректное значение параметра 'PotHeight'.")]
        public void Test_SetValue_CorrectValue(ParameterType parameterType,
            double expectedValue, string message)
        {
            var tmpParameters = Parameters;
            tmpParameters.SetValue(parameterType, expectedValue);
            var actualValue = tmpParameters.GetValue(parameterType);
            Assert.AreEqual(expectedValue, actualValue, message);
        }

        [TestCase(ParameterType.HandlesHeight,
            10,
            2.5,
            "Метод задает некорректное значение параметра 'PotHeight'.")]
        public void Test_SetValue_ParameterType_HandlesHeightMin(
            ParameterType parameterType,
            double HandlessHeightValue,
            double exceptedValue,
            string message)
        {
            var tmpParameters = Parameters;
            tmpParameters.SetValue(parameterType, HandlessHeightValue);
            var actualValue = tmpParameters.GetMinValue(ParameterType.HandlesThickness);
            Assert.AreEqual(exceptedValue, actualValue, message);
        }

        [TestCase(ParameterType.HandlesHeight,
            100,
            0,
            "Метод высчитывает некорректное значение параметра 'HandlesHeight'.")]
        public void Test_SetValue_ParameterType_UpdateErrorLimits(
            ParameterType parameterType,
            double HandlessHeightValue,
            double exceptedValue,
            string message)
        {
            var tmpParameters = Parameters;
            var exception = Assert.Throws<AggregateException>(
                () =>
                {
                    tmpParameters.SetValue(parameterType, HandlessHeightValue);
                    var actualValue = tmpParameters.GetMaxValue(ParameterType.HandlesThickness);
                    Assert.AreEqual(exceptedValue, actualValue, message);
                });
        }

        [TestCase(12.5, "Ме")]
        public void Test_UpdateHandlessHeightLimit(double exceptedValue, string message)
        {
            var tmpParameters = Parameters;
            tmpParameters.UpdateHandlesHeightLimit();
            var actualValue = tmpParameters.GetMaxValue(ParameterType.HandlesHeight);
            Assert.AreEqual(exceptedValue, actualValue, message);
        }

        [TestCase(ParameterType.PotHeight, Parameters.MinPotHeight,
            "Метод возвращает некорректное текущее значение параметра PotHeight.",
            TestName =
                "Тест метода GetValue: Получить текущее значение параметра 'PotHeight'.")]
        public void Test_GetValue_CorrectValue(ParameterType parameterType,
            double expectedValue, string message)
        {
            var tmpParameters = Parameters;
            var actualValue = tmpParameters.GetValue(parameterType);
            Assert.AreEqual(expectedValue, actualValue, message);
        }

        [Test(Description = "Тест авто свойства HandleType")]
        public void HandleTypeProperty()
        {
            // Arrange
            var parameters = Parameters;
            var expected = false;

            // Act
            parameters.HandleType = expected;
            var actual = parameters.HandleType;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(ParameterType.PotHeight, 2000,
            "Метод задает некорректное значение параметра 'PotHeight'.")]
        public void SetValueFailureNeckHeightOne(ParameterType parameterType,
            double unexpectedValue, string message)
        {
            // Arrange
            var parameters = Parameters;
            var parameter = parameters[parameterType];
            var expected = 8;
            var expectedMessage =
                $" is not in the range {parameter.MinValue}-{parameter.MaxValue}.\n";

            // Assert & Act
            var exception = Assert.Throws<AggregateException>(
                () => parameters.SetValue(parameterType, unexpectedValue));

            Assert.AreEqual(expectedMessage, exception.InnerExceptions[0].Message);
        }

        [TestCase(ParameterType.HandlesThickness, 2000,
            "Метод задает некорректное значение параметра 'PotHeight'.")]
        public void SetValue(ParameterType parameterType,
            double unexpectedValue, string message)
        {
            var parameters = Parameters;
            var expected = 0;
            var exception = Assert.Throws<AggregateException>(
                () =>
                {
                    parameters.SetValue(parameterType, unexpectedValue);
                    var actual = parameters[ParameterType.HandlesHeight].MaxValue;
                    Assert.AreEqual(expected, actual);
                });
        }
    }
}