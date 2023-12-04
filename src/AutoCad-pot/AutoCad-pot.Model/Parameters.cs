namespace AutoCad_pot.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Общий класс параметров модели.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Минимальная высота кастрюли.
        /// </summary>
        public const double MinPotHeight = 150;

        /// <summary>
        /// Максимальная высота кастрюли.
        /// </summary>
        public const double MaxPotHeight = 300;

        /// <summary>
        /// Минимальный диаметр кастрюли.
        /// </summary>
        public const double MinPotDiameter = 150;

        /// <summary>
        /// Максимальный диаметр кастрюли.
        /// </summary>
        public const double MaxPotDiameter = 200;

        /// <summary>
        /// Минимальная толщина дна.
        /// </summary>
        public const double MinBottomThickness = 1;

        /// <summary>
        /// Максимальная толщина данных.
        /// </summary>
        public const double MaxBottomThickness = 10;

        /// <summary>
        /// Минимальная толщина стенок.
        /// </summary>
        public const double MinWallThickness = 0.5;

        /// <summary>
        /// Максимальная толщина стенок.
        /// </summary>
        public const double MaxWallThickness = 3;

        /// <summary>
        /// Минимальная толщина ручек.
        /// </summary>
        public const double MinHandlesThickness = 3;

        /// <summary>
        /// Максимальная толщина ручек.
        /// </summary>
        public const double MaxHandlesThickness = 10;

        /// <summary>
        /// Минимальная высота ручек.
        /// </summary>
        public const double MinHandlesHeight = 1.5;

        /// <summary>
        /// Максимальная высота ручек.
        /// </summary>
        public const double MaxHandlesHeight = 2;

        /// <summary>
        /// Словарь с параметрам модели.
        /// </summary>
        private readonly Dictionary<ParameterType, Parameter> _parameters;

        /// <summary>
        /// Конструктор словаря с параметрами.
        /// </summary>
        public Parameters()
        {
            _parameters =
               new Dictionary<ParameterType, Parameter>
               {
                    {
                        ParameterType.PotHeight, new Parameter(
                            MinPotHeight,
                            MaxPotHeight,
                            MinPotHeight)
                    },
                    {
                        ParameterType.PotDiameter, new Parameter(
                            MinPotDiameter,
                            MaxPotDiameter,
                            MinPotDiameter)
                    },
                    {
                        ParameterType.BottomThickness, new Parameter(
                            MinBottomThickness,
                            MaxBottomThickness,
                            MinBottomThickness)
                    },
                    {
                        ParameterType.WallThickness, new Parameter(
                            MinWallThickness,
                            MaxWallThickness,
                            MinWallThickness)
                    },
                    {
                        ParameterType.HandlesThickness, new Parameter(
                            MinHandlesThickness,
                            MaxHandlesThickness,
                            MinHandlesThickness)
                    },
                    {
                        ParameterType.HandlesHeight, new Parameter(
                            MinHandlesHeight,
                            MaxHandlesHeight,
                            MinHandlesHeight)
                    }
               };
        }

        /// <summary>
        /// Задать значение параметра.
        /// </summary>
        /// <param name="parameterType">Название параметра. </param>
        /// <param name="newValue">Новое значение для параметра. </param>
        public void SetValue(ParameterType parameterType, double newValue)
        {
            _parameters[parameterType].Value = newValue;
            if (parameterType == ParameterType.HandlesThickness)
            {
                UpdateMaxHandlesHeight();
                UpdateMinHandlesHeight();
            }
        }

        /// <summary>
        /// Получить значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Значение параметра.</returns>
        public double GetValue(ParameterType parameterType)
        {
            return _parameters[parameterType].Value;
        }

        /// <summary>
        /// Получить минимально допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Минимально допустимое значение параметра.</returns>
        public double GetMinValue(ParameterType parameterType)
        {
            return _parameters[parameterType].MinValue;
        }

        /// <summary>
        /// Получить максимально допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Максимально допустимое значение параметра.</returns>
        public double GetMaxValue(ParameterType parameterType)
        {
            return _parameters[parameterType].MaxValue;
        }

        /// <summary>
        /// Валидация значения.
        /// </summary>
        /// <param name="parameterType">тип проверяемого параметра</param>
        /// <param name="value">проверяемое значение</param>
        /// <returns>true, если значение прошло проверку,
        /// false в обратном случае</returns>
        public bool Validate(ParameterType parameterType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (!_parameters[parameterType].IsCorrect(Convert.ToDouble(value)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обновляет максимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMaxHandlesHeight()
        {
            _parameters[ParameterType.HandlesHeight].MaxValue =
                2 * _parameters[ParameterType.HandlesThickness].Value / 3;
        }

        /// <summary>
        /// Обновляет минимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMinHandlesHeight()
        {
            _parameters[ParameterType.HandlesHeight].MinValue =
                _parameters[ParameterType.HandlesThickness].Value / 2;
        }
    }
}
