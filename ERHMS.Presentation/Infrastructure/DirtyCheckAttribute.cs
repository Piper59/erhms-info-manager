using System;

namespace ERHMS.Presentation
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DirtyCheckAttribute : Attribute { }
}
