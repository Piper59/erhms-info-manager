using System;
using System.ComponentModel;

namespace ERHMS.Presentation
{
    public class CustomPropertyDescriptor<TComponent, TProperty> : PropertyDescriptor
    {
        private Func<TComponent, TProperty> getter;
        private Action<TComponent, TProperty> setter;

        public override Type ComponentType
        {
            get { return typeof(TComponent); }
        }

        public override Type PropertyType
        {
            get { return typeof(TProperty); }
        }

        public override bool IsReadOnly
        {
            get { return setter == null; }
        }

        public CustomPropertyDescriptor(string name, Func<TComponent, TProperty> getter, Action<TComponent, TProperty> setter)
            : base(name, null)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public CustomPropertyDescriptor(string name, Func<TComponent, TProperty> getter)
            : this(name, getter, null) { }

        public override object GetValue(object component)
        {
            return getter((TComponent)component);
        }

        public override void SetValue(object component, object value)
        {
            setter((TComponent)component, (TProperty)value);
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override void ResetValue(object component)
        {
            SetValue(component, default(TProperty));
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}
