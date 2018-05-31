using Epi;
using Epi.Fields;
using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class MergeSelectedViewModel : DocumentViewModel
    {
        public class PropertyChildViewModel : ViewModelBase
        {
            private string name;
            public string Name
            {
                get { return name; }
                set { SetProperty(nameof(Name), ref name, value); }
            }

            private object value1;
            public object Value1
            {
                get { return value1; }
                set { SetProperty(nameof(Value1), ref value1, value); }
            }

            private object value2;
            public object Value2
            {
                get { return value2; }
                set { SetProperty(nameof(Value2), ref value2, value); }
            }

            private string text1;
            public string Text1
            {
                get { return text1; }
                set { SetProperty(nameof(Text1), ref text1, value); }
            }

            private string text2;
            public string Text2
            {
                get { return text2; }
                set { SetProperty(nameof(Text2), ref text2, value); }
            }

            private bool selected1;
            public bool Selected1
            {
                get
                {
                    return selected1;
                }
                set
                {
                    SetProperty(nameof(Selected1), ref selected1, value);
                    if (value)
                    {
                        Selected2 = false;
                    }
                }
            }

            private bool selected2;
            public bool Selected2
            {
                get
                {
                    return selected2;
                }
                set
                {
                    SetProperty(nameof(Selected2), ref selected2, value);
                    if (value)
                    {
                        Selected1 = false;
                    }
                }
            }

            public PropertyChildViewModel(string name, Responder responder1, Responder responder2)
            {
                Name = name;
                Value1 = responder1.GetProperty(name);
                Value2 = responder2.GetProperty(name);
                Text1 = Convert.ToString(Value1);
                Text2 = Convert.ToString(Value2);
            }

            public object GetSelectedValue()
            {
                if (Selected1)
                {
                    return Value1;
                }
                else if (Selected2)
                {
                    return Value2;
                }
                else
                {
                    throw new InvalidOperationException("Neither value is selected.");
                }
            }
        }

        public Responder Responder1 { get; private set; }
        public Responder Responder2 { get; private set; }
        public ICollection<PropertyChildViewModel> Properties { get; private set; }

        public ICommand SelectAll1Command { get; private set; }
        public ICommand SelectAll2Command { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public MergeSelectedViewModel(Responder responder1, Responder responder2)
        {
            Title = "Merge";
            Responder1 = responder1;
            Responder2 = responder2;
            Properties = new List<PropertyChildViewModel>();
            List<int> fieldIds = Context.Project.GetSortedFieldIds(Context.Responders.View.Id).ToList();
            IEnumerable<string> propertyNames = Context.Responders.View.Fields.InputFields
                .Cast<Field>()
                .OrderBy(field => fieldIds.IndexOf(field.Id))
                .Select(field => field.Name)
                .Prepend(ColumnNames.GLOBAL_RECORD_ID);
            foreach (string propertyName in propertyNames)
            {
                Properties.Add(new PropertyChildViewModel(propertyName, responder1, responder2));
            }
            SelectAll1Command = new Command(SelectAll1);
            SelectAll2Command = new Command(SelectAll2);
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        public event EventHandler Saved;
        private void OnSaved(EventArgs e)
        {
            Saved?.Invoke(this, e);
        }
        private void OnSaved()
        {
            OnSaved(EventArgs.Empty);
        }

        public void SelectAll1()
        {
            foreach (PropertyChildViewModel property in Properties)
            {
                property.Selected1 = true;
            }
        }

        public void SelectAll2()
        {
            foreach (PropertyChildViewModel property in Properties)
            {
                property.Selected2 = true;
            }
        }

        public async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            foreach (PropertyChildViewModel property in Properties)
            {
                if (!property.Selected1 && !property.Selected2)
                {
                    fields.Add(property.Name);
                }
            }
            if (fields.Count > 0)
            {
                await ServiceLocator.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            {
                Responder responder = Properties.First().Selected1 ? Responder1 : Responder2;
                foreach (PropertyChildViewModel property in Properties)
                {
                    responder.SetProperty(property.Name, property.GetSelectedValue());
                }
                Context.Responders.Save(responder);
            }
            {
                Responder responder = Properties.First().Selected1 ? Responder2 : Responder1;
                responder.Deleted = true;
                Context.Responders.Save(responder);
            }
            ServiceLocator.Dialog.Notify(Resources.ResponderPairMerged);
            ServiceLocator.Data.Refresh(typeof(Responder));
            OnSaved();
            await CloseAsync();
        }
    }
}
