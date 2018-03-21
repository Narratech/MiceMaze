using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;



[XmlRoot("Maze")]
public class Maze
{

    [XmlArray("Tiles")]
    [XmlArrayItem("Tile")]
    public List<TileXml> Tiles = new List<TileXml>();

    public static Maze Load()
    {
        TextAsset maze = Resources.Load("maze") as TextAsset;
        var serializer = new XmlSerializer(typeof(Maze));
        using (var stream = new System.IO.StringReader(maze.text))
        {
            return serializer.Deserialize(stream) as Maze;
        }

    }




}
