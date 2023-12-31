﻿namespace AutoCad_pot.ModelApi
{
    using System.Windows.Forms;
    using AutoCad_pot.View;
    using Autodesk.AutoCAD.Runtime;

    /// <summary>
    /// Класс-обёртка для работы с Api.
    /// </summary>
    public class Connector : IExtensionApplication
    {
        /// <summary>
        /// Метод, вызывающий MainForm.
        /// </summary>
        [CommandMethod("BuildPot")]
        public void BuildPot()
        {
            var form = new MainForm();
            var dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var parameters = form.Parameters;
                var builder = new Builder(parameters);
                builder.BuildPot();
            }
        }

        /// <summary>
        /// Метод, срабатывающийся при инициализации плагина.
        /// </summary>
        public void Initialize()
        {
            MessageBox.Show(
                "Плагин загружен. Введите команду 'BuildPot' для работы с плагином",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Метод, срабатывающий при закрытии AutoCad.
        /// </summary>
        public void Terminate()
        {
        }
    }
}
