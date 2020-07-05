using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoManager2.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager2.model.Tests {
    [TestClass()]
    public class DatabaseHelperTests {

        private string dbName = "testDatabase";

        [TestMethod()]
        public void DatabaseHelperTest() {
            var dbh = new DatabaseHelper(dbName);
        }
    }
}