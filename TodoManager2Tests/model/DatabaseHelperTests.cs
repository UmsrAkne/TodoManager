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
            // 生成時にエラーが出ないかのテスト
            var dbh = new DatabaseHelper(dbName);
        }

        [TestMethod()]
        public void insertTest() {
            var dbh = new DatabaseHelper(dbName);

            string[] columnNames = { "name" };

            string[] values0 = { "valueTest1" };
            string[] values1 = { "valueTest2" };
            string[] values2 = { "valueTest3" };

            dbh.insert("todos", columnNames, values0);
            dbh.insert("todos", columnNames, values1);
            dbh.insert("todos", columnNames, values2);

            var vals = dbh.select("SELECT name FROM todos;");

            Assert.AreEqual(vals.Count, 3);
            Assert.AreEqual(vals[0]["name"], "valueTest1");
            Assert.AreEqual(vals[1]["name"], "valueTest2");
            Assert.AreEqual(vals[2]["name"], "valueTest3");
        }
    }
}