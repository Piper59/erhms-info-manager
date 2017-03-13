using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace ERHMS.EpiInfo.Domain
{
    public class EntityBase : DynamicObject, INotifyPropertyChanged, ICloneable
    {
        private IDictionary<string, object> properties;

        private bool @new;
        public bool New
        {
            get
            {
                return @new;
            }
            set
            {
                if (value != @new)
                {
                    @new = value;
                    OnPropertyChanged(nameof(New));
                }
            }
        }

        protected EntityBase()
        {
            properties = new Dictionary<string, object>();
            New = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public bool TryGetProperty(string propertyName, out object value)
        {
            return properties.TryGetValue(propertyName, out value);
        }

        public bool TryGetProperty<T>(string propertyName, out T value)
        {
            object obj;
            if (TryGetProperty(propertyName, out obj))
            {
                value = (T)obj;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public object GetProperty(string propertyName)
        {
            object value;
            TryGetProperty(propertyName, out value);
            return value;
        }

        public T GetProperty<T>(string propertyName)
        {
            return (T)GetProperty(propertyName);
        }

        public bool PropertyEquals(string propertyName, object value)
        {
            object currentValue;
            if (TryGetProperty(propertyName, out currentValue))
            {
                return Equals(value, currentValue);
            }
            else
            {
                return false;
            }
        }

        public bool SetProperty(string propertyName, object value)
        {
            if (PropertyEquals(propertyName, value))
            {
                return false;
            }
            else
            {
                properties[propertyName] = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        protected void AddSynonym(string sourceName, string targetName)
        {
            if (sourceName != targetName)
            {
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == sourceName)
                    {
                        OnPropertyChanged(targetName);
                    }
                };
            }
        }

        public sealed override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetProperty(binder.Name, out result);
        }

        public sealed override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }

        public object Clone()
        {
            EntityBase clone = (EntityBase)Activator.CreateInstance(GetType());
            clone.New = New;
            foreach (KeyValuePair<string, object> property in properties)
            {
                object value = property.Value;
                ICloneable cloneableValue = value as ICloneable;
                if (cloneableValue == null)
                {
                    clone.SetProperty(property.Key, value);
                }
                else
                {
                    clone.SetProperty(property.Key, cloneableValue.Clone());
                }
            }
            return clone;
        }
    }
}
