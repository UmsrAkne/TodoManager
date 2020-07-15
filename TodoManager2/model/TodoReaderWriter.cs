using System;
using System.Collections.Generic;

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
                todo.Text,
                todo.CreationDateTime.ToString(),
                todo.CompletionDateTime.ToString(),
                todo.DueDateTime.ToString(),
                todo.Priority,
                todo.IsCompleted.ToString(),
                todo.CompletionComment
            };

            dbHelper.insert(TABLE_NAME_TODOS, columnNames, values);
        }

        public Todo getTodo(int id) {
            var commandText = "SELECT * FROM " + TABLE_NAME_TODOS + " WHERE id = " + id;
            var dics = dbHelper.select(commandText);

            if (dics.Count == 0) {
                throw new ArgumentException("引数に指定されたIDに該当するレコードがありませんでした");
            }
            Dictionary<string ,object> todoDictionary = dics[0];

            return toTodo(todoDictionary);
        }

        public List<Todo> getIncompleteTodos() {
            var commandText = "SELECT * FROM " + TABLE_NAME_TODOS + " " 
                            + "WHERE " + nameof(Todo.IsCompleted) + " = 'False'";

            var dics = dbHelper.select(commandText);
            if (dics.Count == 0) {
                return new List<Todo>();
            }

            List<Todo> todos = new List<Todo>();
            dics.ForEach(d => todos.Add(toTodo(d)));

            return todos;
        }

        /// <summary>
        /// パラメーターで指定した期間内に作成したTodoをリストで取得します。
        /// </summary>
        /// <param name="minDate">ここで指定した日時以降のTodoを取得します</param>
        /// <param name="maxDate">ここで指定した日時以前のTodoを取得します</param>
        /// <returns></returns>
        public List<Todo> getTodosWithin(DateTime minDate, DateTime maxDate) {
            var commandText = "SELECT * FROM " + TABLE_NAME_TODOS + " "
                            + "WHERE " + nameof(Todo.CreationDateTime).ToString() + " >= '" + minDate.ToString() + "' "
                            + "AND " + nameof(Todo.CreationDateTime).ToString() + " <= '" + maxDate.ToString() + "';";

            var dics = dbHelper.select(commandText);
            if (dics.Count == 0) {
                return new List<Todo>();
            }

            List<Todo> todos = new List<Todo>();
            dics.ForEach(d => todos.Add(toTodo(d)));
            return todos;
        }

        /// <summary>
        /// データベースから取得してきたディクショナリーを元に Todo を生成します。
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private Todo toTodo(Dictionary<string, object> todoDictionary) {

            var creationDate = DateTime.Parse((string)todoDictionary[nameof(Todo.CreationDateTime)]);
            var completionDate = DateTime.Parse((string)todoDictionary[nameof(Todo.CompletionDateTime)]);
            var dueDate= DateTime.Parse((string)todoDictionary[nameof(Todo.DueDateTime)]);
            var uid = (long)todoDictionary["id"];
            var todo = new Todo(creationDate, completionDate, uid);

            todo.DueDateTime = dueDate;
            todo.Text = (string)todoDictionary[nameof(Todo.Text)];
            todo.Title = (string)todoDictionary[nameof(Todo.Title)];
            todo.Priority = (string)todoDictionary[nameof(Todo.Priority)];
            todo.CompletionComment = (string)todoDictionary[nameof(Todo.CompletionComment)];

            if(completionDate.CompareTo(new DateTime()) != 0) {
                todo.IsCompleted = true;
            }

            return todo;
        }
    }
}
