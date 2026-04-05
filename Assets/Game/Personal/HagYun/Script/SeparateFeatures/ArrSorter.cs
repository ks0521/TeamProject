using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public static class ArrSorter<T>
    {
        static void Swap(T[] arr, int a, int b)
        {
            T temp = arr[a];
            arr[b] = arr[a];
            arr[a] = temp;
        }
        public static void StartArrSort(T[] arr, int length, Comparison<T> comparison) => ArrSort(arr, 0, length - 1, comparison);

        public static void ArrSort(T[] arr, int left, int right, Comparison<T> comparison)
        {
            int rightSubLeft = right - left;
            if (rightSubLeft <= 1) return;
            else if (rightSubLeft == 2)
            {
                if (0 < comparison(arr[left], arr[right])) Swap(arr, left, right);
                return;
            }

            int mid = left + rightSubLeft / 2;
            if (0 < comparison(arr[left], arr[mid])) Swap(arr, left, mid);
            if (0 < comparison(arr[left], arr[right])) Swap(arr, left, right);
            if (0 < comparison(arr[mid], arr[right])) Swap(arr, mid, right);

            T pivot = arr[mid];
            Swap(arr, mid, right);

            int i = left;
            for (int j = left; j < right; j++)
            {
                if (comparison(arr[j], pivot) < 0)
                {
                    Swap(arr, i, j);
                    i++;
                }
            }

            Swap(arr, i, right);

            ArrSort(arr, left, i - 1, comparison);
            ArrSort(arr, i + 1, right, comparison);
        }
    }
}