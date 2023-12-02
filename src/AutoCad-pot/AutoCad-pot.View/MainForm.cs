namespace AutoCad_pot.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using AutoCad_pot.Model;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
            InitializeComponent();
            parameters = new Parameters();
            InitializeFields();
            InitializeErrors();
        }

        /// <summary>
        /// Получить значения поля _parameters.
        /// </summary>
        public Parameters parameters { get; }

        private void UpdateLabel()
        {
            HandlesHeightLimitsLabel.Text = Convert.ToString(parameters.GetMinValue(ParameterType.HandlesHeight))
                + " - " + Convert.ToString(parameters.GetMaxValue(ParameterType.HandlesHeight)) + " mm";
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
                    errors += error.ToString() + "\n";
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
                Validate(textBox);

                if (_fields[textBox] == ParameterType.HandlesThickness)
                {
                    Validate(HandlesHeightTextBox);
                }
            }
        }

        private void Validate(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                CheckError(textBox, true);
                return;
            }

            if (parameters.Validate(_fields[textBox], Convert.ToDouble(textBox.Text)))
            {
                parameters.SetValue(_fields[textBox], Convert.ToDouble(textBox.Text));
                CheckError(textBox, false);
            }
            else
            {
                CheckError(textBox, true);
            }

            if (_fields[textBox] == ParameterType.HandlesThickness)
            {
                parameters.UpdateMaxHandlesHeight();
                parameters.UpdateMinHandlesHeight();
                UpdateLabel();
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

                if (e.KeyChar == ',')
                {
                    if (textBox.Text == "")
                    {
                        e.Handled = true;
                        return;
                    }

                    e.Handled = textBox.Text.Contains(",")
                        ? e.Handled = true
                        : e.Handled = false;

                    return;
                }

                e.Handled = true;
            }
        }
    }
}
