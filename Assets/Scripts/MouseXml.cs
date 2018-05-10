using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;


public class MouseXml
{
    [XmlAttribute("IA")]
    public bool IA;

    public string IAType;

    public int Red;

    public int Green;

    public int Blue;

}