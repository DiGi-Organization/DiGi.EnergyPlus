using DiGi.Python.Classes;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.EnergyPlus
{
    public static partial class Modify
    {
        /// <summary>
        /// Runs EnergyPlus on an input file in a run directory (DiGi-Organization/DiGi.EnergyPlus#1): writes the IDF as <see cref="Constants.FileName.Input"/>, copies the weather file as <see cref="Constants.FileName.Weather"/>, and runs the command line with ReadVarsESO (<c>-r</c>), so that <c>Convert.ToDiGi_EnergyPlusResult</c> can read the directory afterwards.
        /// <para>The output files the reader takes are deleted first, so a failed run never leaves the previous run's results to be read. The run is ended when it has written nothing for <see cref="Constants.Default.RunInactivityTimeout"/> minutes or when <paramref name="cancellationToken"/> is cancelled. A failure is logged through DiGi.Serilog with the exit code and the trailing error output.</para>
        /// </summary>
        /// <param name="text">The text of the IDF, as <c>Convert.ToEnergyPlus_String</c> writes it. This value can be null.</param>
        /// <param name="path_EPW">The path of the weather file. This value can be null.</param>
        /// <param name="path_Executable">The path of the EnergyPlus executable, from <see cref="Query.ExecutablePath(string?)"/>. This value can be null.</param>
        /// <param name="directory">The run directory; created when missing. This value can be null.</param>
        /// <param name="cancellationToken">The token that cancels the run.</param>
        /// <returns>True when EnergyPlus exited with 0 and reports that it completed successfully; false when an argument is missing, a file does not exist, or the run failed.</returns>
        public static async Task<bool> RunAsync(string? text, string? path_EPW, string? path_Executable, string? directory, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(path_EPW) || string.IsNullOrWhiteSpace(path_Executable) || string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            if (!File.Exists(path_EPW) || !File.Exists(path_Executable))
            {
                Serilog.Modify.Log(Serilog.Enums.LogEventLevel.Error, "{Type}: weather file {EPW} or executable {Executable} does not exist", nameof(RunAsync), path_EPW!, path_Executable!);
                return false;
            }

            try
            {
                Directory.CreateDirectory(directory);

                foreach (string fileName in new[] { Constants.FileName.Error, Constants.FileName.Variables, Constants.FileName.Tables })
                {
                    string path = Path.Combine(directory, fileName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }

                File.WriteAllText(Path.Combine(directory, Constants.FileName.Input), text);
                File.Copy(path_EPW, Path.Combine(directory, Constants.FileName.Weather), true);

                string arguments = "-w \"" + Constants.FileName.Weather + "\" -d \"" + directory + "\" -r \"" + Constants.FileName.Input + "\"";

                PythonProcessResult pythonProcessResult = await Task.Run(() => Python.Query.ExecuteProcess(path_Executable!, arguments, directory!, null, TimeSpan.FromMinutes(Constants.Default.RunInactivityTimeout), cancellationToken), cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                string path_Error = Path.Combine(directory, Constants.FileName.Error);
                bool completed = File.Exists(path_Error) && File.ReadAllText(path_Error).Contains("EnergyPlus Completed Successfully");
                if (pythonProcessResult.ExitCode == 0 && completed)
                {
                    return true;
                }

                string standardError = string.Join(Environment.NewLine, pythonProcessResult.StandardError ?? []);
                Serilog.Modify.Log(Serilog.Enums.LogEventLevel.Error, "{Type}: EnergyPlus exited with {ExitCode} (completed {Completed}, stalled {Stalled}) in {Directory}: {StandardError}", nameof(RunAsync), pythonProcessResult.ExitCode.ToString(CultureInfo.InvariantCulture), completed, pythonProcessResult.Stalled, directory!, standardError);
                return false;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                Serilog.Modify.Log(exception, "{Type}: EnergyPlus could not be run in {Directory}", nameof(RunAsync), directory!);
                return false;
            }
        }
    }
}
