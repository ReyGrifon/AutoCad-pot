namespace AutoCad_pot.UnitTests
{
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

        [TestCase(MaxValue, MaxValue,
            "������ ����������� ������� ������.",
            TestName = "���������� ���� ������� �������� MaxValue: "
                       + "������ ������������ �������� �������.")]
        [TestCase(-10, MinValue,
            "������ ������� �������� ������ �����������.",
            TestName = "���������� ���� ������� �������� MaxValue: "
                       + "������� ��������� �������� ������ ���������� �����������.")]
        public void Test_MaxValue_Set_WrongValue(double value, double expectedValue,
            string message)
        {
            var tmpParameter = Parameter;
            tmpParameter.MaxValue = value;
            Assert.AreEqual(expectedValue, tmpParameter.MaxValue, message);
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

        [TestCase(Value, Value, "������ ����������� ������� ������.",
            TestName = "���������� ���� ������� �������� Value: "
                       + "������ ����� ��������.")]
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
            var tmpParameter = Parameter;
            tmpParameter.Value = value;
            Assert.AreEqual(expectedValue, tmpParameter.Value, message);
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
            Assert.Catch(tmpParameter.Validate(value), message);
            Assert.IsFalse(tmpParameter.Validate(value), message);
        }

        [TestCase(
            "��������� ������ ��� ��������� ��������.",
            TestName = "���������� ���� ������ Equals.")]
        public void Test_EqualsParameter_CorrectValue(string message)
        {
            var tmpParameter = Parameter;
            Assert.IsTrue(tmpParameter.Validate(Value), message);
        }
    }
}