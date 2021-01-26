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
        protected static ISet<string> GetNameSet(params IEnumerable<string>[] sources)
        {
            return new HashSet<string>(sources.SelectMany(source => source), StringComparer.OrdinalIgnoreCase);
        }

        protected static IEnumerable<string> GetTableNames(Project project)
        {
            return project.CollectedData.GetDbDriver().GetTableNames();
        }

        protected static IEnumerable<string> GetViewNames(Project project)
        {
            return project.Views.Cast<View>().Select(view => view.Name);
        }

        protected abstract Regex Regex { get; }
        protected abstract string BaseName { get; }
        protected abstract string Separator { get; }
        protected abstract TSuffix StartSuffix { get; }
        protected abstract ISet<string> Names { get; }

        protected string Format(string baseName, TSuffix suffix)
        {
            return $"{baseName}{Separator}{suffix}";
        }

        protected abstract TSuffix ParseSuffix(string value);

        public virtual bool Exists(string name)
        {
            return Names.Contains(name);
        }

        public virtual void Add(string name)
        {
            Names.Add(name);
        }

        protected abstract TSuffix GetNextSuffix(TSuffix suffix);

        public virtual string Generate(string name)
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
                    suffix = ParseSuffix(suffixGroup.Value);
                }
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

    public abstract class IntSuffixNameGenerator : NameGenerator<int>
    {
        protected override Regex Regex { get; } = new Regex(@"^(?<baseName>.+?)(?<suffix>[0-9]+)?$");
        protected override string Separator => "";
        protected override int StartSuffix => 1;

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

        protected override Regex Regex { get; } = new Regex(@"^(?<baseName>.+?)(?:_(?<suffix>[a-zA-Z]))?$");
        protected override string Separator => "_";
        protected override char StartSuffix => 'A';

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

        public override string Generate(string name)
        {
            while (true)
            {
                try
                {
                    return base.Generate(name);
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
        protected override string BaseName => null;
        protected override ISet<string> Names { get; }

        public TableNameGenerator(Project project)
        {
            Names = GetNameSet(GetTableNames(project), GetViewNames(project));
        }
    }

    public class ViewNameGenerator : CharSuffixNameGenerator
    {
        public const int MaxLength = 50;

        private static readonly Regex invalidCharRegex = new Regex(@"[^a-zA-Z0-9_]");
        private static readonly Regex invalidStartCharRegex = new Regex(@"^[^a-zA-Z]");
        private static readonly Regex trailingDigitsRegex = new Regex(@"[0-9]+$");

        private readonly ISet<string> viewNames;
        private readonly ISet<string> tableAndViewNames;

        protected override string BaseName => null;
        protected override ISet<string> Names => viewNames;

        public ViewNameGenerator(Project project)
        {
            viewNames = GetNameSet(GetViewNames(project));
            tableAndViewNames = GetNameSet(GetTableNames(project), viewNames);
        }

        public bool IsIdentical(string name)
        {
            return tableAndViewNames.Contains(name);
        }

        public bool IsSimilar(string name)
        {
            return viewNames.Contains(trailingDigitsRegex.Replace(name, ""));
        }

        public override bool Exists(string name)
        {
            return IsIdentical(name) || IsSimilar(name);
        }

        public override void Add(string name)
        {
            viewNames.Add(name);
            tableAndViewNames.Add(name);
        }

        public bool IsValid(string name, out InvalidViewNameReason reason)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            reason = InvalidViewNameReason.None;
            if (name == "")
            {
                reason = InvalidViewNameReason.Empty;
            }
            else if (name.Length > MaxLength)
            {
                reason = InvalidViewNameReason.TooLong;
            }
            else if (invalidCharRegex.IsMatch(name))
            {
                reason = InvalidViewNameReason.InvalidChar;
            }
            else if (invalidStartCharRegex.IsMatch(name))
            {
                reason = InvalidViewNameReason.InvalidStartChar;
            }
            else if (IsIdentical(name))
            {
                reason = InvalidViewNameReason.Identical;
            }
            else if (IsSimilar(name))
            {
                reason = InvalidViewNameReason.Similar;
            }
            return reason == InvalidViewNameReason.None;
        }
    }

    public class PageNameGenerator : IntSuffixNameGenerator
    {
        protected override Regex Regex => null;
        protected override string BaseName => "Page";
        protected override string Separator => " ";
        protected override int StartSuffix { get; }
        protected override ISet<string> Names { get; }

        public PageNameGenerator(View view)
        {
            StartSuffix = view.Pages.Count + 1;
            Names = GetNameSet(view.Pages.Select(page => page.Name));
        }
    }

    public class FieldNameGenerator : IntSuffixNameGenerator
    {
        protected override string BaseName => null;
        protected override ISet<string> Names { get; }

        public FieldNameGenerator(View view)
        {
            Names = GetNameSet(view.Fields.Cast<Field>().Select(field => field.Name));
        }
    }
}
