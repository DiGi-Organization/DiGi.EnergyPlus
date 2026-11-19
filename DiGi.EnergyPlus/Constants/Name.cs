namespace DiGi.EnergyPlus.Constants
{
    /// <summary>
    /// Names of the objects of the EnergyPlus input file that a reader of its output refers to (DiGi-Organization/DiGi.EnergyPlus#1).
    /// </summary>
    public static class Name
    {
        /// <summary>
        /// The name of the single zone the building is modelled as.
        /// </summary>
        public const string Zone = "Building";

        /// <summary>
        /// The name of the ideal-loads air system that heats the zone.
        /// </summary>
        public const string IdealLoadsAirSystem = "Ideal Loads";

        /// <summary>
        /// The name of the other-side coefficients that hold the outside face of a floor on the ground or over the cellar at the monthly mean external temperature.
        /// </summary>
        public const string GroundOtherSideCoefficients = "Ground";

        /// <summary>
        /// The name of the other-side coefficients that hold the outside face of the thermal-bridge surface at the outdoor dry-bulb temperature.
        /// </summary>
        public const string ThermalBridgeOtherSideCoefficients = "Thermal Bridges";

        /// <summary>
        /// The name of the hidden surface that carries the thermal bridges.
        /// </summary>
        public const string ThermalBridgeSurface = "Thermal Bridges";
    }
}
