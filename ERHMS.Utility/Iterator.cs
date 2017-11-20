namespace ERHMS.Utility
{
    public class Iterator<T>
    {
        public T Value { get; private set; }
        public int Index { get; private set; }

        public Iterator(T value, int index)
        {
            Value = value;
            Index = index;
        }
    }
}
