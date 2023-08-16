namespace PerformanceApp.Utilites
{
    public static class QuickSort
    {
        public static void Sort(int[] array, int left, int right)
        {
            if (left >= right) return;
            var pivotIndex = Partition(array, left, right);
            Sort(array, left, pivotIndex - 1);
            Sort(array, pivotIndex + 1, right);
        }

        static int Partition(int[] array, int left, int right)
        {
            var pivot = array[right];
            var i = left - 1;

            for (var j = left; j < right; j++)
            {
                if (array[j] >= pivot) continue;
                i++;
                Swap(array, i, j);
            }

            Swap(array, i + 1, right);
            return i + 1;
        }

        static void Swap(int[] array, int i, int j)
        {
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
