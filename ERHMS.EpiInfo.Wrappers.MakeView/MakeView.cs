using Epi.Windows.MakeView.Dialogs;
using ERHMS.Utility;
using System;
using System.Windows.Forms;
using System.Xml;

namespace ERHMS.EpiInfo.Wrappers
{
    public class MakeView : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(MakeView), args);
        }

        public static Wrapper OpenView(string projectPath, string viewName)
        {
            return Create(() => Main_OpenView(projectPath, viewName));
        }
        private static void Main_OpenView(string projectPath, string viewName)
        {
            MainForm form = new MainForm();
            form.OpenProject(projectPath);
            form.ProjectExplorer.SelectView(viewName);
            Application.Run(form);
        }

        public static Wrapper CreateTemplate(string projectPath, string viewName)
        {
            return Create(() => Main_CreateTemplate(projectPath, viewName));
        }
        private static void Main_CreateTemplate(string projectPath, string viewName)
        {
            MainForm form = new MainForm();
            Project project = new Project(projectPath);
            form.OpenProject(projectPath);
            form.ProjectExplorer.SelectView(viewName);
            form.Shown += (sender, e) =>
            {
                using (CreateTemplateDialog dialog = new CreateTemplateDialog(viewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        Template template = new Template(form.Mediator);
                        template.CreateTemplate(form.CurrentView, dialog.TemplateName, dialog.Description);
                        RaiseEvent(WrapperEventType.TemplateCreated);
                        string message = "Template has been created. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            form.Close();
                        }
                    }
                    else
                    {
                        string message = "Template has not been created. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            form.Close();
                        }
                    }
                }
            };
            Application.Run(form);
        }

        public static Wrapper CreateWebTemplate(string projectPath, string viewName)
        {
            return Create(() => Main_CreateWebTemplate(projectPath, viewName));
        }
        private static void Main_CreateWebTemplate(string projectPath, string viewName)
        {
            MainForm form = new MainForm(false);
            Project project = new Project(projectPath);
            form.OpenProject(project);
            form.ProjectExplorer.SelectView(viewName);
            Template template = new Template(form.Mediator);
            Out.Write(template.CreateWebTemplate());
        }

        public static Wrapper InstantiateProjectTemplate(string projectPath, string templatePath)
        {
            return Create(() => Main_InstantiateProjectTemplate(projectPath, templatePath));
        }
        private static void Main_InstantiateProjectTemplate(string projectPath, string templatePath)
        {
            MainForm form = new MainForm(false);
            Project project = new Project(projectPath);
            form.OpenProject(project);
            Template template = new Template(form.Mediator);
            template.InstantiateTemplate(templatePath);
        }

        public static Wrapper InstantiateViewTemplate(string projectPath, string templatePath, string namePrefix)
        {
            return Create(() => Main_InstantiateViewTemplate(projectPath, templatePath, namePrefix));
        }
        private static void Main_InstantiateViewTemplate(string projectPath, string templatePath, string namePrefix)
        {
            MainForm form = new MainForm();
            Project project = new Project(projectPath);
            form.OpenProject(project);
            XmlDocument document = new XmlDocument();
            document.Load(templatePath);
            XmlElement viewElement = document.SelectSingleElement("/Template/Project/View");
            string viewName = project.SuggestViewName(namePrefix + viewElement.GetAttribute("Name"));
            form.Shown += (sender, e) =>
            {
                using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        viewElement.SetAttribute("Name", dialog.ViewName);
                        string tempTemplatePath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
                        document.Save(tempTemplatePath);
                        Template template = new Template(form.Mediator);
                        template.InstantiateTemplate(tempTemplatePath);
                        RaiseEvent(WrapperEventType.ViewCreated);
                        string message = "Form has been created. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            form.Close();
                        }
                    }
                    else
                    {
                        string message = "Form has not been created. Close Epi Info?";
                        if (MessageBox.Show(form, message, "Close?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            form.Close();
                        }
                    }
                }
            };
            Application.Run(form);
        }

        public static Wrapper PublishToMobile(string projectPath, string viewName)
        {
            return Create(() => Main_PublishToMobile(projectPath, viewName));
        }
        private static void Main_PublishToMobile(string projectPath, string viewName)
        {
            MainForm form = new MainForm(false);
            Project project = new Project(projectPath);
            form.OpenProject(project);
            form.ProjectExplorer.SelectView(viewName);
            form.Shown += (sender, e) =>
            {
                using (CopyToAndroid dialog = new CopyToAndroid(form.CurrentView, form.Mediator))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.ShowDialog(form);
                }
            };
            Application.Run(form);
        }
    }
}
