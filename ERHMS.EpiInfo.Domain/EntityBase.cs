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
                    OnPropertyChanged("New");
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
        protected void OnPropertyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        public bool HasProperty(string name)
        {
            return properties.ContainsKey(name);
        }

        public IEnumerable<string> GetPropertyNames()
        {
            return properties.Keys;
        }

        public object GetProperty(string name)
        {
            return properties[name];
        }

        public T GetProperty<T>(string name)
        {
            return (T)GetProperty(name);
        }

        public bool TryGetProperty(string name, out object value)
        {
            return properties.TryGetValue(name, out value);
        }

        public bool TryGetProperty<T>(string name, out T value)
        {
            object valueObject;
            if (TryGetProperty(name, out valueObject))
            {
                value = (T)valueObject;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public bool PropertyEquals(string name, object value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            object currentValue;
            if (TryGetProperty(name, out currentValue))
            {
                string valueString = value as string;
                string currentValueString = currentValue as string;
                if (valueString != null && currentValueString != null)
                {
                    return string.Equals(valueString, currentValueString, comparisonType);
                }
                else
                {
                    return Equals(value, currentValue);
                }
            }
            else
            {
                return false;
            }
        }

        public bool SetProperty(string name, object value)
        {
            if (PropertyEquals(name, value))
            {
                return false;
            }
            else
            {
                properties[name] = value;
                OnPropertyChanged(name);
                return true;
            }
        }

        protected void LinkProperties(string sourceName, string targetName)
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
