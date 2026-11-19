namespace DiGi.EnergyPlus.Constants
{
    /// <summary>
    /// Names of the files of an EnergyPlus run (DiGi-Organization/DiGi.EnergyPlus#1).
    /// </summary>
    public static class FileName
    {
        /// <summary>
        /// The input file written into the run directory.
        /// </summary>
        public const string Input = "in.idf";

        /// <summary>
        /// The weather file copied into the run directory.
        /// </summary>
        public const string Weather = "in.epw";

        /// <summary>
        /// The error file EnergyPlus writes: version, warnings, severe errors and whether the run completed.
        /// </summary>
        public const string Error = "eplusout.err";

        /// <summary>
        /// The variables EnergyPlus reports, written by ReadVarsESO.
        /// </summary>
        public const string Variables = "eplusout.csv";

        /// <summary>
        /// The tabular reports EnergyPlus writes in the comma style.
        /// </summary>
        public const string Tables = "eplustbl.csv";

        /// <summary>
        /// The git-ignored conf naming the EnergyPlus executable on this machine, read from a <c>user files</c> directory.
        /// </summary>
        public const string ExecutableConfiguration = "EnergyPlus_Executable.conf";
    }
}
