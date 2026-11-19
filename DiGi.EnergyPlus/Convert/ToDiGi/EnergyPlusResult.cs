using DiGi.EnergyPlus.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace DiGi.EnergyPlus
{
    public static partial class Convert
    {
        /// <summary>
        /// Reads the result of an EnergyPlus run from its run directory (DiGi-Organization/DiGi.EnergyPlus#1), as <c>Modify.RunAsync</c> leaves it.
        /// <para>From <see cref="Constants.FileName.Error"/>: the version, and the warning and severe error counts of the completed run. From <see cref="Constants.FileName.Variables"/>: the monthly <see cref="Constants.Name.HeatingEnergyVariable"/> of every ideal-loads air system, converted from J to kWh, January to December. From <see cref="Constants.FileName.Tables"/>, when present: the U-factor with film of every opaque external surface and the glass U-factor of every window of the envelope summary.</para>
        /// </summary>
        /// <param name="directory">The run directory. This value can be null.</param>
        /// <returns>The <see cref="EnergyPlusResult"/>, or null when the directory or a required file is missing, the run did not complete successfully, or no system reports 12 monthly heating energies.</returns>
        public static EnergyPlusResult? ToDiGi_EnergyPlusResult(string? directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return null;
            }

            string path_Error = Path.Combine(directory, Constants.FileName.Error);
            string path_Variables = Path.Combine(directory, Constants.FileName.Variables);
            if (!File.Exists(path_Error) || !File.Exists(path_Variables))
            {
                return null;
            }

            // Version and completion.
            string text_Error = File.ReadAllText(path_Error);
            MatchCollection matchCollection = Regex.Matches(text_Error, @"EnergyPlus Completed Successfully-- (\d+) Warning; (\d+) Severe Errors");
            if (matchCollection.Count == 0)
            {
                return null;
            }

            Match match_Completed = matchCollection[matchCollection.Count - 1];
            int warningCount = int.Parse(match_Completed.Groups[1].Value, CultureInfo.InvariantCulture);
            int severeErrorCount = int.Parse(match_Completed.Groups[2].Value, CultureInfo.InvariantCulture);

            Match match_Version = Regex.Match(text_Error, @"Program Version,EnergyPlus, Version ([^,\s]+)");
            string? energyPlusVersion = match_Version.Success ? match_Version.Groups[1].Value : null;

            // Monthly heating energy per ideal-loads air system.
            string[] lines_Variables = File.ReadAllLines(path_Variables);
            if (lines_Variables.Length < 13)
            {
                return null;
            }

            string[] headers = lines_Variables[0].Split(',');
            string suffix = ":" + Constants.Name.HeatingEnergyVariable + " [J](Monthly)";
            Dictionary<string, List<double>> systemHeatingEnergies = [];
            for (int i = 1; i < headers.Length; i++)
            {
                string header = headers[i].Trim();
                if (!header.EndsWith(suffix, StringComparison.Ordinal))
                {
                    continue;
                }

                List<double> heatingEnergies_System = [];
                for (int j = 1; j < lines_Variables.Length && heatingEnergies_System.Count < 12; j++)
                {
                    string[] values = lines_Variables[j].Split(',');
                    if (values.Length <= i || !double.TryParse(values[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    {
                        continue;
                    }

                    heatingEnergies_System.Add(value / 3600000.0);
                }

                if (heatingEnergies_System.Count == 12)
                {
                    systemHeatingEnergies[header.Substring(0, header.Length - suffix.Length)] = heatingEnergies_System;
                }
            }

            if (systemHeatingEnergies.Count == 0)
            {
                return null;
            }

            List<double> heatingEnergies = [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0];
            double heatingEnergy = 0.0;
            foreach (List<double> heatingEnergies_System in systemHeatingEnergies.Values)
            {
                for (int i = 0; i < 12; i++)
                {
                    heatingEnergies[i] += heatingEnergies_System[i];
                    heatingEnergy += heatingEnergies_System[i];
                }
            }

            // Thermal transmittances of the envelope summary, keyed by the surface name in lower case.
            Dictionary<string, double>? thermalTransmittances = null;
            string path_Tables = Path.Combine(directory, Constants.FileName.Tables);
            if (File.Exists(path_Tables))
            {
                string[] lines_Tables = File.ReadAllLines(path_Tables);
                thermalTransmittances = [];

                foreach ((string Section, string Column) table in new[] { ("Opaque Exterior", "U-Factor with Film [W/m2-K]"), ("Exterior Fenestration", "Glass U-Factor [W/m2-K]") })
                {
                    int index_Section = Array.FindIndex(lines_Tables, x => x.Trim() == table.Section);
                    if (index_Section < 0)
                    {
                        continue;
                    }

                    int index_Header = index_Section + 1;
                    while (index_Header < lines_Tables.Length && string.IsNullOrWhiteSpace(lines_Tables[index_Header]))
                    {
                        index_Header++;
                    }

                    if (index_Header >= lines_Tables.Length)
                    {
                        continue;
                    }

                    int index_Column = Array.FindIndex(lines_Tables[index_Header].Split(','), x => x.Trim() == table.Column);
                    if (index_Column < 0)
                    {
                        continue;
                    }

                    for (int i = index_Header + 1; i < lines_Tables.Length && !string.IsNullOrWhiteSpace(lines_Tables[i]); i++)
                    {
                        string[] values = lines_Tables[i].Split(',');

                        // Total and average rows have no construction.
                        if (values.Length <= index_Column || values.Length < 3 || string.IsNullOrWhiteSpace(values[2]))
                        {
                            continue;
                        }

                        if (double.TryParse(values[index_Column].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double thermalTransmittance))
                        {
                            thermalTransmittances[values[1].Trim().ToLowerInvariant()] = thermalTransmittance;
                        }
                    }
                }
            }

            return new EnergyPlusResult(energyPlusVersion, heatingEnergy, heatingEnergies, systemHeatingEnergies, thermalTransmittances, warningCount, severeErrorCount);
        }
    }
}
