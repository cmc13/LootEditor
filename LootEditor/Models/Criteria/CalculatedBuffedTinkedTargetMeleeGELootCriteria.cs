using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models;

[Serializable]
public class CalculatedBuffedTinkedTargetMeleeGELootCriteria : LootCriteria
{
    public double CalculatedBuffedTinkedDamageOverTime { get; set; }
    public double BuffedMeleeDefenseBonus { get; set; }
    public double BuffedAttackBonus { get; set; }
    public override string Filter => $"{base.Filter}:{CalculatedBuffedTinkedDamageOverTime}:{BuffedMeleeDefenseBonus}:{BuffedAttackBonus}";

    public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE;

    public CalculatedBuffedTinkedTargetMeleeGELootCriteria() { }

    public override string ToString() => $"Melee Target: {CalculatedBuffedTinkedDamageOverTime} Dmg/Time; {BuffedMeleeDefenseBonus}md; {BuffedAttackBonus}a";

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

    public override async Task WriteInternalAsync(Stream stream)
    {
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

    public override bool IsMatch(string[] filter)
    {
        if (!base.IsMatch(filter))
            return false;

        if (filter.Length >= 2 && !string.IsNullOrEmpty(filter[1]))
        {
            if (!double.TryParse(filter[1], out var test) || test != CalculatedBuffedTinkedDamageOverTime)
                return false;
        }

        if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
        {
            if (!double.TryParse(filter[2], out var test) || test != BuffedMeleeDefenseBonus)
                return false;
        }

        if (filter.Length >= 4 && !string.IsNullOrEmpty(filter[3]))
        {
            if (!double.TryParse(filter[3], out var test) || test != BuffedAttackBonus)
                return false;
        }

        return true;
    }
}