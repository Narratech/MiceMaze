using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("Configuration")]
public class ConfigurationXml
{

    [XmlArray("Mouses")]
    [XmlArrayItem("Mouse")]
    public List<MouseXml> Mouses = new List<MouseXml>();
    

    public string maze;

    public bool scientificIA;

    public string scientificIAType;

    public int NumberTurnsMaze;

    public int NumberTurnsInterrogation;

    public bool TotalVision;

    public static ConfigurationXml Load()
    {
        TextAsset configuration = Resources.Load("configuration") as TextAsset;
        var serializer = new XmlSerializer(typeof(ConfigurationXml));
        using (var stream = new System.IO.StringReader(configuration.text))
        {
            return serializer.Deserialize(stream) as ConfigurationXml;
        }

    }



}
