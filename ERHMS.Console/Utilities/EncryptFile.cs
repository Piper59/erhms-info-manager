namespace ERHMS.Console.Utilities
{
    public class EncryptFile : IUtility
    {
        public string InputPath { get; }
        public string OutputPath { get; }
        public string Password { get; }

        public EncryptFile(string inputPath, string outputPath, string password)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            Password = password;
        }

        public EncryptFile(string inputPath, string outputPath)
            : this(inputPath, outputPath, "") { }

        public void Run()
        {
            Epi.Configuration.EncryptFile(InputPath, OutputPath, Password);
        }
    }
}
