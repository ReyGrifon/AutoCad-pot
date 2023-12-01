namespace AutoCad_pot.Model
{
    /// <summary>
    /// Класс параметра модели.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Минимальное значение параметра.
        /// </summary>
        private double _minValue;

        private double _maxValue;

        private double _value;

        /// <summary>
        /// Конструктор пустого класса.
        /// </summary>
        public Parameter()
        {
            // Содержит пустые параметры.
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="minValue">Минимальное значение параметра.</param>
        /// <param name="maxValue">Максимальное значение параметра.</param>
        /// <param name="value">Значения параметра.</param>
        public Parameter(double minValue, double maxValue, double value)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        /// <summary>
        /// Свойство минимального знаачения.
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        /// <summary>
        /// Свойство максимального значения.
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        /// <summary>
        /// Свойство значения параметра.
        /// </summary>
        public double Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Метод проверяющий число на верность.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsCorrect(double value)
        {
            if (MinValue > value || value > MaxValue)
            {
                return false;
            }

            return true;
        }
    }
}
