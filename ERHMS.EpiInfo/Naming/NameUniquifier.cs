using System;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Naming
{
    public static class NameUniquifier
    {
        public abstract class Suffixed<TSuffix> : INameUniquifier
        {
            protected abstract Regex NameRegex { get; }
            protected abstract TSuffix InitialSuffix { get; }

            protected virtual string GetInitialBaseName(string name)
            {
                return name;
            }

            protected abstract TSuffix ParseSuffix(string value);
            protected abstract TSuffix GetNextSuffix(TSuffix suffix);
            protected abstract string Format(string baseName, TSuffix suffix);
            public abstract bool Exists(string name);

            public virtual string Uniquify(string name)
            {
                string baseName;
                TSuffix suffix;
                Match match = NameRegex.Match(name);
                if (match.Success)
                {
                    baseName = match.Groups["baseName"].Value;
                    suffix = GetNextSuffix(ParseSuffix(match.Groups["suffix"].Value));
                }
                else
                {
                    baseName = GetInitialBaseName(name);
                    suffix = InitialSuffix;
                }
                while (true)
                {
                    name = Format(baseName, suffix);
                    if (!Exists(name))
                    {
                        return name;
                    }
                    suffix = GetNextSuffix(suffix);
                }
            }
        }

        public abstract class CharSuffixed : Suffixed<char>
        {
            private static readonly string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            protected override Regex NameRegex { get; } =
                new Regex(@"^(?<baseName>.+?)(?:_(?<suffix>[A-Z]))$", RegexOptions.IgnoreCase);
            protected override char InitialSuffix => alphabet[1];

            protected override char ParseSuffix(string value)
            {
                return char.Parse(value);
            }

            protected override char GetNextSuffix(char suffix)
            {
                int index = alphabet.IndexOf(char.ToUpper(suffix));
                if (index == -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(suffix));
                }
                char nextSuffix = alphabet[index + 1];
                return char.IsLower(suffix) ? char.ToLower(nextSuffix) : nextSuffix;
            }

            protected override string Format(string baseName, char suffix)
            {
                return $"{baseName}_{suffix}";
            }

            public override string Uniquify(string name)
            {
                while (true)
                {
                    try
                    {
                        return base.Uniquify(name);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        name = Format(name, InitialSuffix);
                        if (!Exists(name))
                        {
                            return name;
                        }
                    }
                }
            }
        }

        public abstract class IntSuffixed : Suffixed<int>
        {
            protected override Regex NameRegex { get; } = new Regex(@"^(?<baseName>.*[^0-9])(?<suffix>[0-9]+)$");
            protected override int InitialSuffix => 2;

            protected override int ParseSuffix(string value)
            {
                return int.Parse(value);
            }

            protected override int GetNextSuffix(int suffix)
            {
                return suffix + 1;
            }

            protected override string Format(string baseName, int suffix)
            {
                return $"{baseName}{suffix}";
            }
        }
    }
}
