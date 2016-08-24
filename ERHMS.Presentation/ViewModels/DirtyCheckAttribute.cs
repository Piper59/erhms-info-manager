using System;

namespace ERHMS.Presentation.ViewModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DirtyCheckAttribute : Attribute { }
}
