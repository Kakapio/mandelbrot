using System;
using System.Diagnostics;
using System.Numerics;
using SkiaSharp;

namespace Mandelbrot
{
    internal class Program
    {
        const int MaxIterations = 1000;
        
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Starting program...");
            const int size = 4096;
            
            int[,] unoptimized = ParallelForRun(size);
            
            DataToImage(size, unoptimized);
            
            stopwatch.Stop();
            Console.WriteLine($"Program successfully completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static void DataToImage(int size, int[,] unoptimized)
        {
            using (var surface = SKSurface.Create(width: size, height: size, SKColorType.Rgba8888, SKAlphaType.Premul))
            {
                SKCanvas canvas = surface.Canvas;
                canvas.DrawColor(SKColors.Coral);

                for (int i = 0; i < unoptimized.GetLength(0); i++)
                {
                    for (int j = 0; j < unoptimized.GetLength(1); j++)
                    {
                        byte rgb = (byte)unoptimized[i, j];
                        canvas.DrawPoint(new SKPoint(i, j),
                            unoptimized[i, j] >= MaxIterations ? SKColors.Black : new SKColor(rgb, 0, 100));
                    }
                }

                OutputImage(surface, "ParallelFor");
            }
        }

        public static int[,] UnoptimizedRun(int size)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int[,] output = new int[size, size];
            
            for (int i = 0; i < output.GetLength(0); i += 1)
            {
                for (int j = 0; j < output.GetLength(1); j += 1)
                {
                    double x = (i - size / 2d) / (size / 4d);
                    double y = (j - size / 2d) / (size / 4d);
                    output[i, j] = IterCount(x, y, MaxIterations);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Time taken to complete unoptimized run: {stopwatch.ElapsedMilliseconds}ms");
            return output;
        }
        
        public static int[,] ParallelForRun(int size)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int[,] output = new int[size, size];

            Parallel.For(0, output.GetLength(0), i =>
            {
                for (int j = 0; j < output.GetLength(1); j += 1)
                {
                    double x = (i - size / 2d) / (size / 4d);
                    double y = (j - size / 2d) / (size / 4d);
                    output[i, j] = IterCount(x, y, MaxIterations);
                }
            });

            stopwatch.Stop();
            Console.WriteLine($"Time taken to complete parallel for run: {stopwatch.ElapsedMilliseconds}ms");
            return output;
        }

        /// <summary>
        /// Calculate the number of iterations needed for a given coord.
        /// </summary>
        /// <param name="constX">The real part of the coord</param>
        /// <param name="constY">The imaginary part of the coord</param>
        /// <param name="maxIterations">Maximum number of iterations allowed</param>
        /// <returns></returns>
        public static int IterCount(double constX, double constY, int maxIterations)
        {
            const double maxMagnitude = 2d;
            const double maxMagnitudeSquared = maxMagnitude * maxMagnitude;
            int i = 0;
            double x = 0.0d, y = 0.0d;
            double xSquared = 0.0d, ySquared = 0.0d;
            
            while (xSquared + ySquared <= maxMagnitudeSquared && i < maxIterations)
            {
                xSquared = x * x;
                ySquared = y * y;
                double xtmp = xSquared - ySquared + constX;
                y = 2.0d * x * y + constY;
                x = xtmp;
                i++;
            }
            return i;
        }

        private static void OutputImage(SKSurface surface, string name)
        {
            Console.WriteLine("Attempting to write .png to disk...");
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            using (var stream = File.OpenWrite(name + ".png"))
            {
                data.SaveTo(stream);
                Console.WriteLine("Success!");
            }
        }
    }
}