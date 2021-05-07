using System;
using System.Collections.Generic;

public static class CommonAlgorithms
{
    public static void QuickSort<T>(List<T> list, int startIndex, int endIndex, Func<T, T, bool> smallerFunc)
    {
        if (endIndex <= startIndex) return;
        var pi = Partition(list, startIndex, endIndex, smallerFunc);
        QuickSort(list, startIndex, pi - 1, smallerFunc);
        QuickSort(list, pi + 1, endIndex, smallerFunc);
    }
    
    private static int Partition<T>(List<T> arr, int startIndex, int endIndex, Func<T, T, bool> smallerFunc)
    {
        var i = startIndex - 1;
        var j = startIndex;
        var pivotIndex = endIndex;
        while (j < pivotIndex)
        {
            if (smallerFunc(arr[j], arr[pivotIndex]))
            {
                i += 1;
                Swap(arr, i, j);
            }
            j++;
        }

        Swap(arr, ++i, j);
        return i;
    }
    
    private static void Swap<T>(List<T> arr, int firstIndex, int secondIndex)
    {
        var temp = arr[firstIndex];
        arr[firstIndex] = arr[secondIndex];
        arr[secondIndex] = temp;
    }
    
    public static int BinarySearchForIndex<T>(List<T> list, int startIndex, int endIndex, T t, Func<T, T, bool> equals, Func<T, T, bool> smaller)
    {
        if (endIndex >= startIndex) 
        {
            int mid = startIndex + (endIndex - startIndex) / 2;

            if (equals(t, list[mid]))
                return mid + 1;
            
            if (smaller(t, list[mid]))
                return BinarySearchForIndex(list, startIndex, mid - 1, t, equals, smaller);
 
            return BinarySearchForIndex(list, mid + 1, endIndex, t, equals, smaller);
        }
        
        return startIndex;
    }
}
