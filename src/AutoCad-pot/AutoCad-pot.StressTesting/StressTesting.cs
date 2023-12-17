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
            var builder = new Builder(parameters);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var streamWriter = new StreamWriter($"log.txt", false);
            var currentProcess = Process.GetCurrentProcess();
            var count = 0;
            while (count != 2000)
            {
                const double gigabyteInByte = 0.000000000931322574615478515625;
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
