namespace ERHMS.Console.Utilities
{
    public class DecryptFile : Utility
    {
        public string InputPath { get; }
        public string OutputPath { get; }
        public string Password { get; }

        public DecryptFile(string inputPath, string outputPath, string password)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            Password = password;
        }

        public DecryptFile(string inputPath, string outputPath)
            : this(inputPath, outputPath, "") { }

        public override void Run()
        {
            Epi.Configuration.DecryptFile(InputPath, OutputPath, Password);
        }
    }
}
