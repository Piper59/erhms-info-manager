using Epi;

namespace ERHMS.Console.Utilities
{
    public class EncryptFile : Utility
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public EncryptFile(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public override void Run()
        {
            Configuration.EncryptFile(InputPath, OutputPath, GetPassword());
        }
    }
}
