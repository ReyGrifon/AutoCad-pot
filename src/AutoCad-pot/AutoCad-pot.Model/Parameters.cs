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
        public const double MinHandlesThickness = 2.5;

        /// <summary>
        /// Максимальная толщина ручек.
        /// </summary>
        public const double MaxHandlesThickness = 5;

        /// <summary>
        /// Минимальная высота ручек.
        /// </summary>
        public const double MinHandlesHeight = 10;

        /// <summary>
        /// Максимальная высота ручек.
        /// </summary>
        public const double MaxHandlesHeight = 12.5;

        /// <summary>
        /// Словарь с параметрам модели.
        /// </summary>
        private readonly Dictionary<ParameterType, Parameter> _parameters;

        /// <summary>
        /// Словарь ошибок.
        /// </summary>
        private readonly Dictionary<ParameterType, List<ArgumentException>> _error;

        /// <summary>
        /// флаг для определения вида ручки.
        /// </summary>
        public bool HandleType { get; set; }

        /// <summary>
        /// Конструктор словаря с параметрами.
        /// </summary>
        public Parameters()
        {
            HandleType = true;
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
            _error = new Dictionary<ParameterType, List<ArgumentException>>
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
        public Parameter this[ParameterType type] => _parameters[type];

        /// <summary>
        /// Задать значение параметра.
        /// </summary>
        /// <param name="parameterType">Название параметра. </param>
        /// <param name="newValue">Новое значение для параметра. </param>
        public void SetValue(ParameterType parameterType, double newValue)
        {
            _error[parameterType].Clear();
            try
            {
                _parameters[parameterType].Value = newValue;
                if (parameterType == ParameterType.HandlesHeight)
                {
                    UpdateMaxHandlesThickness();
                    UpdateMinHandlesThickness();
                }

                if (parameterType == ParameterType.HandlesThickness)
                {
                    UpdateMaxHandlesHeight();
                }
            }
            catch (ArgumentException exception)
            {
                _error[parameterType].Add(exception);

                if (parameterType == ParameterType.HandlesHeight && HandleType)
                {
                    UpdateErrorLimits(ParameterType.HandlesThickness);
                }
            }

            if (_error[parameterType].Any())
            {
                throw new AggregateException(_error[parameterType]);
            }
        }

        /// <summary>
        /// Делает "Неправильные" границы для параметра
        /// "Высота ручки"
        /// "Высота ручки".
        /// </summary>
        private void UpdateErrorLimits(ParameterType type)
        {
            _parameters[type].MaxValue = 0;
            _parameters[type].MinValue = 0;
        }

        /// <summary>
        /// Обновляет максимальный порог для параметра
        /// "Толщина ручки".
        /// </summary>
        public void UpdateMaxHandlesThickness()
        {
            _parameters[ParameterType.HandlesThickness].MaxValue =
                _parameters[ParameterType.HandlesHeight].Value / 2;
        }

        /// <summary>
        /// Обновляет максимальный порог для параметра
        /// "Толщина ручки".
        /// </summary>
        public void UpdateMinHandlesThickness()
        {
            _parameters[ParameterType.HandlesThickness].MinValue =
                _parameters[ParameterType.HandlesHeight].Value / 4;
        }

        /// <summary>
        /// Обновляет параметры для ручки сотейника.
        /// </summary>
        public void UpdateHandlesHeightDefaultLimit()
        {
            _parameters[ParameterType.HandlesHeight].MaxValue = 12.5;
            _parameters[ParameterType.HandlesHeight].Value = 10;
        }

        /// <summary>
        /// Обновляет максимальный порог для параметра
        /// "Высота ручки".
        /// </summary>
        public void UpdateMaxHandlesHeight()
        {
            var maxLimit = _parameters[ParameterType.HandlesThickness].Value * 5;
            if (maxLimit <= 20)
            {
                _parameters[ParameterType.HandlesHeight].MaxValue =
                    maxLimit;
            }
            else
            {
                _parameters[ParameterType.HandlesHeight].MaxValue = 20;
            }
        }
    }
}
