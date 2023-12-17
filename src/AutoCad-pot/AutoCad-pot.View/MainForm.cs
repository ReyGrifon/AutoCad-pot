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

        private static readonly Color ErrorColor = Color.LightPink;

        private Dictionary<ParameterType, TextBox> _fields;

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
            UpdateLabel();
        }

        /// <summary>
        /// Получить значения поля _parameters.
        /// </summary>
        public Parameters Parameters { get; }

        private void UpdateLabel()
        {
            HandlesHeightLimitsLabel.Text = 
                Convert.ToString(
                    Math.Round(Parameters.GetMinValue(ParameterType.HandlesHeight), 2))
                + " - " + 
                Convert.ToString(
                    Math.Round(Parameters.GetMaxValue(ParameterType.HandlesHeight), 2)) + " mm";
        }

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

        private void PotBuildButton_Click(object sender, EventArgs e)
        {
            if (!CheckOnErrors())
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var currentControl =
                _fields.First(x => x.Value == sender);
            var currentParameter = currentControl.Key;
            if (sender is TextBox textBox)
            {
                try
                {
                    if (_fields[currentParameter].Text == "")
                    {
                        _fields[currentParameter].BackColor = ErrorColor;
                        _errors[currentParameter] +=
                            currentParameter + " is empty";
                        return;
                    }

                    Parameters.SetValue(currentParameter, Convert.ToDouble(textBox.Text));
                    _errors[currentParameter] = "";
                    _fields[currentParameter].BackColor = CorrectColor;
                }
                catch (AggregateException aggregateException)
                {
                    if (_errors[currentParameter] != "")
                    {
                        _errors[currentParameter] = "";
                    }

                    foreach (ArgumentException exception in aggregateException.InnerExceptions)
                    {
                        _errors[currentParameter] +=
                            currentParameter + exception.Message;
                    }

                    _fields[currentParameter].BackColor = ErrorColor;
                }

                try
                {
                    _errors[ParameterType.HandlesHeight] = "";
                    _fields[ParameterType.HandlesHeight].BackColor = CorrectColor;
                    if (currentParameter == ParameterType.HandlesThickness && Parameters.HandleType)
                    {
                        UpdateLabel();
                        Parameters.SetValue(ParameterType.HandlesHeight, Convert.ToDouble(HandlesHeightTextBox.Text));
                    }
                }
                catch (AggregateException aggregateException)
                {
                    if (_errors[ParameterType.HandlesHeight] != "")
                    {
                        _errors[ParameterType.HandlesHeight] = "";
                    }

                    foreach (ArgumentException exception in aggregateException.InnerExceptions)
                    {
                        _errors[ParameterType.HandlesHeight] +=
                            ParameterType.HandlesHeight + exception.Message;
                    }

                    _fields[ParameterType.HandlesHeight].BackColor = ErrorColor;
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

        private void HandlePotRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.HandleType = true;
            HandlesHeightLabel.Visible = true;
            HandlesHeightTextBox.Visible = true;
            HandlesHeightLimitsLabel.Visible = true;
            UpdateLabel();
        }

        private void SaucepanRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Parameters.HandleType = false;
            HandlesHeightLabel.Visible = false;
            HandlesHeightTextBox.Visible = false;
            HandlesHeightLimitsLabel.Visible = false;
        }
    }
}