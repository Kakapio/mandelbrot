using System;
using SkiaSharp;

namespace Mandelbrot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int height = 500, width = 500;
            // Console.WriteLine("Please specify a height and width for the image.");
            // try
            // {
            //     Console.Write("Height: ");
            //     height = Int32.Parse(Console.ReadLine());
            //     Console.Write("Width: ");
            //     width = Int32.Parse(Console.ReadLine());
            // }
            // catch (FormatException e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }
            
            using (var surface = SKSurface.Create(width: width, height: height, SKColorType.Rgba8888, SKAlphaType.Premul)) 
            {
                SKCanvas canvas = surface.Canvas;
                canvas.DrawColor(SKColors.Black);

                canvas.DrawPoint(new SKPoint(250, 250), SKColors.Chartreuse);
                
                OutputImage(surface);
            }
            
            Console.WriteLine("Program successfully completed.");
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