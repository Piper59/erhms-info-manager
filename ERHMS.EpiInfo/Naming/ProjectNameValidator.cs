using System.IO;

namespace ERHMS.EpiInfo.Naming
{
    public class ProjectNameValidator : NameValidator
    {
        public static int MaxLength => 64;

        public string LocationRoot { get; }

        public ProjectNameValidator(string locationRoot)
        {
            LocationRoot = locationRoot;
        }

        protected override int GetMaxLength()
        {
            return MaxLength;
        }

        public bool IsIdentical(string name)
        {
            ProjectInfo projectInfo = new ProjectInfo(LocationRoot, name);
            return File.Exists(projectInfo.FilePath);
        }

        public override bool IsValid(string name, out InvalidNameReason reason)
        {
            if (!base.IsValid(name, out reason))
            {
                return false;
            }
            if (IsIdentical(name))
            {
                reason = InvalidNameReason.Identical;
                return false;
            }
            reason = InvalidNameReason.None;
            return true;
        }
    }
}
