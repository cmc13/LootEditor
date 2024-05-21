using System;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace LootEditor;

public class ArmorGroupBindingSourceExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var ser = new XmlSerializer(typeof(VTClassicColorInfo));
        using var fs = Assembly.GetExecutingAssembly().GetManifestResourceStream("LootEditor.Assets.ColorSlots.Default.xml");
        var info = ser.Deserialize(fs) as VTClassicColorInfo;
        return info.slotdef.Select(s => Tuple.Create(s.name, s.name)).Concat([Tuple.Create("(None)", (string)null)]).OrderBy(s => s.Item1);
    }
}
