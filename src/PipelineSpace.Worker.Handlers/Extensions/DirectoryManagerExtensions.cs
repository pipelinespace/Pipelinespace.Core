using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PipelineSpace.Worker.Handlers.Extensions
{
    public static class DirectoryManagerExtensions
    {
        private static readonly Dictionary<string, string> toRename = new Dictionary<string, string>
        {
            { "dot_git", ".git" },
            { "gitmodules", ".gitmodules" },
        };

        private static readonly Type[] whitelist = { typeof(IOException), typeof(UnauthorizedAccessException) };

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            // From http://stackoverflow.com/questions/58744/best-way-to-copy-the-entire-contents-of-a-directory-in-c/58779#58779

            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(Rename(dir.Name)));
            }
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, Rename(file.Name)));
            }
        }

        private static string Rename(string name)
        {
            return toRename.ContainsKey(name) ? toRename[name] : name;
        }

        public static void DeleteDirectory(string directoryPath)
        {
            // From http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true/329502#329502

            if (!Directory.Exists(directoryPath))
            {
                Trace.WriteLine(string.Format("Directory '{0}' is missing and can't be removed.", directoryPath));
                return;
            }
            NormalizeAttributes(directoryPath);
            DeleteDirectory(directoryPath, maxAttempts: 5, initialTimeout: 16, timeoutFactor: 2);
        }

        private static void NormalizeAttributes(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                NormalizeAttributes(subdirectoryPath);
            }
            File.SetAttributes(directoryPath, FileAttributes.Normal);
        }

        private static void DeleteDirectory(string directoryPath, int maxAttempts, int initialTimeout, int timeoutFactor)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                    return;
                }
                catch (Exception ex)
                {
                    var caughtExceptionType = ex.GetType();

                    if (!whitelist.Any(knownExceptionType => knownExceptionType.IsAssignableFrom(caughtExceptionType)))
                    {
                        throw;
                    }

                    if (attempt < maxAttempts)
                    {
                        Thread.Sleep(initialTimeout * (int)Math.Pow(timeoutFactor, attempt - 1));
                        continue;
                    }

                    Trace.WriteLine(string.Format("{0}The directory '{1}' could not be deleted ({2} attempts were made) due to a {3}: {4}" +
                                                  "{0}Most of the time, this is due to an external process accessing the files in the temporary repositories created during the test runs, and keeping a handle on the directory, thus preventing the deletion of those files." +
                                                  "{0}Known and common causes include:" +
                                                  "{0}- Windows Search Indexer (go to the Indexing Options, in the Windows Control Panel, and exclude the bin folder of LibGit2Sharp.Tests)" +
                                                  "{0}- Antivirus (exclude the bin folder of LibGit2Sharp.Tests from the paths scanned by your real-time antivirus)" +
                                                  "{0}- TortoiseGit (change the 'Icon Overlays' settings, e.g., adding the bin folder of LibGit2Sharp.Tests to 'Exclude paths' and appending an '*' to exclude all subfolders as well)",
                        Environment.NewLine, Path.GetFullPath(directoryPath), maxAttempts, caughtExceptionType, ex.Message));
                }
            }
        }

        public static void RenameDirectory(string repositoryPath, string organizationName, string projectName, string serviceName)
        {
            string[] directories = Directory.GetDirectories(repositoryPath, "*", SearchOption.AllDirectories).OrderBy(x => x.Length).ToArray();
            foreach (var directory in directories)
            {
                string originalDirectoryName = directory;
                string modifiedDirectoryName = string.Empty;
                modifiedDirectoryName = Regex.Replace(originalDirectoryName, @"ProjectPS", projectName, RegexOptions.IgnoreCase);
                modifiedDirectoryName = Regex.Replace(modifiedDirectoryName, @"ServicePS", serviceName, RegexOptions.IgnoreCase);

                if (!originalDirectoryName.Equals(modifiedDirectoryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (Directory.Exists(originalDirectoryName))
                    {
                        Directory.Move(originalDirectoryName, modifiedDirectoryName);
                    }
                }
            }

            string[] files = Directory.GetFiles(repositoryPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string contents = File.ReadAllText(file);
                contents = Regex.Replace(contents, @"OrganizationPS", organizationName, RegexOptions.IgnoreCase);
                contents = Regex.Replace(contents, @"ProjectPS", projectName, RegexOptions.IgnoreCase);
                contents = Regex.Replace(contents, @"ServicePS", serviceName, RegexOptions.IgnoreCase);

                /*functions*/
                contents = Regex.Replace(contents, @"$(PS.Organization).ToLower()", organizationName.ToLower(), RegexOptions.IgnoreCase);
                contents = Regex.Replace(contents, @"$(PS.Project).ToLower()", projectName.ToLower(), RegexOptions.IgnoreCase);
                contents = Regex.Replace(contents, @"$(PS.Service).ToLower()", serviceName.ToLower(), RegexOptions.IgnoreCase);
                
                // Make files writable
                File.SetAttributes(file, FileAttributes.Normal);

                File.WriteAllText(file, contents);

                string originalFileName = file;
                string modifiedFileName = string.Empty;
                modifiedFileName = Regex.Replace(originalFileName, @"OrganizationPS", organizationName, RegexOptions.IgnoreCase);
                modifiedFileName = Regex.Replace(originalFileName, @"ProjectPS", projectName, RegexOptions.IgnoreCase);
                modifiedFileName = Regex.Replace(modifiedFileName, @"ServicePS", serviceName, RegexOptions.IgnoreCase);

                if (!originalFileName.Equals(modifiedFileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    File.Move(originalFileName, modifiedFileName);
                }
            }
        }
    }
}
