namespace AutoCad_pot.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        private readonly Dictionary<ParameterType, Parameter> _parametersDictionary;

        /// <summary>
        /// Словарь ошибок.
        /// </summary>
        private readonly Dictionary<ParameterType, List<ArgumentException>> _errorDictionary;

        public bool HandleType { get; set; }

        /// <summary>
        /// Конструктор словаря с параметрами.
        /// </summary>
        public Parameters()
        {
            HandleType = true;
            _parametersDictionary =
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
            _errorDictionary = new Dictionary<ParameterType, List<ArgumentException>>
            {
                { ParameterType.PotHeight, new List<ArgumentException>() },
                { ParameterType.PotDiameter, new List<ArgumentException>() },
                { ParameterType.BottomThickness, new List<ArgumentException>() },
                { ParameterType.WallThickness, new List<ArgumentException>() },
                { ParameterType.HandlesThickness, new List<ArgumentException>() },
                { ParameterType.HandlesHeight, new List<ArgumentException>() }
            };
        }

        /// <summary>
        /// Перегрузка оператора [].
        /// </summary>
        /// <param name="type">Тип параметра.</param>
        /// <returns>Параметр.</returns>
        public Parameter this[ParameterType type] => _parametersDictionary[type];

        /// <summary>
        /// Задать значение параметра.
        /// </summary>
        /// <param name="parameterType">Название параметра. </param>
        /// <param name="newValue">Новое значение для параметра. </param>
        public void SetValue(ParameterType parameterType, double newValue)
        {
            _errorDictionary[parameterType].Clear();
            try
            {
                _parametersDictionary[parameterType].Value = newValue;
                if (parameterType == ParameterType.HandlesThickness)
                {
                    UpdateMaxHandlesHeight();
                    UpdateMinHandlesHeight();
                }
            }
            catch (ArgumentException exception)
            {
                _errorDictionary[parameterType].Add(exception);
                if (parameterType == ParameterType.HandlesThickness)
                {
                    UpdateErrorHandlesHeight();
                }
            }

            if (_errorDictionary[parameterType].Any())
            {
                throw new AggregateException(_errorDictionary[parameterType]);
            }
        }

        /// <summary>
        /// Получить значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Значение параметра.</returns>
        public double GetValue(ParameterType parameterType)
        {
            return _parametersDictionary[parameterType].Value;
        }

        /// <summary>
        /// Получить минимально допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Минимально допустимое значение параметра.</returns>
        public double GetMinValue(ParameterType parameterType)
        {
            return _parametersDictionary[parameterType].MinValue;
        }

        /// <summary>
        /// Получить максимально допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра.</param>
        /// <returns>Максимально допустимое значение параметра.</returns>
        public double GetMaxValue(ParameterType parameterType)
        {
            return _parametersDictionary[parameterType].MaxValue;
        }

        /// <summary>
        /// Обновляет максимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMaxHandlesHeight()
        {
            _parametersDictionary[ParameterType.HandlesHeight].MaxValue =
                2 * _parametersDictionary[ParameterType.HandlesThickness].Value / 3;
        }

        /// <summary>
        /// Делает "Неправильные" границы для параметра
        /// "Высота ручки"
        /// "Высота ручки".
        /// </summary>
        public void UpdateErrorHandlesHeight()
        {
            _parametersDictionary[ParameterType.HandlesHeight].MaxValue = 0;
            _parametersDictionary[ParameterType.HandlesHeight].MinValue = 0;
        }

        /// <summary>
        /// Обновляет минимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMinHandlesHeight()
        {
            _parametersDictionary[ParameterType.HandlesHeight].MinValue =
                _parametersDictionary[ParameterType.HandlesThickness].Value / 2;
        }
    }
}
