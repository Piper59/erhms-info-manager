using System.Windows;

namespace ERHMS.Sandbox
{
    public class Service : IService
    {
        public void SayHello(string name)
        {
            MessageBox.Show(string.Format("Hello, {0}!", name));
        }
    }
}
