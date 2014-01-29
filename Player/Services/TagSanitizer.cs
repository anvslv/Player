using System;
using System.Linq;
using System.Text;
using System.Xml;

namespace Player.Core
{
    /// <summary>
    /// Provides a method to sanitize the tag of a song (e.g removing invalid characters).
    /// This applies not to the song on the physical drive directly, but only to the presentation in the application.
    /// </summary>
    internal static class TagSanitizer
    {
        public static string Sanitize(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException("tag");

            var buffer = new StringBuilder(tag.Length);

            foreach (char c in tag)
            {
                buffer.Append(XmlConvert.IsXmlChar(c) ? c : '_');
            }

            return toUtf8(buffer.ToString());
        }

        // http://www.cyberforum.ru/csharp-net/thread356693.html
        public static string toUtf8(string unknown)
        {
            return new string(unknown.ToCharArray().
                Select(x => ((x + 848) >= 'А' && (x + 848) <= 'ё') ? (char)(x + 848) : x).
                ToArray());
        } 
    }
}