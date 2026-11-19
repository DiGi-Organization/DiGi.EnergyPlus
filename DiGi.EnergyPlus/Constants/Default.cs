namespace DiGi.EnergyPlus.Constants
{
    /// <summary>
    /// Default values of the EnergyPlus export (DiGi-Organization/DiGi.EnergyPlus#1).
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// The EnergyPlus version the input file is written for, as its <c>Version</c> object states it. The field lists of every object written were checked against the IDD of this release.
        /// </summary>
        public const string Version = "26.2";

        /// <summary>
        /// The number of time steps per hour of the simulation.
        /// </summary>
        public const int TimestepsPerHour = 6;

        /// <summary>
        /// The maximum number of warm-up days. A heavy building on a constant set-point converges slowly, so the EnergyPlus default of 25 is raised.
        /// </summary>
        public const int MaximumWarmupDays = 50;

        /// <summary>
        /// The minimum number of warm-up days.
        /// </summary>
        public const int MinimumWarmupDays = 6;

        /// <summary>
        /// The smallest thermal resistance EnergyPlus accepts for a <c>Material:NoMass</c> [m2K/W].
        /// </summary>
        public const double MinimumThermalResistance = 0.001;

        /// <summary>
        /// The distance within which a vertex counts as coincident with its predecessor or collinear with its neighbours, and is deleted before a ring is written [m]. EnergyPlus deletes such vertices itself and drops a surface left with fewer than three.
        /// </summary>
        public const double VertexTolerance = 0.01;

        /// <summary>
        /// The sine of the turn below which a vertex counts as collinear with its neighbours and is deleted before a ring is written (about 0.3°). EnergyPlus judges collinearity by the angle too, spikes included: a 0.16 m by 80 m wedge of a triangulated neighbour, its apex angle 0.014°, was left with fewer than three vertices (DiGi-Organization/DiGi.EnergyPlus#1).
        /// </summary>
        public const double CollinearSine = 0.005;

        /// <summary>
        /// The smallest shortfall of a construction's layer resistance that is made up by a massless layer of its own [m2K/W]. A smaller one is made up by scaling the layers: a near-zero <c>Material:NoMass</c> outermost stopped EnergyPlus finding the conduction transfer functions of a brick wall with internal insulation (DiGi-Organization/DiGi.EnergyPlus#1).
        /// </summary>
        public const double MinimumAdjustmentThermalResistance = 0.1;

        /// <summary>
        /// The largest number of layers of an EnergyPlus <c>Construction</c>.
        /// </summary>
        public const int MaximumLayerCount = 10;

        /// <summary>
        /// The thermal transmittance of the hidden surface that carries the thermal bridges [W/(m2K)]. Its area is H_tb divided by this value; a low value keeps the surface's own film resistance, which EnergyPlus calculates rather than takes as R_si, a small part of its resistance.
        /// </summary>
        public const double ThermalBridgeThermalTransmittance = 0.25;

        /// <summary>
        /// The distance between the building and the hidden surface that carries the thermal bridges [m].
        /// </summary>
        public const double ThermalBridgeOffset = 1.0;

        /// <summary>
        /// The thermal absorptance (emissivity) of every material written [-].
        /// </summary>
        public const double ThermalAbsorptance = 0.9;

        /// <summary>
        /// The visible absorptance of every material written [-].
        /// </summary>
        public const double VisibleAbsorptance = 0.7;

        /// <summary>
        /// The roughness of every material written.
        /// </summary>
        public const string Roughness = "MediumRough";

        /// <summary>
        /// The span without a line of output after which a run is ended [min]. EnergyPlus reports every simulated month, so a quiet run has hung.
        /// </summary>
        public const int RunInactivityTimeout = 10;

        /// <summary>
        /// The thermal resistance of the placeholder construction of an adiabatic surface whose own construction cannot be written [m2K/W].
        /// </summary>
        public const double PlaceholderThermalResistance = 0.1;
    }
}
