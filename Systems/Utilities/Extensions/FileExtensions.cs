using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Helpful methods for working with file IO. </summary>
    public static class FileExtensions
    {
        /// <summary> Search the given directory, recursively, for files. </summary>
        /// <param name="directoryPath"> The Godot filepath to seach. </param>
        /// <param name="extensions"> A list of allowed extensions. A null means to accept everything. </param>
        /// <returns> The filepaths of all the found extensions. </returns>
        /// <exception cref="DirectoryNotFoundException"> If the given directory isn't found. </exception>
        public static String[] GetResources(String directoryPath, String[]? extensions = null)
        {
            DirAccess dataDirectory = DirAccess.Open(directoryPath);
            if (dataDirectory == null)
            {
                throw new DirectoryNotFoundException($"The '{directoryPath}' directory does not exist! Something has gone very wrong Captain.");
            }

            List<String> resources = new List<String>();
            dataDirectory.ListDirBegin();
            String current = dataDirectory.GetNext();
            while (!String.IsNullOrEmpty(current))
            {
                String currentPath = directoryPath + '/' + current;
                if (dataDirectory.CurrentIsDir())
                {
                    resources.AddRange(GetResources(currentPath, extensions));
                }
                else
                {
                    // Only add the file if it's part of the acceptable extensions.
                    if (extensions == null || extensions.Contains(Path.GetExtension(currentPath)))
                    {
                        resources.Add(currentPath);
                    }
                }
                current = dataDirectory.GetNext();
            }
            dataDirectory.ListDirEnd();

            return resources.ToArray();
        }
    }
}
