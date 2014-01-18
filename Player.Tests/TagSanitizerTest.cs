using System;
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
    }
}
