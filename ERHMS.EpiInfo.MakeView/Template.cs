using Epi;
using Epi.Windows.MakeView;
using Epi.Windows.MakeView.PresentationLogic;
using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ERHMS.EpiInfo.MakeView
{
    internal class Template
    {
        object @base;

        public Template(GuiMediator mediator)
        {
            Type type = Assembly.GetAssembly(typeof(MakeViewWindowsModule)).GetType("Epi.Windows.MakeView.Template");
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(GuiMediator) });
            @base = constructor.Invoke(new object[] { mediator });
        }

        public void InstantiateTemplate(string path)
        {
            Log.Current.DebugFormat("Instantiating template: {0}", path);
            ReflectionExtensions.Invoke(@base, "CreateFromTemplate", new Type[] { typeof(string) }, new object[] { path });
        }

        public void InstantiateTemplate(TemplateInfo templateInfo)
        {
            InstantiateTemplate(templateInfo.File.FullName);
        }

        public void CreateTemplate(View view, string templateName, string description)
        {
            Log.Current.DebugFormat("Creating template: {0}, {1}", view.Name, templateName);
            ReflectionExtensions.Invoke(@base, "CreateViewTemplate", new Type[] { typeof(string), typeof(View) }, new object[] { templateName, view });
            Configuration configuration = Configuration.GetNewInstance();
            string path = Path.Combine(
                configuration.Directories.Templates,
                "Forms",
                string.Format("{0}{1}", templateName, TemplateInfo.FileExtension));
            XmlDocument document = new XmlDocument();
            document.Load(path);
            document.DocumentElement.SetAttribute("Description", description);
            document.Save(path);
        }

        public string CreateWebTemplate()
        {
            Log.Current.Debug("Creating web template");
            return (string)ReflectionExtensions.Invoke(@base, "CreateWebSurveyTemplate");
        }
    }
}
