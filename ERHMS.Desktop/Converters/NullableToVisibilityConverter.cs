namespace ERHMS.Desktop.Converters
{
    public class NullableToVisibilityConverter : BooleanToVisibilityConverter
    {
        public NullableToVisibilityConverter()
        {
            BaseConverter = new NullableToBooleanConverter();
        }
    }
}
