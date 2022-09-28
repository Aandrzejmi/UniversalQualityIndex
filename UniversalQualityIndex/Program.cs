using System;
using System.Linq;
using System.Drawing;
using System.IO;
using static Program;

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
            Bitmap origBmp = new(origPath);
            Bitmap testBmp = new(testPath);

            int width = origBmp.Width;
            int height = origBmp.Height;

            if (testBmp.Width != width || testBmp.Height != height)
            {
                Console.WriteLine("Error: Images have diffrent resolutions!");
                return 4;
            }

            double[,] orig = new double[width,height];
            double[,] test = new double[width,height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    orig[i, j] = origBmp.GetPixel(i, j).GetBrightness();
                    test[i, j] = testBmp.GetPixel(i, j).GetBrightness();
                }
            }

            int stepsW = width - window;
            int stepsH = height - window;

            double quality = 0;

            for (int i = 0; i < stepsW; i++)
            {
                for (int j = 0; j < stepsH; j++)
                {
                    quality += Quality(orig, test, new Window { startX = i, startY = j, width = width });
                }

                Console.WriteLine($"Progress: {i} / {stepsW}");
            }

            quality /= stepsW * stepsH;

            Console.WriteLine($"Universal Image Quality Index: {quality}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file!:\n\t{e.Message}");
            return 2;
        }

        return 0;
    }

    public struct Window
    {
        public int startX;
        public int startY;
        public int width;
    }

    public static double Quality(double[,] orig, double[,] test, Window window)
    {
        double origMean = 0;
        double testMean = 0;

        for (int i = window.startX; i < window.width; i++)
        {
            for (int j = window.startY; j < window.width; j++)
            {
                origMean += orig[i, j];
                testMean += test[i, j];
            }
        }

        origMean /= window.width * window.width;
        testMean /= window.width * window.width;

        double denominator = Variance(orig, origMean, window) + Variance(test, testMean, window);
        denominator *= origMean * origMean + testMean * testMean;

        double quality = 4.0 * origMean * testMean * Deviation(orig, test, origMean, testMean, window);
        quality /= denominator;

        return quality;
    }

    public static double Deviation(double[,] orig, double[,] test, double origMean, double testMean, Window window)
    {
        double sum = 0;

        for (int i = window.startX; i < window.width; i++)
        {
            for (int j = window.startY; j < window.width; j++)
            {
                sum += (orig[i,j] - origMean) * (test[i,j] - testMean);
            }
        }

        return sum / (window.width * window.width - 1);
    }

    public static double Variance(double[,] arr, double mean, Window window)
    {
        return Deviation(arr, arr, mean, mean, window);
    }
}