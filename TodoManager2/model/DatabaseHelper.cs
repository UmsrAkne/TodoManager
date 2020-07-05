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
        }

        private void create() {
            SQLiteConnection.CreateFile(DatabaseName + ".sqlite");
        }
    }
}
