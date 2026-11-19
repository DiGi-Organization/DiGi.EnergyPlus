using System.IO;

namespace DiGi.EnergyPlus
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the path of the EnergyPlus executable from <see cref="Constants.FileName.ExecutableConfiguration"/> in a directory (DiGi-Organization/DiGi.EnergyPlus#1). The path is a machine setting, so it lives in a git-ignored <c>user files</c> conf and never in code.
        /// <para>The conf holds <c>key=value</c> lines; blank lines and lines starting with <c>#</c> are skipped, and the key <see cref="Constants.Name.ExecutablePathKey"/> names the executable, for example <c>ExecutablePath=C:\EnergyPlusV26-2-0\energyplus.exe</c>.</para>
        /// </summary>
        /// <param name="directory">The directory holding the conf, typically a <c>user files</c> directory. This value can be null.</param>
        /// <returns>The path of the executable, or null when the directory, the conf, its key or the executable does not exist.</returns>
        public static string? ExecutablePath(string? directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return null;
            }

            string path_Configuration = Path.Combine(directory, Constants.FileName.ExecutableConfiguration);
            if (!File.Exists(path_Configuration))
            {
                return null;
            }

            foreach (string line in File.ReadAllLines(path_Configuration))
            {
                string line_Trimmed = line.Trim();
                if (line_Trimmed.Length == 0 || line_Trimmed.StartsWith('#'))
                {
                    continue;
                }

                int index = line_Trimmed.IndexOf('=');
                if (index <= 0 || line_Trimmed.Substring(0, index).Trim() != Constants.Name.ExecutablePathKey)
                {
                    continue;
                }

                string path = line_Trimmed.Substring(index + 1).Trim().Trim('"');
                return File.Exists(path) ? path : null;
            }

            return null;
        }
    }
}
