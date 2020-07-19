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

            string cName = "id";
            string[] columnNames = { cName };

            string[] values0 = { "1" };
            string[] values1 = { "2" };
            string[] values2 = { "3" };

            dbh.insert("todos", columnNames, values0);
            dbh.insert("todos", columnNames, values1);
            dbh.insert("todos", columnNames, values2);


            var vals = dbh.select("SELECT " + cName + " FROM todos;");

            Assert.AreEqual(vals.Count, 3);
            Assert.AreEqual(vals[0][cName], (long)1);
            Assert.AreEqual(vals[1][cName], (long)2);
            Assert.AreEqual(vals[2][cName], (long)3);
        }

        [TestMethod()]
        public void addNotNullColumnTest() {
            var dbh = new DatabaseHelper(dbName);

            string defaultColumnName = "id";
            string secondColumnName = "testColumn";

            string[] columnNames =  { defaultColumnName, secondColumnName };
            string[] value0 =       { "2",               "testText" };

            dbh.addNotNullColumn("todos", secondColumnName, "TEXT");
            dbh.insert("todos", columnNames, value0);
            var vals = dbh.select("SELECT " + secondColumnName + " FROM todos;");

            Assert.AreEqual(vals.Count, 1);
            Assert.AreEqual(vals[0][secondColumnName], "testText");
        }

        [TestCleanup()]
        public void cleanup() {
            System.IO.File.Delete(dbName + ".sqlite");
        }

        [TestMethod()]
        public void getMaxInColumnTest() {
            var dbh = new DatabaseHelper(dbName);

            string[] columnNames = { "id" };
            string[] val0 = { "0" };
            string[] val1 = { "1" };
            string[] val2 = { "2" };

            dbh.insert("todos", columnNames, val0);
            dbh.insert("todos", columnNames, val1);
            dbh.insert("todos", columnNames, val2);

            Assert.AreEqual(dbh.getMaxInColumn("todos", "id"), 2);
        }

        [TestMethod()]
        public void getRecordCountTest() {
            var dbh = new DatabaseHelper(dbName);
            Assert.AreEqual(dbh.getRecordCount("todos"), 0);

            dbh.insert("todos", new string[] { "id" }, new string[] { "1" });
            Assert.AreEqual(dbh.getRecordCount("todos"), 1);
        }

        [TestMethod()]
        public void createTableTest() {
            var dbh = new DatabaseHelper(dbName);
            dbh.createTable("tagTable");
            Assert.AreEqual(dbh.getRecordCount("tagTable"), 0);
        }

        [TestMethod()]
        public void existColumnTest() {
            var dbh = new DatabaseHelper(dbName);
            dbh.createTable("testTable");
            dbh.addNotNullColumn("testTable","testColumn","TEXT");
            Assert.AreEqual(dbh.existColumn("testTable", "testColumn"), true);
            Assert.AreEqual(dbh.existColumn("testTable", "testColumnA"), false);
        }
    }
}