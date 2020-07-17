using Epi;
using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public abstract class NameGenerator<TSuffix>
    {
        protected static ISet<string> ToSet(IEnumerable<string> names)
        {
            return new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
        }

        protected static IEnumerable<string> GetTableNames(Project project)
        {
            return project.CollectedData.GetDbDriver().GetTableNames();
        }

        protected static IEnumerable<string> GetViewNames(Project project)
        {
            return project.Views.Cast<View>().Select(view => view.Name);
        }

        protected static IEnumerable<string> GetTableAndViewNames(Project project)
        {
            return Enumerable.Concat(GetTableNames(project), GetViewNames(project));
        }

        protected Regex Regex { get; set; }
        protected string BaseName { get; set; }
        protected string Separator { get; set; }
        protected TSuffix StartSuffix { get; set; }
        protected ISet<string> Names { get; set; }

        protected virtual string Format(string baseName, TSuffix suffix)
        {
            return $"{baseName}{Separator}{suffix}";
        }

        protected abstract TSuffix ParseSuffix(string value);

        public virtual bool Exists(string name)
        {
            return Names.Contains(name);
        }

        protected virtual void Add(string name)
        {
            Names.Add(name);
        }

        protected abstract TSuffix GetNextSuffix(TSuffix suffix);

        public virtual string MakeUnique(string name)
        {
            string baseName = BaseName;
            TSuffix suffix = StartSuffix;
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
                name = Format(baseName, suffix);
                if (!Exists(name))
                {
                    Add(name);
                    return name;
                }
                suffix = GetNextSuffix(suffix);
            }
        }
    }

    public abstract class IntSuffixNameGenerator : NameGenerator<int>
    {
        protected IntSuffixNameGenerator()
        {
            Regex = new Regex(@"^(?<baseName>.+?)(?<suffix>[0-9]+)?$");
            Separator = "";
            StartSuffix = 1;
        }

        protected override int ParseSuffix(string value)
        {
            return int.Parse(value);
        }

        protected override int GetNextSuffix(int suffix)
        {
            return suffix + 1;
        }
    }

    public abstract class CharSuffixNameGenerator : NameGenerator<char>
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        protected CharSuffixNameGenerator()
        {
            Regex = new Regex(@"^(?<baseName>.+?)(?:_(?<suffix>[A-Z]))?$", RegexOptions.IgnoreCase);
            Separator = "_";
            StartSuffix = 'A';
        }

        protected override char ParseSuffix(string value)
        {
            return value.Single();
        }

        protected override char GetNextSuffix(char suffix)
        {
            int index = Alphabet.IndexOf(char.ToUpper(suffix));
            char nextSuffix = Alphabet[index + 1];
            return char.IsLower(suffix) ? char.ToLower(nextSuffix) : nextSuffix;
        }

        public override string MakeUnique(string name)
        {
            while (true)
            {
                try
                {
                    return base.MakeUnique(name);
                }
                catch (IndexOutOfRangeException)
                {
                    name = Format(name, StartSuffix);
                }
            }
        }
    }

    public class TableNameGenerator : IntSuffixNameGenerator
    {
        public TableNameGenerator(Project project)
        {
            Names = ToSet(GetTableAndViewNames(project));
        }
    }

    public class ViewNameGenerator : CharSuffixNameGenerator
    {
        private static readonly Regex TrailingDigitsRegex = new Regex(@"[0-9]+$");

        private ISet<string> viewNames;

        public ViewNameGenerator(Project project)
        {
            Names = ToSet(GetTableAndViewNames(project));
            viewNames = ToSet(GetViewNames(project));
        }

        public bool Conflicts(string name)
        {
            string baseName = TrailingDigitsRegex.Replace(name, "");
            return viewNames.Contains(baseName);
        }

        protected override void Add(string name)
        {
            base.Add(name);
            viewNames.Add(name);
        }
    }

    public class PageNameGenerator : IntSuffixNameGenerator
    {
        public PageNameGenerator(View view)
        {
            Regex = null;
            BaseName = "Page";
            Separator = " ";
            StartSuffix = view.Pages.Count + 1;
            Names = ToSet(view.Pages.Select(page => page.Name));
        }
    }

    public class FieldNameGenerator : IntSuffixNameGenerator
    {
        public FieldNameGenerator(View view)
        {
            Names = ToSet(view.Fields.Cast<Field>().Select(field => field.Name));
        }
    }
}
