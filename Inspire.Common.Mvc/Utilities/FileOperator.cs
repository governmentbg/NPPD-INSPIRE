namespace Inspire.Common.Mvc.Utilities
{
    using System;
    using System.Configuration;
    using System.IO;

    public static class FileOperator
    {
        public static string TempDirectory => ConfigurationManager.AppSettings["TempFolderPath"] != null
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TempFolderPath"])
            : null;

        public static string ReadFile(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var streamReader = new StreamReader(fileStream, true))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}