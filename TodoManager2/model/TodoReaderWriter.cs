using System;

/// <summary>
/// このクラスでは Todo オブジェクトをデータベースに書き込んだり、List として取り出したりする機能を提供します。
/// </summary>

namespace TodoManager2.model {

    public class TodoReaderWriter {

        private DatabaseHelper dbHelper;

        public TodoReaderWriter(DatabaseHelper dbHelper) {
            this.dbHelper = dbHelper;
        }
    }
}
