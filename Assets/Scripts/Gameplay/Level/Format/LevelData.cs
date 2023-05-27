using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RecoDeli.Scripts.Gameplay.Level.Format
{
    [Serializable]
    public class LevelData
    {
        public LevelInfo Info;
        public List<LevelObjectData> Objects;

        public LevelData() 
        {
            Objects = new List<LevelObjectData>();
        }

        public string ToXML()
        {
            using var textWriter = new StringWriter();
            XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            });
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            new XmlSerializer(typeof(LevelData)).Serialize(xmlWriter, this, namespaces);
            return textWriter.ToString();
        }

        public static LevelData FromXML(string xmlString)
        {
            using var textReader = new StringReader(xmlString);
            return (LevelData)(new XmlSerializer(typeof(LevelData)).Deserialize(textReader));
        }
    }
}
