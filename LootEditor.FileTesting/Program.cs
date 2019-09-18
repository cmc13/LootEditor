using LootEditor.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.FileTesting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var inFile = @"C:\Users\spect\Downloads\LootSnobV4.utl";
            var outFile = @"C:\Users\spect\Downloads\LootSnobV44.utl";

            var sw = Stopwatch.StartNew();
            var lf = new LootFile();
            using (var fs = new FileStream(inFile, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8, true, 65535))
            {
                await lf.ReadFileAsync(reader).ConfigureAwait(false);
            }

            Console.WriteLine($"Read file in {sw.Elapsed.TotalMilliseconds}ms");

            sw.Restart();
            using (var fs = new FileStream(outFile, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8, 65535))
            {
                await lf.WriteFileAsync(writer).ConfigureAwait(false);
            }
            Console.WriteLine($"Wrote file in {sw.Elapsed.TotalMilliseconds}ms");
            Console.ReadKey();
        }
    }
}
