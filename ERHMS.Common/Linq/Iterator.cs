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

        public override string ToString()
        {
            return $"[{Index}: {Value}]";
        }
    }
}
