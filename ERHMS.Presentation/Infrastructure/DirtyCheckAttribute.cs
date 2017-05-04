using System;

namespace ERHMS.Presentation.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DirtyCheckAttribute : Attribute { }
}
