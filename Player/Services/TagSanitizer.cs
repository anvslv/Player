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
                buffer.Append(XmlConvert.IsXmlChar(c) ? c.ToUtf8() : '_');
            }

            return buffer.ToString();
        }
         
        public static char ToUtf8(this char x)
        {
            char result;
            switch (x + 848)
            { 
                case 1032:
                    result = 'ё'; 
                    break; 
                default:
                    if ((x + 848) >= 'А' && (x + 848) <= 'ё')
                        result = (char) (x + 848);
                    else 
                        result = x;
                    break;
            }

            return result;
        } 
    }
} 