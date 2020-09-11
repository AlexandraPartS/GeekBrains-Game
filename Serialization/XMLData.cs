using System;
using System.IO;
using UnityEngine;
using System.Xml;

public class XMLData : ISaveData
{
    static string SavePath = Path.Combine(Application.dataPath, "XMLdata.xml");

    public void Save(PlayerData player)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("Player");
        xmlDoc.AppendChild(rootNode);

        XmlElement element = xmlDoc.CreateElement("Name");
        element.SetAttribute("value", player.PLName);
        rootNode.AppendChild(element);

        element = xmlDoc.CreateElement("Health");
        element.SetAttribute("value", player.PLHealth.ToString());
        rootNode.AppendChild(element);

        element = xmlDoc.CreateElement("Dead");
        element.SetAttribute("value", player.PLDead.ToString());
        rootNode.AppendChild(element);

        xmlDoc.Save(SavePath);
    }



    public PlayerData Load()
    {
        var result = new PlayerData();

        if(!File.Exists(SavePath))
        {
            Debug.Log("File NOT exest!");
            return result;
        }


        using (XmlTextReader reader = new XmlTextReader(SavePath))
        {
            while(reader.Read())
            {
                if(reader.IsStartElement("Name"))
                {
                    result.PLName = reader.GetAttribute("value");
                }

                if(reader.IsStartElement("Health"))
                {
                    Int32.TryParse(reader.GetAttribute("value"), out result.PLHealth);
                }

                if (reader.IsStartElement("Dead"))
                {
                    result.PLDead = Convert.ToBoolean(reader.GetAttribute("value"));
                }
            }
        }






            return result;
    }



}
