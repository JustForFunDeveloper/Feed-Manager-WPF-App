using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace HS_Feed_Manager.Core.Handler
{
    public class XmlHandler
    {
        public static string GetSerializedConfigXml(Type type, object config)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                XmlSerializer xmlSerializer = new XmlSerializer(type);
                using (var stringWriter = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                    {
                        if (config == null)
                            return null;
                        else
                            xmlSerializer.Serialize(writer, config);
                        return stringWriter.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetSerializedConfigXml: " + ex.ToString(), LogLevel.Error);
                return null;
            }
        }

        public static object GetDeserializedConfigObject(Type type, string xmlDocumentText)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (StringReader reader = new StringReader(xmlDocumentText))
                {
                    return serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetDeserializedConfigObject: " + ex.ToString(), LogLevel.Error);
                return null;
            }
        }
    }
}
