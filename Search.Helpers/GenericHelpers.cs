using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Search.Helpers
{
    public class GenericHelpers
    {

        

        //public IList<T> ReadFile<T>(string filePath)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

        //    using (FileStream stream = File.OpenRead(filePath))
        //    {
        //        List<T> deserializedList = (List<T>)serializer.Deserialize(stream);
        //        return deserializedList;
        //    }
        //}


        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <typeparam name="T">The type of the object that gets serialized.</typeparam>
        /// <param name="stream">The stream to which the bytes will be written.</param>
        /// <param name="serializableObject">The object that gets serialized.</param>
        public void SerializeObject<T>(Stream stream, T serializableObject) where T : IXmlSerializable
        {
            var writerSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ", // 4 spaces
                Encoding = Encoding.UTF8
            };

            using (var xmlWriter = XmlWriter.Create(stream, writerSettings))
            {
                serializableObject.WriteXml(xmlWriter);
            }

            stream.Close();
        }

        /// <summary>
        /// Deserialize a stream and return the object.
        /// </summary>
        /// <typeparam name="T">The type of the object that returns from the deserialization.</typeparam>
        /// <param name="stream">The stream which contains the bytes to deserialize.</param>
        /// <returns>The object recovered.</returns>
        public T DeserializeObject<T>(Stream stream, string ElementName) //where T : IXmlSerializable
        {

            using (var xmlTextReader = XmlReader.Create(stream))
            {
                var s = xmlTextReader.ToString();

                var serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute() { ElementName = ElementName });
                List<T> result = (List<T>)serializer.Deserialize(xmlTextReader);
//                T result = (T)serializer.Deserialize(xmlTextReader);
                stream.Close();
                return result[0];
            }
        }
    }
}
