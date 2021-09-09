using Epi;

namespace ERHMS.Console.Utilities
{
    public class DecryptFile : Utility
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public DecryptFile(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public override void Run()
        {
            Configuration.DecryptFile(InputPath, OutputPath, GetPassword());
        }
    }
}
