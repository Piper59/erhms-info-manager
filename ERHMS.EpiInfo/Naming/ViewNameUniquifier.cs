using Epi;

namespace ERHMS.EpiInfo.Naming
{
    public class ViewNameUniquifier : NameUniquifier.CharSuffixed
    {
        private readonly ViewNameValidator validator;

        public Project Project { get; }

        public ViewNameUniquifier(Project project)
        {
            Project = project;
            validator = new ViewNameValidator(project);
        }

        public override bool Exists(string name)
        {
            return validator.IsIdentical(name) || validator.IsSimilar(name);
        }
    }
}
