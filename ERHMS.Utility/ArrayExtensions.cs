using System;

namespace ERHMS.Utility
{
    public static class ArrayExtensions
    {
        public static void Resize<T>(ref T[] array, int size, int start)
        {
            T[] result = new T[size];
            Array.Copy(array, 0, result, start, Math.Min(array.Length, size - start));
            array = result;
        }
    }
}
