using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public static class Extensions
    {
        public static Task<string> ReadLineForRealAsync(this TextReader reader)
            => Task.FromResult(reader.ReadLine());

        public static Task WriteLineForRealAsync(this TextWriter writer, string text)
            => Task.Run(() => writer.WriteLine(text));
    }
}
