using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class CalculatedBuffedTinkedTargetMeleeGELootCriteria : LootCriteria
    {
        public double CalculatedBuffedTinkedDamageOverTime { get; set; }
        public double BuffedMeleeDefenseBonus { get; set; }
        public double BuffedAttackBonus { get; set; }

        public override LootCriteriaType Type => LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE;

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            CalculatedBuffedTinkedDamageOverTime = await ReadValue<double>(reader).ConfigureAwait(false);
            BuffedMeleeDefenseBonus = await ReadValue<double>(reader).ConfigureAwait(false);
            BuffedAttackBonus = await ReadValue<double>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(CalculatedBuffedTinkedDamageOverTime.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(BuffedMeleeDefenseBonus.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(BuffedAttackBonus.ToString()).ConfigureAwait(false);
        }
    }
}