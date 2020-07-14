using System;

/// <summary>
/// このクラスでは Todo オブジェクトをデータベースに書き込んだり、List として取り出したりする機能を提供します。
/// </summary>

namespace TodoManager2.model {

    public class TodoReaderWriter {

        private DatabaseHelper dbHelper;
        private readonly string TABLE_NAME_TODOS = "todos";

        public TodoReaderWriter(DatabaseHelper dbHelper) {
            this.dbHelper = dbHelper;
        }

        public void add(Todo todo) {

            long newID = 0;

            if(dbHelper.getRecordCount(TABLE_NAME_TODOS) > 0) {
                newID = dbHelper.getMaxInColumn(TABLE_NAME_TODOS, "id") + 1;
            }

            string[] columnNames = {
                nameof(Todo.ID),
                nameof(Todo.Title),
                nameof(Todo.Text),
                nameof(Todo.CreationDateTime),
                nameof(Todo.CompletionDateTime),
                nameof(Todo.DueDateTime),
                nameof(Todo.Priority),
                nameof(Todo.IsCompleted),
                nameof(Todo.CompletionComment),
            };

            string[] values = {
                newID.ToString(),
                todo.Title,
                todo.Title,
                todo.CreationDateTime.ToString(),
                todo.CompletionDateTime.ToString(),
                todo.DueDateTime.ToString(),
                todo.Priority,
                todo.IsCompleted.ToString(),
                todo.CompletionComment
            };

            dbHelper.insert(TABLE_NAME_TODOS, columnNames, values);
        }
    }
}
