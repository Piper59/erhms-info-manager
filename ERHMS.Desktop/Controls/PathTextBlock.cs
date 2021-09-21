using ERHMS.Desktop.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Controls
{
    public class PathTextBlock : TextBlock
    {
        private BindingBase binding;
        public BindingBase Binding
        {
            get
            {
                return binding;
            }
            set
            {
                binding = value;
                SetBinding(TextProperty, GetTextBinding());
            }
        }

        private object fallbackValue = DependencyProperty.UnsetValue;
        public object FallbackValue
        {
            get
            {
                return fallbackValue;
            }
            set
            {
                fallbackValue = value;
                SetBinding(TextProperty, GetTextBinding());
            }
        }

        private BindingBase GetTextBinding()
        {
            MultiBinding textBinding = new MultiBinding
            {
                Converter = new PathTrimmingConverter(this),
                FallbackValue = FallbackValue
            };
            textBinding.Bindings.Add(Binding);
            textBinding.Bindings.Add(new Binding(nameof(ActualWidth))
            {
                Source = this
            });
            return textBinding;
        }
    }
}
