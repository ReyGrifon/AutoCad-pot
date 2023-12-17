namespace AutoCad_pot.StressTesting
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using AutoCad_pot.Model;
    using AutoCad_pot.ModelApi;
    using Autodesk.AutoCAD.Runtime;
    using Microsoft.VisualBasic.Devices;

    public class StressTesting : IExtensionApplication
    {
        /// <summary>
        /// Метод, вызывающий нагрузочное тестирование
        /// </summary>
        [CommandMethod("StressBuildPot")]
        public void StressBuildPot()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.PotHeight, 225);
            parameters.SetValue(ParameterType.PotDiameter, 175);
            parameters.SetValue(ParameterType.BottomThickness, 5.5);
            parameters.SetValue(ParameterType.WallThickness, 1.75);
            parameters.SetValue(ParameterType.HandlesThickness, 6.5);
            parameters.SetValue(ParameterType.HandlesHeight, 3.79);
            var builder = new Builder(parameters);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var streamWriter = new StreamWriter($"log.txt", false);
            var currentProcess = Process.GetCurrentProcess();
            const double gigabyteInByte = 0.000000000931322574615478515625;
            var count = 0;
            while (count != 1001)
            {
                builder.BuildPot();
                var computerInfo = new ComputerInfo();
                var usedMemory = (computerInfo.TotalPhysicalMemory
                                  - computerInfo.AvailablePhysicalMemory)
                                 * gigabyteInByte;
                streamWriter.WriteLine(
                    $"{++count}\t{stopWatch.Elapsed:hh\\:mm\\:ss}\t{usedMemory}");
                streamWriter.Flush();
            }

            stopWatch.Stop();
            streamWriter.Close();
            streamWriter.Dispose();
            Console.Write($"End {new ComputerInfo().TotalPhysicalMemory}");
        }

        /// <summary>
        /// Метод, срабатывающийся при инициализации плагина.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Метод, срабатывающий при закрытии AutoCad.
        /// </summary>
        public void Terminate()
        {
        }
    }
}
