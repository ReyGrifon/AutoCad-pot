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
        /// g.
        /// </summary>
        public const double MinPotHeight = 150;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxPotHeight = 300;

        /// <summary>
        /// k.
        /// </summary>
        public const double MinPotDiameter = 150;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxPotDiameter = 200;

        /// <summary>
        /// k.
        /// </summary>
        public const double MinBottomThickness = 1;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxBottomThickness = 10;

        /// <summary>
        /// k.
        /// </summary>
        public const double MinWallThickness = 0.5;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxWallThickness = 3;

        /// <summary>
        /// k.
        /// </summary>
        public const double MinHandlesThickness = 3;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxHandlesThickness = 10;

        /// <summary>
        /// k.
        /// </summary>
        public const double MinHandlesHeight = 0;

        /// <summary>
        /// k.
        /// </summary>
        public const double MaxHandlesHeight = 0;

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
        /// <param name="parameterType"></param>
        /// <param name="newValue"></param>
        public void SetValue(ParameterType parameterType, double newValue)
        {
            _parameters[parameterType].Value = newValue;
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
        /// Валидатор.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Validate(ParameterType parameterType, double value)
        {
            if (_parameters[parameterType].MinValue > value
                || value > _parameters[parameterType].MaxValue)
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
                Math.Round(2 * _parameters[ParameterType.HandlesThickness].Value / 3, 2);
        }

        /// <summary>
        /// Обновляет минимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMinHandlesHeight()
        {
            _parameters[ParameterType.HandlesHeight].MinValue =
                Math.Round(_parameters[ParameterType.HandlesThickness].Value / 2, 2);
        }
    }
}
