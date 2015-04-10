using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GauntletLeaderboard.Core.Extensions
{
    public static class XElementExtensions
    {
        public static IEnumerable<T> Deserialize<T>(this IEnumerable<XElement> elements)
        {
            var serializer = new XmlSerializer(typeof(T));
            var items = new List<T>();
            return elements.Select(e =>
            {
                using (var reader = e.CreateReader())
                    return (T)serializer.Deserialize(reader);
            });
        }
    }
}