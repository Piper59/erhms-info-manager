using Epi;
using Epi.Fields;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class MergeSelectedViewModel : ViewModelBase
    {
        public class PropertyChildViewModel : ViewModelBase
        {
            private string name;
            public string Name
            {
                get { return name; }
                set { Set(nameof(Name), ref name, value); }
            }

            private string value1;
            public string Value1
            {
                get { return value1; }
                set { Set(nameof(Value1), ref value1, value); }
            }

            private string value2;
            public string Value2
            {
                get { return value2; }
                set { Set(nameof(Value2), ref value2, value); }
            }

            private bool isSelected1;
            public bool IsSelected1
            {
                get
                {
                    return isSelected1;
                }
                set
                {
                    if (Set(nameof(IsSelected1), ref isSelected1, value))
                    {
                        if (value)
                        {
                            IsSelected2 = false;
                        }
                    }
                }
            }

            private bool isSelected2;
            public bool IsSelected2
            {
                get
                {
                    return isSelected2;
                }
                set
                {
                    if (Set(nameof(IsSelected2), ref isSelected2, value))
                    {
                        if (value)
                        {
                            IsSelected1 = false;
                        }
                    }
                }
            }

            public PropertyChildViewModel(IServiceManager services, string name, Responder responder1, Responder responder2)
                : base(services)
            {
                Name = name;
                Value1 = Convert.ToString(responder1.GetProperty(name));
                Value2 = Convert.ToString(responder2.GetProperty(name));
            }
        }

        public Responder Responder1 { get; private set; }
        public Responder Responder2 { get; private set; }
        public IList<PropertyChildViewModel> Properties { get; private set; }

        public RelayCommand SelectAll1Command { get; private set; }
        public RelayCommand SelectAll2Command { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public MergeSelectedViewModel(IServiceManager services, Responder responder1, Responder responder2)
            : base(services)
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
                Properties.Add(new PropertyChildViewModel(services, propertyName, responder1, responder2));
            }
            SelectAll1Command = new RelayCommand(SelectAll1);
            SelectAll2Command = new RelayCommand(SelectAll2);
            SaveCommand = new RelayCommand(Save);
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
                property.IsSelected1 = true;
            }
        }

        public void SelectAll2()
        {
            foreach (PropertyChildViewModel property in Properties)
            {
                property.IsSelected2 = true;
            }
        }

        public bool Validate()
        {
            ICollection<string> fields = new List<string>();
            foreach (PropertyChildViewModel property in Properties)
            {
                if (!property.IsSelected1 && !property.IsSelected2)
                {
                    fields.Add(property.Name);
                }
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Responder responder;
            responder = Properties[0].IsSelected1 ? Responder1 : Responder2;
            foreach (PropertyChildViewModel property in Properties)
            {
                Responder selectedResponder = property.IsSelected1 ? Responder1 : Responder2;
                responder.SetProperty(property.Name, selectedResponder.GetProperty(property.Name));
            }
            Context.Responders.Save(responder);
            responder = Properties[0].IsSelected1 ? Responder2 : Responder1;
            responder.Deleted = true;
            Context.Responders.Save(responder);
            OnSaved();
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Responders have been merged."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Responder)));
            Close();
        }
    }
}
