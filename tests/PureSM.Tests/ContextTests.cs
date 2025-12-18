using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureSM;
using System;

namespace PureSM.Tests
{
    [TestClass]
    public class ContextTests
    {
        private Context _context;

        [TestInitialize]
        public void Setup()
        {
            _context = new Context();
        }

        [TestMethod]
        public void SetItem_WithValidKeyAndValue_StoresItem()
        {
            // Arrange
            var key = "testKey";
            var value = "testValue";

            // Act
            _context.SetItem(key, value);

            // Assert
            Assert.AreEqual(value, _context.GetItem(key));
        }

        [TestMethod]
        public void SetItem_WithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            object value = "testValue";

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _context.SetItem(null, value));
        }

        [TestMethod]
        public void SetItem_WithNullValue_StoresNullValue()
        {
            // Arrange
            var key = "testKey";

            // Act
            _context.SetItem(key, null);

            // Assert
            Assert.IsNull(_context.GetItem(key));
        }

        [TestMethod]
        public void GetItem_WithValidKey_ReturnsStoredValue()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = "testValue";
            _context.SetItem(key, expectedValue);

            // Act
            var result = _context.GetItem(key);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void GetItem_WithNonExistentKey_ReturnsNull()
        {
            // Act
            var result = _context.GetItem("nonExistentKey");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetItem_WithNullKey_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _context.GetItem(null));
        }

        [TestMethod]
        public void GetItem_Generic_WithValidKeyAndMatchingType_ReturnsCastValue()
        {
            // Arrange
            var key = "testKey";
            var expectedValue = new object();
            _context.SetItem(key, expectedValue);

            // Act
            var result = _context.GetItem<object>(key);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void GetItem_Generic_WithNullKey_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _context.GetItem<object>(null));
        }

        [TestMethod]
        public void GetItem_Generic_WithNonExistentKey_ReturnsNull()
        {
            // Act
            var result = _context.GetItem<object>("nonExistentKey");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ContainsKey_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            var key = "testKey";
            _context.SetItem(key, "value");

            // Act
            var result = _context.ContainsKey(key);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsKey_WithNonExistentKey_ReturnsFalse()
        {
            // Act
            var result = _context.ContainsKey("nonExistentKey");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsKey_WithNullKey_ReturnsFalse()
        {
            // Act
            var result = _context.ContainsKey(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Items_ReturnsReadOnlyDictionary()
        {
            // Arrange
            _context.SetItem("key1", "value1");
            _context.SetItem("key2", "value2");

            // Act
            var items = _context.Items;

            // Assert
            Assert.AreEqual(2, items.Count);
            Assert.IsTrue(items.ContainsKey("key1"));
            Assert.IsTrue(items.ContainsKey("key2"));
        }

        [TestMethod]
        public void SetItem_OverwriteExistingKey_UpdatesValue()
        {
            // Arrange
            var key = "testKey";
            var firstValue = "firstValue";
            var secondValue = "secondValue";
            _context.SetItem(key, firstValue);

            // Act
            _context.SetItem(key, secondValue);

            // Assert
            Assert.AreEqual(secondValue, _context.GetItem(key));
        }
    }
}
