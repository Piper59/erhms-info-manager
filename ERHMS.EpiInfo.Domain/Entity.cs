using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Domain
{
    public class Entity : DynamicObject, INotifyPropertyChanged, ICloneable
    {
        private static readonly Regex PrefixPattern = new Regex(@"^.+\.");

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

        protected Entity()
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

        public bool TryGetProperty(string name, out object value)
        {
            return properties.TryGetValue(name, out value);
        }

        public bool TryGetProperty<T>(string name, out T value)
        {
            object obj;
            if (TryGetProperty(name, out obj))
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

        public object GetProperty(string name)
        {
            object value;
            TryGetProperty(name, out value);
            return value;
        }

        public T GetProperty<T>(string name)
        {
            return (T)GetProperty(name);
        }

        public bool PropertyEquals(string name, object value)
        {
            object obj;
            if (TryGetProperty(name, out obj))
            {
                return Equals(value, obj);
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

        public void SetProperties(IDataRecord record)
        {
            for (int index = 0; index < record.FieldCount; index++)
            {
                string name = PrefixPattern.Replace(record.GetName(index), "");
                object value = record.IsDBNull(index) ? null : record.GetValue(index);
                SetProperty(name, value);
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
            Entity clone = (Entity)Activator.CreateInstance(GetType());
            clone.New = New;
            foreach (KeyValuePair<string, object> property in properties)
            {
                object value = property.Value;
                ICloneable cloneable = value as ICloneable;
                if (cloneable == null)
                {
                    clone.SetProperty(property.Key, value);
                }
                else
                {
                    clone.SetProperty(property.Key, cloneable.Clone());
                }
            }
            return clone;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GetHashCode(properties.Values);
        }

        public override bool Equals(object obj)
        {
            Entity entity = obj as Entity;
            if (entity == null)
            {
                return false;
            }
            foreach (KeyValuePair<string, object> property in properties)
            {
                object value;
                if (!entity.properties.TryGetValue(property.Key, out value))
                {
                    return false;
                }
                if (!Equals(value, property.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
