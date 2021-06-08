using ERHMS.Common.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ERHMS.Common.Naming
{
    public class FileNameUniquifier : NameUniquifier.IntSuffixed
    {
        private readonly Regex nameRegex;
        protected override Regex NameRegex => nameRegex;

        public string DirectoryPath { get; }
        public string Extension { get; }

        public FileNameUniquifier(string directoryPath, string extension)
        {
            nameRegex = new Regex(
                $@"^(?<baseName>.+) \((?<suffix>[0-9]+)\){Regex.Escape(extension)}$",
                RegexOptions.IgnoreCase);
            DirectoryPath = directoryPath;
            Extension = extension;
        }

        protected override string GetInitialBaseName(string name)
        {
            string extension = Path.GetExtension(name);
            if (!Comparers.Path.Equals(extension, Extension))
            {
                throw new ArgumentException(
                    $"Specified extension '{extension}' does not match expected extension '{Extension}'.",
                    nameof(name));
            }
            return Path.GetFileNameWithoutExtension(name);
        }

        protected override string Format(string baseName, int suffix)
        {
            return $"{baseName} ({suffix}){Extension}";
        }

        public override bool Exists(string name)
        {
            return File.Exists(Path.Combine(DirectoryPath, name));
        }
    }
}
