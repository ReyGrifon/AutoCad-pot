namespace AutoCad_pot.Model
{
    /// <summary>
    /// Класс параметра модели.
    /// </summary>
    public class Parameter
    {
        private double _maxValue;

        private double _value;

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
        /// Свойство минимального значения.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Свойство максимального значения.
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set => _maxValue = value <= MinValue ? MinValue : value;
        }

        /// <summary>
        /// Свойство значения параметра.
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Метод проверяющий число на нахождение в границах min и max.
        /// </summary>
        /// <param name="value">проверяемое значение. </param>
        /// <returns>false, если число не находится в пределах,
        /// true в обратной ситуации. </returns>
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
