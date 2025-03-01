using BD_Assignment_2025.IRepositories;
using Moq;
using NUnit.Framework;

namespace BD_Assignment_2025.Testing
{
    [TestFixture]
    public class BlockedCountryTest
    {
        private Mock<IBlockedCountryRepository> _mockRepo;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IBlockedCountryRepository>();
        }

        [Test]
        public async Task TestBlockCountry()
        {
            // Arrange
            var arrange = await _mockRepo.Object.IsAnyExist(bc => bc.Key.ToUpper().Equals("EG"));
            // Act

            var compare = false;
            // Assert
            Assert.AreEqual(arrange, compare);
        }
    }
}
