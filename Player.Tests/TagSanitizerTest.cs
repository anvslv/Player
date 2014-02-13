using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Player.Core;
using Xunit;

namespace Player.Tests
{
    public class TagSanitizerTest
    {
        [Fact]
        public void Sanitize_TagContainsInvalidXmlCharacter_ReturnsSanitizedString()
        {
            const string tag = "A\u0018B";

            string sanitized = TagSanitizer.Sanitize(tag);

            Assert.Equal("A_B", sanitized);
        }

        [Fact]
        public void Sanitize_TagIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TagSanitizer.Sanitize(null));
        }

        [Fact]
        public void Sanitize_TagIsEnglish_ReturnsCorrectResult()
        {
            const string tag = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string sanitized = TagSanitizer.Sanitize(tag);

            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", sanitized);
        }
         
        [Fact]
        public void Sanitize_TagIsCyrillicNonUnicode_ReturnsCorrectResult()
        {
            const string tag = "Ð¸";

            string sanitized = TagSanitizer.Sanitize(tag);

            Assert.Equal("Ёё", sanitized);
        }

        [Fact]
        public void Sanitize_TagIsCyrillicUnicode_ReturnsCorrectResult()
        {
            const string tag = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЙЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

            string sanitized = TagSanitizer.Sanitize(tag);

            Assert.Equal("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЙЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя", sanitized);
        }

        [Fact]
        public void PrintChars()
        {
            for (int i = 0; i < 1300; i++)
            {
                Debug.WriteLine("{0}: {1}", i, (char) i);
            }
        }
    }
}
