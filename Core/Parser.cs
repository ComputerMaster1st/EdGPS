namespace EdGps.Core
{
    public static class Parser
    {
        public static string ParseDirectoryPath(string directoryPath) {
            var charsToRemove = new string[] { "'", "&", "\"" };
            var sanitizePath = directoryPath;

            foreach (var c in charsToRemove)
                sanitizePath = sanitizePath.Replace(c, string.Empty);

            return sanitizePath.Trim(' ');
        }
    }
}