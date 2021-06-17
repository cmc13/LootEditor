using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    public static class Extensions
    {
        public static Task<string> ReadLineForRealAsync(this TextReader reader)
            => Task.FromResult(reader.ReadLine());

        public static async Task WriteLineForRealAsync(this Stream stream, string text)
        {
            var bytes = Encoding.Default.GetBytes(text);
            await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            bytes = Encoding.Default.GetBytes(Environment.NewLine);
            await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }
    }
}
