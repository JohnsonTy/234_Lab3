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
using Google.Protobuf.WellKnownTypes;

namespace MMABooksTests
{
    internal class CustomerTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            CustomerDB db = new CustomerDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetStateData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewCustomerConstructor()
        {
            // not in Data Store - no code
            Customer c = new Customer();
            Assert.AreEqual(string.Empty, c.Name);
            Assert.AreEqual(string.Empty, c.Address);
            Assert.IsTrue(c.IsNew);
            Assert.IsFalse(c.IsValid);
        }


        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            Customer c = new Customer(1);

            Assert.AreEqual("Molunguri, A", c.Name);
            Assert.AreEqual("1108 Johanna Bay Drive", c.Address);
            Assert.IsFalse(c.IsNew);
            Assert.IsTrue(c.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Customer c = new Customer();
            c.Name = "Minnie Mouse";
            c.Address = "101 Main Street";
            c.City = "Orlando";
            c.State = "FL";
            c.ZipCode = "10001";
            c.Save();
            Customer c2 = new Customer(c.CustomerID);
            Assert.AreEqual(c2.Address, c.Address);
            Assert.AreEqual(c2.Name, c.Name);
        }

        [Test]
        public void TestUpdate()
        {
            Customer c = new Customer();
            c.Name = "testName";
            c.Address = "testAddress 5";
            c.City = "testCity";
            c.State = "testState";
            c.ZipCode = "12345";
            c.Save();

            Customer c2 = new Customer();
            c2.Name = "Unedited Name";
            Assert.AreEqual(c2.Address, c.Address);
            Assert.AreEqual(c2.Name, c.Name);
        }

        [Test]
        public void TestDelete()
        {
            Customer c = new Customer(1);
            c.Delete();
            c.Save();
            Assert.Throws<Exception>(() => new Customer(1));
        }

        [Test]
        public void TestGetList()
        {
            Customer c = new Customer(1);
            List<Customer> customers = (List<Customer>)c.GetList();
            Assert.AreEqual(696, customers.Count);
            Assert.AreEqual("Mills, Richard", customers[0].Name);
            Assert.AreEqual(999, customers[0].CustomerID);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
             Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
            c.Name = "??????????";
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Customer c = new Customer(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => c.Address = "");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Customer c1 = new Customer(1);
            Customer c2 = new Customer(2);

            c1.Name = "Updated first";
            c1.Save();

            c2.Name = "Updated second";
            Assert.Throws<Exception>(() => c2.Save());
        }
    }
}
