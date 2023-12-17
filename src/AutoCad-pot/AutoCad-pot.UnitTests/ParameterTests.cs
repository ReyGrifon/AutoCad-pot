namespace AutoCad_pot.UnitTests
{
    using System;
    using AutoCad_pot.Model;
    using NUnit.Framework;

    /// <summary>
    /// ������������ ������ Parameter.
    /// </summary>
    [TestFixture(Description = "��������� ����� ������ Parameter.")]
    public class ParameterTests
    {
        private const double MinValue = 10;
        private const double MaxValue = 99;
        private const double Value = 30;

        private Parameter Parameter => new Parameter(MinValue, MaxValue, Value);

        [TestCase(
            MinValue,
            "������ ����������� ������� ������.",
            TestName = "���������� ���� ������� �������� MinValue: "
                       + "������ �������� ��� �������� �������.")]
        public void Test_MinValue_Set_CorrectValue(double expectedValue, string message)
        {
            var parameter = new Parameter(expectedValue, MaxValue, Value);
            Assert.AreEqual(expectedValue, parameter.MinValue, message);
        }

        [TestCase(MinValue, "������ ����������� ���������� ��������.",
            TestName = "���������� ���� ������� �������� MinValue.")]
        public void Test_MinValue_Get_CorrectValue(double expectedMin, string message)
        {
            var parameter = new Parameter(expectedMin, MaxValue, Value);
            var actual = parameter.MinValue;
            Assert.AreEqual(expectedMin, actual, message);
        }

        [TestCase(100, "������ ���������� ������������ ��������.",
            TestName = "���������� ���� ������� �������� MaxValue.")]
        public void Test_MaxValue_Get_CorrectValue(double expectedValue, string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.MaxValue = expectedValue;
            var actual = tmpParameter.MaxValue;
            Assert.AreEqual(expectedValue, actual, message);
        }

        [TestCase(1000, Value, "������ ����������� ������� ������.",
            TestName =
                "���������� ���� ������� �������� Value: "
                + "������ ��������, ����������� ����������� ����������.")]
        [TestCase(-10, Value, "������ ����������� ������� ������.",
            TestName =
                "���������� ���� ������� �������� Value: "
                + "������ ��������, ������ ���������� ����������.")]
        public void Test_Value_Set_CorrectValue(double value, double expectedValue,
            string message)
        {
            var parameter = Parameter;
            var excpectedMessage =
                $" is not in the range {parameter.MinValue}-{parameter.MaxValue}.\n";

            // Assert & Act
            var exception = Assert.Throws<ArgumentException>(
                () => parameter.Value = value);
            Assert.AreEqual(excpectedMessage, exception.Message);
        }

        [TestCase(Value, "������ ���������� ������������ ��������.",
            TestName = "���������� ���� ������� �������� Value.")]
        public void Test_Value_Get_CorrectValue(double expectedValue, string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.Value = expectedValue;
            var actual = tmpParameter.Value;
            Assert.AreEqual(expectedValue, actual, message);
        }

        [TestCase(
            9999,
            "������ ��� ���������.",
            TestName = "���������� ���� ������ Equals.")]
        public void Test_EqualsParameter_WrongValue(
            double value,
            string message)
        {
            var tmpParameter = Parameter;
            var excpectedMessage =
                $" is not in the range {tmpParameter.MinValue}-{tmpParameter.MaxValue}.\n";

            var exception = Assert.Throws<ArgumentException>(
                () => tmpParameter.Value = value);
            Assert.AreEqual(excpectedMessage, exception.Message);

        }

        [TestCase(
            30,
            "������ ��� ���������.",
            TestName = "���������� ���� ������ Equals.")]
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
            "��������� ������ ��� ��������� ��������.",
            TestName = "���������� ���� ������ Equals.")]
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