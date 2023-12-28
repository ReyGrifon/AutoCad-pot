namespace AutoCad_pot.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using AutoCad_pot.Model;
    using TextBox = System.Windows.Forms.TextBox;

    /// <summary>
    /// Основной класс формы.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Словарь ошибок.
        /// </summary>
        private Dictionary<ParameterType, string> _errors;

        /// <summary>
        /// Переменная для цвета поля при прохождении валидации.
        /// </summary>
        private static readonly Color CorrectColor = Color.White;

        /// <summary>
        /// Переменная для цвета поля при провале валидации.
        /// </summary>
        private static readonly Color ErrorColor = Color.LightPink;

        /// <summary>
        /// Словарь текстбоксов с ключами параметрами.
        /// </summary>
        private Dictionary<ParameterType, TextBox> _fields;

        /// <summary>
        /// Поле, хранящее тип параметра и соответствующее ему элемент Label.
        /// </summary>
        private Dictionary<ParameterType, Label> _labels;

        /// <summary>
        /// Конструктор формы.
        /// </summary>
        public MainForm()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            InitializeComponent();
            Parameters = new Parameters();
            InitializeFields();
            InitializeErrors();
            InitializeLabels();
        }

        /// <summary>
        /// Получить значения поля _parameters.
        /// </summary>
        public Parameters Parameters { get; }

        /// <summary>
        /// Обновление label для параметра/
        /// HandlesHeight
        /// </summary>
        private void UpdateLabel(ParameterType type)
        {
            _labels[type].Text =
                Convert.ToString(
                    Math.Round(Parameters[type].MinValue, 2))
                + " - " +
                Convert.ToString(
                    Math.Round(Parameters[type].MaxValue, 2)) + " mm";
        }

        /// <summary>
        /// Инициализация словаря _fields.
        /// </summary>
        private void InitializeFields()
        {
            _fields = new Dictionary<ParameterType, TextBox>
            {
                { ParameterType.PotHeight,PotHeightTextBox},
                { ParameterType.PotDiameter , PotDiameterTextBox},
                { ParameterType.BottomThickness , BottomThicknessTextBox },
                { ParameterType.WallThickness , WallThicknessTextBox },
                { ParameterType.HandlesThickness , HandlesThicknessTextBox },
                { ParameterType.HandlesHeight , HandlesHeightTextBox }
            };
        }

        /// <summary>
        /// Инициализация полей Label.
        /// </summary>
        private void InitializeLabels()
        {
            _labels = new Dictionary<ParameterType, Label>
            {
                {ParameterType.PotHeight, PotHeightLimitsLabel},
                {ParameterType.PotDiameter, PotDiameterLimitsLabel},
                {ParameterType.BottomThickness,  BottomThicknessLimitsLabel},
                {ParameterType.WallThickness, WallThicknessLimitsLabel},
                {ParameterType.HandlesThickness, HandlesThicknessLimitsLabel},
                {ParameterType.HandlesHeight, HandlesHeightLimitsLabel}
            };
        }

        /// <summary>
        /// Инициализация словаря с ошибками.
        /// </summary>
        private void InitializeErrors()
        {
            _errors = new Dictionary<ParameterType, string>
            {
                { ParameterType.PotHeight, "" },
                { ParameterType.PotDiameter, "" },
                { ParameterType.BottomThickness, "" },
                { ParameterType.WallThickness, "" },
                { ParameterType.HandlesThickness, "" },
                { ParameterType.HandlesHeight, "" }
            };
        }

        /// <summary>
        /// Событие нажимания кнопки Build
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PotBuildButton_Click(object sender, EventArgs e)
        {
            if (!CheckOnErrors())
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Событие изменения поля textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var currentControl =
                _fields.First(x => x.Value == sender);
            var currentParameter = currentControl.Key;
            if (sender is TextBox textBox)
            {
                try
                {
                    _errors[currentParameter] = "";
                    if (_fields[currentParameter].Text == "")
                    {
                        _fields[currentParameter].BackColor = ErrorColor;
                        _errors[currentParameter] +=
                            currentParameter + " is empty\n";
                        return;
                    }

                    Parameters.SetValue(
                        currentParameter,
                        Convert.ToDouble(textBox.Text));
                    _errors[currentParameter] = "";
                    _fields[currentParameter].BackColor = CorrectColor;
                }
                catch (AggregateException aggregateException)
                {

                    foreach (ArgumentException exception in aggregateException.InnerExceptions)
                    {
                        _errors[currentParameter] +=
                            currentParameter + exception.Message;
                    }

                    _fields[currentParameter].BackColor = ErrorColor;
                }

                try
                {
                    if (currentParameter == ParameterType.HandlesThickness &&
                        Parameters.HandleType)
                    {
                        UpdateLabel(ParameterType.HandlesHeight);
                        Parameters.SetValue(
                            ParameterType.HandlesHeight, 
                            Convert.ToDouble(HandlesHeightTextBox.Text));
                        _fields[ParameterType.HandlesHeight].BackColor = CorrectColor;
                        _errors[ParameterType.HandlesHeight] = "";
                    }
                }
                catch (AggregateException aggregateException)
                {
                    _errors[ParameterType.HandlesHeight] = "";

                    foreach (ArgumentException exception in aggregateException.InnerExceptions)
                    {
                        _errors[ParameterType.HandlesHeight] +=
                            ParameterType.HandlesHeight + exception.Message;
                    }

                    _fields[ParameterType.HandlesHeight].BackColor = ErrorColor;
                }

                try
                {
                    if (currentParameter == ParameterType.HandlesHeight &&
                        Parameters.HandleType)
                    {
                        UpdateLabel(ParameterType.HandlesThickness);
                        Parameters.SetValue(
                            ParameterType.HandlesThickness,
                            Convert.ToDouble(HandlesThicknessTextBox.Text));
                        _fields[ParameterType.HandlesThickness].BackColor = CorrectColor;
                        _errors[ParameterType.HandlesThickness] = "";
                    }
                }
                catch (AggregateException aggregateException)
                {
                    _errors[ParameterType.HandlesThickness] = "";

                    foreach (ArgumentException exception in aggregateException.InnerExceptions)
                    {
                        _errors[ParameterType.HandlesThickness] +=
                            ParameterType.HandlesThickness + exception.Message;
                    }

                    _fields[ParameterType.HandlesThickness].BackColor = ErrorColor;
                }
            }
        }

        /// <summary>
        /// Проверяет правильность введенных данных.
        /// </summary>
        /// <returns>true - ошибок нет, false - есть ошибки при введении данных.</returns>
        private bool CheckOnErrors()
        {
            string allErrors = "";

            foreach (var error in _errors)
            {
                if (error.Value != "")
                {
                    allErrors += error.Value;
                }
            }

            if (allErrors != "")
            {
                MessageBox.Show(
                    allErrors,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Событие нажатия на кнопку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (char.IsControl(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }

                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                    return;
                }

                if (e.KeyChar == '.')
                {
                    if (textBox.Text == "")
                    {
                        e.Handled = true;
                        return;
                    }

                    e.Handled = textBox.Text.Contains(".")
                        ? e.Handled = true
                        : e.Handled = false;

                    return;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// установка HandlePotRadioButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePotRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.HandleType = true;
            HandlesThicknessLabel.Visible = true;
            HandlesThicknessTextBox.Visible = true;
            HandlesThicknessLimitsLabel.Visible = true;
            UpdateLabel(ParameterType.HandlesThickness);
            Parameters.UpdateMaxHandlesHeight();
        }

        /// <summary>
        /// Установка SauсepanRadioButton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaucepanRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            HandlesThicknessTextBox.Text = Convert.ToString(
                Parameters[ParameterType.HandlesThickness].MinValue);
            Parameters.HandleType = false;
            HandlesThicknessLabel.Visible = false;
            HandlesThicknessTextBox.Visible = false;
            HandlesThicknessLimitsLabel.Visible = false;
            Parameters.UpdateHandlesHeightDefaultLimit();
            UpdateLabel(ParameterType.HandlesHeight);
            HandlesHeightTextBox.Text = Convert.ToString(
                Parameters[ParameterType.HandlesHeight].Value);
        }
    }
}