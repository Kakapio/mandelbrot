using System;
using SkiaSharp;

namespace Mandelbrot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int length = 500;
            int[] grid = Run(length, 0, 0, 1);
            // Console.WriteLine("Please specify a square size for the image.");
            // try
            // {
            //     Console.Write("Length: ");
            //     height = Int32.Parse(Console.ReadLine());
            // }
            // catch (FormatException e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            
            using (var surface = SKSurface.Create(width: length, height: length, SKColorType.Rgba8888, SKAlphaType.Premul)) 
            {
                SKCanvas canvas = surface.Canvas;
                canvas.DrawColor(SKColors.Coral);

                for (int i = 0; i < length; i += 1)
                {
                    for (int j = 0; j < length; j += 1)
                    {
                        if (grid[i * length + j] > 8)
                        {
                            canvas.DrawPoint(new SKPoint(i, j), SKColors.Aquamarine);
                        }
                    }
                }
                canvas.DrawPoint(new SKPoint(250, 250), SKColors.Chartreuse);
                
                OutputImage(surface);
            }
            
            Console.WriteLine("Program successfully completed.");
        }
        
        public static int[] Run(int length, int fromX, int fromY, int h)
        {
            int[] output = new int[length * length];
            
            for (int i = 0; i < length; i += 1)
            {
                for (int j = 0; j < length; j += 1)
                {
                    float x = fromX + i * h;
                    float y = fromY + j * h;
                    output[i * length + j] = IterCount(x, y);
                }
            }

            return output;
        }
        
        public static int IterCount(float cx, float cy, int maxIterations = 400)
        {
            int result = 0;
            float x = 0.0f;
            float y = 0.0f;
            float xx = 0.0f, yy = 0.0f;
            while (xx + yy <= 4.0f && result < maxIterations)
            {
                xx = x * x;
                yy = y * y;
                float xtmp = xx - yy + cx;
                y = 2.0f * x * y + cy;
                x = xtmp;
                result++;
            }
            Console.WriteLine(result);
            return result;
        }

        private static void OutputImage(SKSurface surface)
        {
            Console.WriteLine("Attempting to write .png to disk...");
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            using (var stream = File.OpenWrite("out.png"))
            {
                // save the data to a stream
                data.SaveTo(stream);
                Console.WriteLine("Success!");
            }
        }
    }
}