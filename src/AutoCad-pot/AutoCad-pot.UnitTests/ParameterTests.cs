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

        private Parameter _parameter;

        /// <summary>
        /// ������������� ���������.
        /// </summary>
        [SetUp]
        public void CreateTestParameter()
        {
            _parameter = new Parameter(MinValue, MaxValue, Value);
        }

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
            _parameter.MaxValue = value;
            Assert.AreEqual(expectedValue, _parameter.MaxValue, message);
        }

        [TestCase(100, "������ ���������� ������������ ��������.",
            TestName = "���������� ���� ������� �������� MaxValue.")]
        public void Test_MaxValue_Get_CorrectValue(double expectedValue, string message)
        {
            _parameter.MaxValue = expectedValue;
            var actual = _parameter.MaxValue;
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
            _parameter.Value = value;
            Assert.AreEqual(expectedValue, _parameter.Value, message);
        }

        [TestCase(Value, "������ ���������� ������������ ��������.",
            TestName = "���������� ���� ������� �������� Value.")]
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
            "������ ��� ���������.",
            TestName = "���������� ���� ������ Equals.")]
        public void Test_EqualsParameter_WrongValue(
            double minValue, double maxValue, double value, string message)
        {
            var actualParameter = new Parameter(minValue, maxValue, value);
            Assert.IsFalse(actualParameter.IsCorrect(Value), message);
        }

        [TestCase(
            "��������� ������ ��� ��������� ��������.",
            TestName = "���������� ���� ������ Equals.")]
        public void Test_EqualsParameter_CorrectValue(string message)
        {
            var actualParameter = new Parameter(MinValue, MaxValue, Value);
            Assert.IsTrue(actualParameter.IsCorrect(Value), message);
        }
    }
}