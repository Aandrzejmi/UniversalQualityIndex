using System;
using System.Linq;
using System.Drawing;

internal class Program
{
    public static int DEFALUT_WINDOW = 8;

    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("To few arguments!");
            return 1;
        }

        string origPath = args[0]; 
        string testPath = args[1];

        Console.WriteLine($"Original Image: {origPath}");
        Console.WriteLine($"Tested Image: {testPath}");

        int window = DEFALUT_WINDOW;

        if (args.Length >= 3)
        {
            if (int.TryParse(args[2], out window))
            {
                Console.WriteLine($"Window Size: {window}");
            }
            else
            {
                Console.WriteLine($"Invalid window Size!: {window}");
                return 3;
            }
        }
        else
        {
            Console.WriteLine($"Default window size: {DEFALUT_WINDOW}");
        }

        try
        {
            Bitmap orig = new(origPath);
            Bitmap test = new(testPath);

            double quality = 0;

            Console.WriteLine($"Universal Image Quality Index: {quality}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file!\n{e.Message}");
            return 2;
        }

        return 0;
    }

    public static double Quality(double[] orig, double[] test)
    {
        double origMean = orig.Average();
        double testMean = test.Average();

        double denominator = Variance(orig, origMean) + Variance(test, testMean);
        denominator *= origMean * origMean + testMean * testMean;

        double quality = 4.0 * origMean * testMean * Deviation(orig, test, origMean, testMean);
        quality /= denominator;

        return quality;
    }

    public static double Deviation(double[] orig, double[] test, double origMean, double testMean)
    {
        double sum = 0;
        int lenght = orig.Length;

        for (int i = 0; i < lenght; i++)
        {
            sum += (orig[i] - origMean) * (test[i] - testMean);
        }

        return sum / (lenght - 1);
    }

    public static double Variance(double[] arr, double mean)
    {
        return Deviation(arr, arr, mean, mean);
    }
}