using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class CalculatedBuffedTinkedTargetMeleeGELootCriteria : LootCriteria
    {
        public double CalculatedBuffedTinkedDamageOverTime { get; set; }
        public double BuffedMeleeDefenseBonus { get; set; }
        public double BuffedAttackBonus { get; set; }

        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE;

        public CalculatedBuffedTinkedTargetMeleeGELootCriteria() { }

        private CalculatedBuffedTinkedTargetMeleeGELootCriteria(SerializationInfo info, StreamingContext context)
        {
            CalculatedBuffedTinkedDamageOverTime = info.GetDouble(nameof(CalculatedBuffedTinkedDamageOverTime));
            BuffedMeleeDefenseBonus = info.GetDouble(nameof(BuffedMeleeDefenseBonus));
            BuffedAttackBonus = info.GetDouble(nameof(BuffedAttackBonus));
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            CalculatedBuffedTinkedDamageOverTime = await ReadValue<double>(reader).ConfigureAwait(false);
            BuffedMeleeDefenseBonus = await ReadValue<double>(reader).ConfigureAwait(false);
            BuffedAttackBonus = await ReadValue<double>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(CalculatedBuffedTinkedDamageOverTime.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(BuffedMeleeDefenseBonus.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(BuffedAttackBonus.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(CalculatedBuffedTinkedDamageOverTime), CalculatedBuffedTinkedDamageOverTime, typeof(double));
            info.AddValue(nameof(BuffedMeleeDefenseBonus), BuffedMeleeDefenseBonus, typeof(double));
            info.AddValue(nameof(BuffedAttackBonus), BuffedAttackBonus, typeof(double));
        }
    }
}