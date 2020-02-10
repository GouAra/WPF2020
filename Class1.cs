using System;

public class Test
{
    static void WriteMas(int n, int[] a)
    {
        for (int i = 0; i < n; i++)
            a[i] = Convert.ToInt32(Console.ReadLine());
    }

    static int[] BubbleSort(int n, int[] a)
    {
        int t;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (a[j + 1] > a[j])
                {
                    t = a[j + 1];
                    a[j + 1] = a[j];
                    a[j] = t;
                }
            }
        }
        return a;
    }

    public static void Main()
    {
        int n;
        n = Convert.ToInt32(Console.ReadLine());
        int[] a = new int[n];
        WriteMas(n, a);
        BubbleSort(n, a);
        for (int i = 0; i < n; i++)
            Console.WriteLine(a[i]);
    }
}