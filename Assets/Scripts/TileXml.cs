using System.Xml;
using System.Xml.Serialization;


public class TileXml
{
    [XmlAttribute("contains")]
    public string Contains;

    public float Rotation;

    public int X;

    public int Z;

}
