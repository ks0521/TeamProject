using System;

namespace Base.Utils
{
    public static class ArrSorter<T>
    {
        static void Swap(T[] arr, int a, int b)
        {
            T temp = arr[a];
            arr[a] = arr[b];
            arr[b] = temp;
        }
        public static void ArrSortStart(T[] arr, Comparison<T> comparison) => ArrSort(arr, 0, arr.Length - 1, comparison);
        public static void ArrSort(T[] arr, int left, int right, Comparison<T> comparison)
        {
            if (left >= right) return;

            int mid = left + (right - left) / 2;
            if (comparison(arr[left], arr[mid]) > 0) Swap(arr, left, mid);
            if (comparison(arr[left], arr[right]) > 0) Swap(arr, left, right);
            if (comparison(arr[mid], arr[right]) > 0) Swap(arr, mid, right);

            if (right - left <= 2) return;

            Swap(arr, mid, right - 1);
            T pivot = arr[right - 1];

            int i = left + 1;
            for (int j = left + 1; j < right - 1; j++)
            {
                if (comparison(arr[j], pivot) < 0)
                {
                    Swap(arr, i, j);
                    i++;
                }
            }

            Swap(arr, i, right - 1);

            ArrSort(arr, left, i - 1, comparison);
            ArrSort(arr, i + 1, right, comparison);
        }
    }
}