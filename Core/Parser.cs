namespace EdGps.Core
{
    public static class Parser
    {
        public static string ParseDirectoryPath(string directoryPath)
            => directoryPath.Trim(new char[] { '\'', '&', '"' })
                .TrimStart(' ')
                .TrimEnd(' ');
    }
}