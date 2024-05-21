namespace LootEditor;

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class VTClassicColorInfo
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("slotdef")]
    public VTClassicColorInfoSlotdef[] slotdef { get; set; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VTClassicColorInfoSlotdef
{
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("entry")]
    public VTClassicColorInfoSlotdefEntry[] entry { get; set; }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name { get; set; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class VTClassicColorInfoSlotdefEntry
{
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte value { get; set; }
}
