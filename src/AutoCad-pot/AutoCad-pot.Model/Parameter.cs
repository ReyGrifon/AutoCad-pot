namespace AutoCad_pot.Model
{
    using System;

    /// <summary>
    /// Класс параметра модели.
    /// </summary>
    public class Parameter
    {

        /// <summary>
        /// Значение параметра.
        /// </summary>
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
        public double MaxValue { get; set; }

        /// <summary>
        /// Свойство значения параметра.
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                Validate(value);
                _value = value;
            }
        }

        /// <summary>
        /// Метод проверяющий число на нахождение в границах min и max.
        /// </summary>
        /// <param name="value">проверяемое значение. </param>
        /// <returns>false, если число не находится в пределах,
        /// true в обратной ситуации. </returns>
        private void Validate(double value)
        {
            if (MinValue == MaxValue)
            {
                throw new ArgumentException(
                    $": Main parameter entered incorrectly.\n");
            }

            if (MinValue > value || value > MaxValue)
            {
                throw new ArgumentException(
                    $" is not in the range {MinValue}-{MaxValue}.\n");
            }
        }
    }
}
