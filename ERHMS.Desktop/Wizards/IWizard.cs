namespace ERHMS.Desktop.Wizards
{
    public interface IWizard
    {
        IStep Step { get; }
        bool? Result { get; }
    }
}
