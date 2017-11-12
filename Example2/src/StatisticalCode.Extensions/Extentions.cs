using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace StatisticalCode.Extensions
{
    internal static class Extentions
    {
        public static T Deserialize<T>(string embededResourceName) where T:class 
        {
            T reult = null;
            var serializer = new XmlSerializer(typeof(T));

            var thisAssembly = typeof(Extentions).GetTypeInfo().Assembly;

            using (var stream = thisAssembly.GetManifestResourceStream(embededResourceName))
            {
                using (var reader = XmlReader.Create(stream))
                {
                    reult = (T)serializer.Deserialize(reader);
                }
            }

            return reult;
        }
    }
}
