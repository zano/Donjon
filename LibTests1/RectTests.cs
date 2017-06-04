using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Tests
{
    [TestClass()]
    public class RectTests
    {
        [TestMethod()]
        public void NormalizedTest()
        {
            Rect rectangle = new Rect { X = 2, Y = 3, Width = 4, Height = 5 };
            Rect rectangle2 = new Rect { X = 2, Y = 3, Width = 4, Height = 5 };
            Rect normalized = rectangle.Normalized();
            Assert.AreEqual(rectangle.X, rectangle2.X);
            Assert.AreEqual(rectangle.X, normalized.X);
            Assert.AreEqual(rectangle2.X, normalized.X);
        }
        [TestMethod()]
        public void NormalizedWidthIsNegativeTest()
        {
            Rect rectangle = new Rect { X = 2, Y = 3, Width = -4, Height = 5 };
            Rect expected = new Rect { X = -2, Y = 3, Width = 4, Height = 5 };
            Rect normalized = rectangle.Normalized();
            Assert.AreEqual(normalized.X, expected.X);
           
        }
    }
}