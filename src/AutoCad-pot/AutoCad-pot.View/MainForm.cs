namespace AutoCad_pot.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using AutoCad_pot.Model;
    using TextBox = System.Windows.Forms.TextBox;

    /// <summary>
    /// Основной класс формы.
    /// </summary>
    public partial class MainForm : Form
    {
        private List<string> _errorsList;

        private string errors;

        /// <summary>
        /// Переменная для цвета поля при прохождении валидации.
        /// </summary>
        private static readonly Color CorrectColor = Color.White;

        private static readonly Color ErrorColor = Color.LightPink;

        private Dictionary<TextBox, ParameterType> _fields;

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

        public string HandleType { get;  }

        private void UpdateLabel()
        {
            HandlesHeightLimitsLabel.Text = 
                Convert.ToString(
                    Math.Round(Parameters.GetMinValue(ParameterType.HandlesHeight), 2))
                + " - " + 
                Convert.ToString(
                    Math.Round(Parameters.GetMaxValue(ParameterType.HandlesHeight), 2)) + " mm";
        }

        private void ErrorUpdateLabel()
        {
            HandlesHeightLimitsLabel.Text = "x - x mm";
        }

        private void InitializeFields()
        {
            _fields = new Dictionary<TextBox, ParameterType>
            {
                { PotHeightTextBox, ParameterType.PotHeight},
                { PotDiameterTextBox, ParameterType.PotDiameter},
                { BottomThicknessTextBox, ParameterType.BottomThickness },
                { WallThicknessTextBox, ParameterType.WallThickness },
                { HandlesThicknessTextBox, ParameterType.HandlesThickness },
                { HandlesHeightTextBox, ParameterType.HandlesHeight }
            };
        }

        private void InitializeErrors()
        {
            _errorsList = new List<string>();
        }

        private void PotBuildButton_Click(object sender, EventArgs e)
        {
            if (_errorsList.Count != 0)
            {
                errors = "";
                foreach (var error in _errorsList)
                {
                    errors += error.ToString() + " is not in the specified range" + "\n";
                }

                MessageBox.Show(
                    errors,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                CheckValueValidate(textBox);
            }
        }

        private void CheckValueValidate(TextBox textBox)
        {
            if (Parameters.Validate(_fields[textBox], textBox.Text))
            {
                Parameters.SetValue(_fields[textBox], Convert.ToDouble(textBox.Text));
                CheckError(textBox, false);
                if (_fields[textBox] == ParameterType.HandlesThickness && Parameters.HandleType)
                {
                    CheckValueValidate(HandlesHeightTextBox);
                    UpdateLabel();
                }
            }
            else
            {
                CheckError(textBox, true);
            }
        }

        private void CheckError(TextBox textBox, bool deciption)
        {
            if (deciption)
            {
                if (!_errorsList.Contains(Convert.ToString(_fields[textBox])))
                {
                    _errorsList.Add(Convert.ToString(_fields[textBox]));
                }

                textBox.BackColor = ErrorColor;
                if (_fields[textBox] == ParameterType.HandlesThickness && Parameters.HandleType)
                {
                    CheckError(HandlesHeightTextBox, true);
                    ErrorUpdateLabel();
                }
            }
            else
            {
                if (_errorsList.Contains(Convert.ToString(_fields[textBox])))
                {
                    _errorsList.Remove(Convert.ToString(_fields[textBox]));
                }

                textBox.BackColor = CorrectColor;
            }
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
            CheckValueValidate(HandlesHeightTextBox);
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
