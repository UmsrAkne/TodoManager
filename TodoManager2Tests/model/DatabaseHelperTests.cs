using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoManager2.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    }
}