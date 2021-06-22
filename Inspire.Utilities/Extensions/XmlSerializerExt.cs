namespace Inspire.Utilities.Extensions
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlSerializerExt
    {
        public static byte[] Serialize<T>(this T value)
        {
            if (value == null)
            {
                return null;
            }

            var serializer = new DataContractSerializer(typeof(T));
            var ms = new MemoryStream();
            using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                serializer.WriteObject(writer, value);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public static string Serialize(object input)
        {
            if (input == null)
            {
                return null;
            }

            var serializer = new XmlSerializer(input.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, input);
            }

            return sb.ToString();
        }
    }
}