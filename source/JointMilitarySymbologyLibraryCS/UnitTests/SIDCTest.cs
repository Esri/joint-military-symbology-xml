using JointMilitarySymbologyLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SIDCTest and is intended
    ///to contain all SIDCTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SIDCTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SIDC Constructor
        ///</summary>
        [TestMethod()]
        public void SIDCConstructorTest()
        {
            string partA = string.Empty;
            string partB = string.Empty;
            SIDC target = new SIDC(partA, partB);
            Assert.AreEqual(target.PartAString, "1000980000");
            Assert.AreEqual(target.PartBString, "1000000000");
        }

        /// <summary>
        ///A test for SIDC Constructor
        ///</summary>
        [TestMethod()]
        public void SIDCConstructorTest1()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            Assert.AreEqual(target.PartAUInt, (uint)1000980000);
            Assert.AreEqual(target.PartBUInt, (uint)1000000000);
        }

        /// <summary>
        ///A test for PartAString
        ///</summary>
        [TestMethod()]
        public void PartAStringTest()
        {
            uint partA = 0; 
            uint partB = 0; 
            SIDC target = new SIDC(partA, partB);
            string expected = "1000980000";
            string actual;
            target.PartAString = expected;
            actual = target.PartAString;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PartAUInt
        ///</summary>
        [TestMethod()]
        public void PartAUIntTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            uint expected = 1000980000;
            uint actual;
            target.PartAUInt = expected;
            actual = target.PartAUInt;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PartBString
        ///</summary>
        [TestMethod()]
        public void PartBStringTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            string expected = "1000000000";
            string actual;
            target.PartBString = expected;
            actual = target.PartBString;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PartBUInt
        ///</summary>
        [TestMethod()]
        public void PartBUIntTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            uint expected = 1000000000;
            uint actual;
            target.PartBUInt = expected;
            actual = target.PartBUInt;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PartAString and PartAUInt
        ///</summary>
        [TestMethod()]
        public void PartAStringMixTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            string expected = "1000980000";
            uint actual;
            target.PartAString = expected;
            actual = target.PartAUInt;
            Assert.AreEqual((uint)1000980000, actual);
        }

        /// <summary>
        ///A test for PartBString and PartBUInt
        ///</summary>
        [TestMethod()]
        public void PartBStringMixTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            string expected = "1000000000";
            uint actual;
            target.PartBString = expected;
            actual = target.PartBUInt;
            Assert.AreEqual((uint)1000000000, actual);
        }

        /// <summary>
        ///A test for PartBString and PartBUInt
        ///</summary>
        [TestMethod()]
        public void PartBUIntMixTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            uint expected = 1000000000;
            string actual;
            target.PartBUInt = expected;
            actual = target.PartBString;
            Assert.AreEqual("1000000000", actual);
        }

        /// <summary>
        ///A test for PartAString and PartAUInt
        ///</summary>
        [TestMethod()]
        public void PartAUIntMixTest()
        {
            uint partA = 0;
            uint partB = 0;
            SIDC target = new SIDC(partA, partB);
            uint expected = 1000980000;
            string actual;
            target.PartAUInt = expected;
            actual = target.PartAString;
            Assert.AreEqual("1000980000", actual);
        }

        /// <summary>
        ///A test for Invalid SIDC constant
        ///</summary>
        [TestMethod()]
        public void InvalidSIDC()
        {
            SIDC target = SIDC.INVALID;
            uint expected1 = 1000980000;
            uint expected2 = 1000000000;
            uint actual1 = target.PartAUInt;
            uint actual2 = target.PartBUInt;
            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        /// <summary>
        ///A test for Retired SIDC constant
        ///</summary>
        [TestMethod()]
        public void RetiredSIDC()
        {
            SIDC target = SIDC.RETIRED;
            uint expected1 = 1000980000;
            uint expected2 = 1100000000;
            uint actual1 = target.PartAUInt;
            uint actual2 = target.PartBUInt;
            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }
    }
}
