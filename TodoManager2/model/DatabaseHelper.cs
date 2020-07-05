using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace TodoManager2.model {
    public class DatabaseHelper {

        public String DatabaseName { get; private set; } = "";

        public DatabaseHelper(string databaseName) {
            DatabaseName = databaseName;
            create();
            createTable("todos");
        }

        private void create() {
            SQLiteConnection.CreateFile(DatabaseName + ".sqlite");
        }

        private void createTable(String tableName) {
            var createTableCommandText = "CREATE TABLE IF NOT EXISTS " + tableName + " ( " +
                                         "name TEXT NOT NULL" +
                                         ");";
            executeNonQuery(createTableCommandText);
        }

        private void executeNonQuery(string commandText) {
            using (var conn = new SQLiteConnection("Data Source=" + DatabaseName + ".sqlite")) {
                conn.Open();
                using(var command = conn.CreateCommand()) {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void insert(string tableName, string[] columnNames, string[] values) {
            var commandText = "INSERT INTO " + tableName + " ";

            var columnNamePart = "(";
            new List<String>(columnNames).ForEach(tn => columnNamePart += tn + ", ");
            columnNamePart = columnNamePart.Substring(0, columnNamePart.Length - 2);
            columnNamePart += ")";

            var valuePart = "VALUES (";
            new List<String>(values).ForEach(v => valuePart += "'" + v + "', ");
            valuePart = valuePart.Substring(0, valuePart.Length - 2);
            valuePart += ");";

            commandText += columnNamePart + " " + valuePart;

            executeNonQuery(commandText);
        }

    }
}
