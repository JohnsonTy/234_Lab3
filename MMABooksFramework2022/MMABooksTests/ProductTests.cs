using MMABooksBusiness;
using MMABooksDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            ProductDB db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetStateData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewStateConstructor()
        {
            // not in Data Store - no code
            Product p = new Product();
            Assert.AreEqual(string.Empty, p.ProductCode);
            Assert.AreEqual(string.Empty, p.Description);
            Assert.IsTrue(p.IsNew);
            Assert.IsFalse(p.IsValid);
        }


        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            // retrieves from Data Store
            Product p = new Product("30");
            Assert.AreEqual("30", p.ProductCode);
            Assert.IsTrue(p.Description.Length > 0);
            Assert.IsFalse(p.IsNew);
            Assert.IsTrue(p.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Product p = new Product();
            p.ProductCode = "60";
            p.Description = "Test Description";
            p.Save();
            Product s2 = new Product("60");
            Assert.AreEqual(s2.ProductCode, p.ProductCode);
            Assert.AreEqual(s2.Description, p.Description);
        }

        [Test]
        public void TestUpdate()
        {
            Product p = new Product("70");
            p.Description = "Edited Description";
            p.Save();

            Product s2 = new Product("70");
            Assert.AreEqual(s2.ProductCode, p.ProductCode);
            Assert.AreEqual(s2.Description, p.Description);
        }

        [Test]
        public void TestDelete()
        {
            Product p = new Product("1756");
            p.Delete();
            p.Save();
            Assert.Throws<Exception>(() => new Product("1756"));
        }

        [Test]
        public void TestGetList()
        {
            Product p = new Product();
            List<Product> products = (List<Product>)p.GetList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("Attention! The Battle Brothers are calling for reinforcements", products[0].Description);
            Assert.AreEqual("Warhammer 40,000", products[0].ProductCode);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            Product p = new Product();
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            Product p = new Product();
            Assert.Throws<Exception>(() => p.Save());
            p.ProductCode = "??";
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Product p = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => p.ProductCode = "??????????????");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Product s1 = new Product("A6N6");
            Product s2 = new Product("A6N6");

            s1.Description = "Updated first";
            s1.Save();

            s2.Description = "Updated second";
            Assert.Throws<Exception>(() => s2.Save());
        }
    }
}
