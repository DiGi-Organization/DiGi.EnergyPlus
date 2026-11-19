using DiGi.Core.Classes;
using DiGi.EnergyPlus.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.EnergyPlus.Classes
{
    /// <summary>
    /// Represents the heating need EnergyPlus calculated for a building written by <c>Convert.ToEnergyPlus_String</c> (DiGi-Organization/DiGi.EnergyPlus#1): the 12 monthly and the annual sensible heating energy of the ideal-loads air systems, per system and in total, the thermal transmittances EnergyPlus reports for the external surfaces, their areas, the EnergyPlus version and the warning and severe error counts of the run. Created by <c>Convert.ToDiGi_EnergyPlusResult</c> from the output of a run; the constructor only assigns and copies.
    /// </summary>
    public class EnergyPlusResult : SerializableResult, IEnergyPlusSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(Areas))]
        private readonly Dictionary<string, double>? areas;

        [JsonInclude, JsonPropertyName(nameof(EnergyPlusVersion))]
        private readonly string? energyPlusVersion;

        [JsonInclude, JsonPropertyName(nameof(HeatingEnergy))]
        private readonly double heatingEnergy = double.NaN;

        [JsonInclude, JsonPropertyName(nameof(HeatingEnergies))]
        private readonly List<double>? heatingEnergies;

        [JsonInclude, JsonPropertyName(nameof(SevereErrorCount))]
        private readonly int severeErrorCount;

        [JsonInclude, JsonPropertyName(nameof(SystemHeatingEnergies))]
        private readonly Dictionary<string, List<double>>? systemHeatingEnergies;

        [JsonInclude, JsonPropertyName(nameof(ThermalTransmittances))]
        private readonly Dictionary<string, double>? thermalTransmittances;

        [JsonInclude, JsonPropertyName(nameof(WarningCount))]
        private readonly int warningCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyPlusResult"/> class. The values are assigned and the collections copied; <c>Convert.ToDiGi_EnergyPlusResult</c> reads them.
        /// </summary>
        /// <param name="energyPlusVersion">The version of EnergyPlus that ran, as it reports it.</param>
        /// <param name="heatingEnergy">The annual sensible heating energy of every ideal-loads air system [kWh].</param>
        /// <param name="heatingEnergies">The monthly sensible heating energy of every ideal-loads air system, January to December [kWh].</param>
        /// <param name="systemHeatingEnergies">The monthly sensible heating energy of each ideal-loads air system, January to December [kWh], keyed by its name as EnergyPlus reports it.</param>
        /// <param name="thermalTransmittances">The thermal transmittance EnergyPlus reports for each external surface [W/(m2K)]: the U-factor with film of an opaque surface or a door, the glass U-factor of a window, keyed by the surface name in lower case.</param>
        /// <param name="areas">The area EnergyPlus reports for each external surface [m2]: the net area of an opaque surface, the area of a window or a door, keyed like <paramref name="thermalTransmittances"/>.</param>
        /// <param name="warningCount">The number of warnings of the run.</param>
        /// <param name="severeErrorCount">The number of severe errors of the run.</param>
        public EnergyPlusResult(
            string? energyPlusVersion,
            double heatingEnergy,
            IEnumerable<double>? heatingEnergies,
            IDictionary<string, List<double>>? systemHeatingEnergies,
            IDictionary<string, double>? thermalTransmittances,
            IDictionary<string, double>? areas,
            int warningCount,
            int severeErrorCount)
        {
            this.areas = areas is null ? null : new Dictionary<string, double>(areas);
            this.energyPlusVersion = energyPlusVersion;
            this.heatingEnergy = heatingEnergy;
            this.heatingEnergies = heatingEnergies is null ? null : [.. heatingEnergies];
            this.thermalTransmittances = thermalTransmittances is null ? null : new Dictionary<string, double>(thermalTransmittances);
            this.warningCount = warningCount;
            this.severeErrorCount = severeErrorCount;

            if (systemHeatingEnergies is not null)
            {
                this.systemHeatingEnergies = [];
                foreach (KeyValuePair<string, List<double>> keyValuePair in systemHeatingEnergies)
                {
                    this.systemHeatingEnergies[keyValuePair.Key] = keyValuePair.Value is null ? [] : [.. keyValuePair.Value];
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyPlusResult"/> class by copying an existing instance.
        /// </summary>
        /// <param name="energyPlusResult">The instance to copy. This value can be null.</param>
        public EnergyPlusResult(EnergyPlusResult? energyPlusResult)
            : base(energyPlusResult)
        {
            if (energyPlusResult is not null)
            {
                areas = energyPlusResult.areas is null ? null : new Dictionary<string, double>(energyPlusResult.areas);
                energyPlusVersion = energyPlusResult.energyPlusVersion;
                heatingEnergy = energyPlusResult.heatingEnergy;
                heatingEnergies = energyPlusResult.heatingEnergies is null ? null : [.. energyPlusResult.heatingEnergies];
                thermalTransmittances = energyPlusResult.thermalTransmittances is null ? null : new Dictionary<string, double>(energyPlusResult.thermalTransmittances);
                warningCount = energyPlusResult.warningCount;
                severeErrorCount = energyPlusResult.severeErrorCount;

                if (energyPlusResult.systemHeatingEnergies is not null)
                {
                    systemHeatingEnergies = [];
                    foreach (KeyValuePair<string, List<double>> keyValuePair in energyPlusResult.systemHeatingEnergies)
                    {
                        systemHeatingEnergies[keyValuePair.Key] = keyValuePair.Value is null ? [] : [.. keyValuePair.Value];
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnergyPlusResult"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object. This value can be null.</param>
        public EnergyPlusResult(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets a copy of the area EnergyPlus reports for each external surface [m2]: the net area of an opaque surface (its openings subtracted) and the area of a window or a door, keyed like <see cref="ThermalTransmittances"/>. With them, Σ U·A is the direct heat transfer coefficient EnergyPlus sees.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, double>? Areas
        {
            get
            {
                return areas is null ? null : new Dictionary<string, double>(areas);
            }
        }

        /// <summary>
        /// Gets the version of EnergyPlus that ran, as it reports it (for example <c>26.2.0-4bd7a1f26f</c>).
        /// </summary>
        [JsonIgnore]
        public string? EnergyPlusVersion
        {
            get
            {
                return energyPlusVersion;
            }
        }

        /// <summary>
        /// Gets the annual sensible heating energy of every ideal-loads air system, the sum of <see cref="HeatingEnergies"/> [kWh].
        /// </summary>
        [JsonIgnore]
        public double HeatingEnergy
        {
            get
            {
                return heatingEnergy;
            }
        }

        /// <summary>
        /// Gets a copy of the monthly sensible heating energy of every ideal-loads air system, January to December [kWh].
        /// </summary>
        [JsonIgnore]
        public List<double>? HeatingEnergies
        {
            get
            {
                return heatingEnergies is null ? null : [.. heatingEnergies];
            }
        }

        /// <summary>
        /// Gets the number of severe errors of the run.
        /// </summary>
        [JsonIgnore]
        public int SevereErrorCount
        {
            get
            {
                return severeErrorCount;
            }
        }

        /// <summary>
        /// Gets a copy of the monthly sensible heating energy of each ideal-loads air system, January to December [kWh], keyed by its name as EnergyPlus reports it (upper case). The building is one zone heated by one system, so there is one entry.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, List<double>>? SystemHeatingEnergies
        {
            get
            {
                if (systemHeatingEnergies is null)
                {
                    return null;
                }

                Dictionary<string, List<double>> result = [];
                foreach (KeyValuePair<string, List<double>> keyValuePair in systemHeatingEnergies)
                {
                    result[keyValuePair.Key] = keyValuePair.Value is null ? [] : [.. keyValuePair.Value];
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a copy of the thermal transmittance EnergyPlus reports for each external surface [W/(m2K)]: the U-factor with film of an opaque surface or a door (EnergyPlus's own film coefficients, not R_si and R_se of PN-EN ISO 6946) and the glass U-factor of a window. Keyed by the surface name in lower case: EnergyPlus reports names upper-cased, and a surface written by <c>Convert.ToEnergyPlus_String</c> is named after its component's identifier in the "D" format.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, double>? ThermalTransmittances
        {
            get
            {
                return thermalTransmittances is null ? null : new Dictionary<string, double>(thermalTransmittances);
            }
        }

        /// <summary>
        /// Gets the number of warnings of the run.
        /// </summary>
        [JsonIgnore]
        public int WarningCount
        {
            get
            {
                return warningCount;
            }
        }
    }
}
