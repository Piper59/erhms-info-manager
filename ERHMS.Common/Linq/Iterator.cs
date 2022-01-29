namespace ERHMS.Common.Linq
{
    public struct Iterator<TValue>
    {
        public int Index { get; }
        public TValue Value { get; }

        public Iterator(int index, TValue value)
        {
            Index = index;
            Value = value;
        }

        public void Deconstruct(out int index, out TValue value)
        {
            index = Index;
            value = Value;
        }

        public override string ToString()
        {
            return $"[{Index}: {Value}]";
        }
    }

    public static class Iterator
    {
        public static Iterator<TValue> Create<TValue>(int index, TValue value)
        {
            return new Iterator<TValue>(index, value);
        }
    }
}
