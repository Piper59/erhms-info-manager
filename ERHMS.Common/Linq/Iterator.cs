namespace ERHMS.Common.Linq
{
    public struct Iterator<TValue>
    {
        public TValue Value { get; }
        public int Index { get; }

        public Iterator(TValue value, int index)
        {
            Value = value;
            Index = index;
        }

        public override string ToString()
        {
            return $"[{Index}: {Value}]";
        }
    }
}
