using JointMilitarySymbologyLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for librarianTest and is intended
    ///to contain all librarianTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LibrarianTest
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
        ///A test for librarian Constructor
        ///</summary>
        [TestMethod()]
        public void LibrarianConstructorTest()
        {
            string configPath = string.Empty;
            Librarian target = new Librarian(configPath);
            Assert.IsNotNull(target, "Librarian object is null.");
        }

        /// <summary>
        ///A test for makeSymbol
        ///</summary>
        [TestMethod()]
        public void MakeSymbolTest()
        {
            string configPath = string.Empty;
            Librarian target = new Librarian(configPath);
            SIDC sidc = new SIDC(1003010000,1100000000);
            Symbol actual = null;
            actual = target.MakeSymbol(sidc);
            Assert.IsNotNull(actual, "Symbol object is null.");
        }

        /// <summary>
        ///A test of the InvalidSymbol property
        ///</summary>
        [TestMethod()]
        public void InvalidSymbolTest()
        {
            string configPath = string.Empty;
            Librarian target = new Librarian(configPath);
            Symbol sym = target.InvalidSymbol;
            string expected1 = "1000980000";
            string expected2 = "1000000000";
            string actual1 = sym.SIDC.PartAString;
            string actual2 = sym.SIDC.PartBString;
            Assert.AreEqual(actual1, expected1);
            Assert.AreEqual(actual2, expected2);
        }

        /// <summary>
        ///A test of the RetiredSymbol property
        ///</summary>
        [TestMethod()]
        public void RetiredSymbolTest()
        {
            string configPath = string.Empty;
            Librarian target = new Librarian(configPath);
            Symbol sym = target.RetiredSymbol;
            string expected1 = "1000980000";
            string expected2 = "1100000000";
            string actual1 = sym.SIDC.PartAString;
            string actual2 = sym.SIDC.PartBString;
            Assert.AreEqual(actual1, expected1);
            Assert.AreEqual(actual2, expected2);
        }
    }
}
