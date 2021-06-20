using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    public class RuleTemplate
    {
        public RuleTemplate(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public string Name => Path.GetFileNameWithoutExtension(FileName);

        public async Task<LootRule> GetRule()
        {
            if (File.Exists(FileName))
            {
                using var fs = File.OpenRead(FileName);
                using var reader = new StreamReader(fs);
                var rule = await LootRule.ReadRuleAsync(1, reader);
                return rule;
            }

            return null;
        }
    }
}
