using Epi;
using Epi.Windows.MakeView;
using Epi.Windows.MakeView.PresentationLogic;
using ERHMS.Utility;
using System;
using System.Reflection;
using System.Xml;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class Template
    {
        private object @base;

        public GuiMediator Mediator { get; private set; }

        public Template(GuiMediator mediator)
        {
            Type type = Assembly.GetAssembly(typeof(MakeViewWindowsModule)).GetType("Epi.Windows.MakeView.Template");
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(GuiMediator) });
            @base = constructor.Invoke(new object[] { mediator });
            Mediator = mediator;
        }

        public void InstantiateTemplate(string path)
        {
            Log.Logger.DebugFormat("Instantiating template: {0}", path);
            Invoker invoker = new Invoker
            {
                Object = @base,
                MethodName = "CreateFromTemplate",
                ArgTypes = new Type[] { typeof(string) }
            };
            invoker.Invoke(path);
        }

        public string CreateTemplate(View view, string templateName, string description)
        {
            Log.Logger.DebugFormat("Creating template: {0}, {1}", view.Name, templateName);
            Invoker invoker = new Invoker
            {
                Object = @base,
                MethodName = "CreateViewTemplate",
                ArgTypes = new Type[] { typeof(string), typeof(View) }
            };
            invoker.Invoke(templateName, view);
            string path = TemplateInfo.GetPath(TemplateLevel.View, templateName);
            XmlDocument document = new XmlDocument();
            document.Load(path);
            document.DocumentElement.SetAttribute("Description", description);
            document.Save(path);
            return path;
        }

        public string CreateWebTemplate()
        {
            Log.Logger.Debug("Creating web template");
            Invoker invoker = new Invoker
            {
                Object = @base,
                MethodName = "CreateWebSurveyTemplate"
            };
            return (string)invoker.Invoke();
        }
    }
}
