using Epi.Windows.Enter.PresentationLogic;
using System;
using System.Drawing.Printing;
using System.Reflection;

namespace ERHMS.Console.Infrastructure
{
    internal static class GuiMediatorExtensions
    {
        private static bool ProcessPrintRequest(
            this GuiMediator @this,
            int recordStart,
            int recordEnd,
            int pageNumberStart,
            int pageNumberEnd)
        {
            MethodInfo method = typeof(GuiMediator).GetMethod(
                "ProcessPrintRequest",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int)
                },
                null);
            return (bool)method.Invoke(@this, new object[]
            {
                recordStart,
                recordEnd,
                pageNumberStart,
                pageNumberEnd
            });
        }

        private static PrintDocument GetPrintDocument(this GuiMediator @this)
        {
            FieldInfo field = typeof(GuiMediator).GetField(
                "printDocument",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (PrintDocument)field.GetValue(@this);
        }

        public static void PrintToFile(this GuiMediator @this, string path)
        {
            @this.ProcessPrintRequest(-1, -1, 1, @this.View.Pages.Count);
            using (PrintDocument document = @this.GetPrintDocument())
            {
                document.PrinterSettings = new PrinterSettings
                {
                    PrintToFile = true,
                    PrintFileName = path
                };
                document.Print();
            }
        }
    }
}
