using Epi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public abstract class NameGenerator<TSuffix>
    {
        protected static ISet<string> GetSet(IEnumerable<string> values)
        {
            return new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        }

        protected virtual Regex Regex { get; } = null;
        protected virtual string BaseName { get; } = null;
        protected virtual string Separator { get; } = "";
        protected abstract TSuffix Start { get; }

        public abstract bool Exists(string name);
        protected abstract TSuffix Next(TSuffix suffix);

        public string GetUniqueName(string name)
        {
            string baseName = BaseName;
            TSuffix suffix = Start;
            if (baseName == null)
            {
                Match match = Regex.Match(name);
                baseName = match.Groups["baseName"].Value;
                Group suffixGroup = match.Groups["suffix"];
                if (suffixGroup.Success)
                {
                    suffix = (TSuffix)Convert.ChangeType(suffixGroup.Value, typeof(TSuffix));
                }
            }
            while (true)
            {
                name = $"{baseName}{Separator}{suffix}";
                if (!Exists(name))
                {
                    return name;
                }
                suffix = Next(suffix);
            }
        }
    }

    public abstract class DigitSuffixNameGenerator : NameGenerator<int>
    {
        protected override Regex Regex { get; } = new Regex(@"^(?<baseName>.+?)(?<suffix>[0-9]+)?$");
        protected override int Start { get; } = 1;

        protected override int Next(int suffix)
        {
            return suffix + 1;
        }
    }

    public abstract class LetterSuffixNameGenerator : NameGenerator<string>
    {
        protected override Regex Regex { get; } = new Regex(@"^(?<baseName>.+?)(?:_(?<suffix>[A-Z]+))?$");
        protected override string Separator { get; } = "_";
        protected override string Start { get; } = "A";

        protected override string Next(string suffix)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            IList<int> codes = suffix.Select(ch => alphabet.IndexOf(ch)).ToList();
            for (int index = suffix.Length - 1; index >= 0; index--)
            {
                int code = codes[index] + 1;
                if (code == alphabet.Length)
                {
                    codes[index] = 0;
                }
                else
                {
                    codes[index] = code;
                    break;
                }
            }
            if (codes.All(index => index == 0))
            {
                codes.Add(0);
            }
            return string.Concat(codes.Select(index => alphabet[index]));
        }
    }

    public class TableNameGenerator : DigitSuffixNameGenerator
    {
        private ISet<string> tableNames;

        public TableNameGenerator(Project project)
        {
            tableNames = GetSet(project.CollectedData.GetDbDriver().GetTableNames());
        }

        public override bool Exists(string name)
        {
            return tableNames.Contains(name);
        }
    }

    public class ViewNameGenerator : LetterSuffixNameGenerator
    {
        private static readonly Regex TrailingDigitsRegex = new Regex(@"[0-9]+$");

        private ISet<string> tableNames;
        private ISet<string> viewNames;

        public ViewNameGenerator(Project project)
        {
            tableNames = GetSet(project.CollectedData.GetDbDriver().GetTableNames());
            viewNames = GetSet(project.Views.Names);
        }

        public override bool Exists(string name)
        {
            return tableNames.Contains(name);
        }

        public bool Conflicts(string name)
        {
            string baseName = TrailingDigitsRegex.Replace(name, "");
            return viewNames.Contains(baseName);
        }
    }

    public class PageNameGenerator : NameGenerator<int>
    {
        private ISet<string> pageNames;

        protected override string BaseName { get; } = "Page";
        protected override string Separator { get; } = " ";
        protected override int Start { get; }

        public PageNameGenerator(View view)
        {
            pageNames = GetSet(view.Pages.Select(page => page.Name));
            Start = view.Pages.Count + 1;
        }

        public override bool Exists(string name)
        {
            return pageNames.Contains(name);
        }

        protected override int Next(int suffix)
        {
            return suffix + 1;
        }
    }

    public class FieldNameGenerator : DigitSuffixNameGenerator
    {
        private ISet<string> fieldNames;

        public FieldNameGenerator(View view)
        {
            fieldNames = GetSet(view.Fields.Names);
        }

        public override bool Exists(string name)
        {
            return fieldNames.Contains(name);
        }
    }
}
