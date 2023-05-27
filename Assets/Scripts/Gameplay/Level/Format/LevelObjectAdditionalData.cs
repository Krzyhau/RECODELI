using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace RecoDeli.Scripts.Gameplay.Level.Format
{
    [Serializable]
    public class LevelObjectAdditionalData : Dictionary<string, object>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) return;

            reader.ReadStartElement();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                string elementName = reader.LocalName;
                string name = reader.GetAttribute("Name");

                switch (elementName)
                {
                    case "int":
                        this.Add(name, Convert.ToInt32(reader.GetAttribute("Value")));
                        break;
                    case "float":
                        this.Add(name, Convert.ToSingle(reader.GetAttribute("Value")));
                        break;
                    case "Vector2":
                        this.Add(name, new Vector2(
                            Convert.ToSingle(reader.GetAttribute("X")),
                            Convert.ToSingle(reader.GetAttribute("Y"))
                        ));
                        break;
                    case "Vector3":
                        this.Add(name, new Vector3(
                            Convert.ToSingle(reader.GetAttribute("X")),
                            Convert.ToSingle(reader.GetAttribute("Y")),
                            Convert.ToSingle(reader.GetAttribute("Z"))
                        ));
                        break;
                    case "Quaternion":
                        this.Add(name, new Quaternion(
                            Convert.ToSingle(reader.GetAttribute("X")),
                            Convert.ToSingle(reader.GetAttribute("Y")),
                            Convert.ToSingle(reader.GetAttribute("Z")),
                            Convert.ToSingle(reader.GetAttribute("W"))
                        ));
                        break;
                    case "string":
                        this.Add(name, reader.GetAttribute("Value"));
                        break;
                    default:
                        break;
                }

                reader.Read();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach ((var name, var value) in this)
            {
                writer.WriteStartElement(value.GetType().Name);
                writer.WriteAttributeString("Name", name);

                if (value is int intValue)
                {
                    writer.WriteAttributeString("Value", intValue.ToString());
                }
                else if(value is float floatValue)
                {
                    writer.WriteAttributeString("Value", floatValue.ToString());
                }
                else if (value is Vector2 vector2Value)
                {
                    writer.WriteAttributeString("X", vector2Value.x.ToString());
                    writer.WriteAttributeString("Y", vector2Value.y.ToString());
                }
                else if (value is Vector3 vector3Value)
                {
                    writer.WriteAttributeString("X", vector3Value.x.ToString());
                    writer.WriteAttributeString("Y", vector3Value.y.ToString());
                    writer.WriteAttributeString("Z", vector3Value.z.ToString());
                }
                else if (value is Quaternion quaternionValue)
                {
                    writer.WriteAttributeString("X", quaternionValue.x.ToString());
                    writer.WriteAttributeString("Y", quaternionValue.y.ToString());
                    writer.WriteAttributeString("Z", quaternionValue.z.ToString());
                    writer.WriteAttributeString("W", quaternionValue.w.ToString());
                }
                else if(value is string stringValue)
                {
                    writer.WriteAttributeString("Value", stringValue);
                }

                writer.WriteEndElement();
            }
        }

        public static void Apply(GameObject gameObject, LevelObjectAdditionalData data)
        {
            var components = gameObject.GetComponents<ILevelObjectComponent>();
            foreach (var component in components)
            {
                var properties = component.GetType().GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(LevelObjectPropertyAttribute)))
                    .ToDictionary(p => p, p => p.GetCustomAttribute<LevelObjectPropertyAttribute>());

                foreach ((var property, var attribute) in properties)
                {
                    if (!data.ContainsKey(attribute.Name)) continue;
                    var value = data[attribute.Name];
                    if (property.PropertyType != value.GetType())
                    {
                        Debug.LogError($"Type mismatch for custom level object parameter {attribute.Name}:" +
                            $" {property.PropertyType.Name} != {value.GetType().Name}");
                    }
                    property.SetValue(component, data[attribute.Name]);
                }
            }
        }

        public static LevelObjectAdditionalData Extract(GameObject gameObject)
        {
            var data = new LevelObjectAdditionalData();

            var components = gameObject.GetComponents<ILevelObjectComponent>();
            foreach (var component in components)
            {
                var properties = component.GetType().GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(LevelObjectPropertyAttribute)))
                    .ToDictionary(p => p, p => p.GetCustomAttribute<LevelObjectPropertyAttribute>());

                foreach ((var property, var attribute) in properties)
                {
                    data[attribute.Name] = property.GetValue(component);
                }
            }

            return data;
        }
    }
}
