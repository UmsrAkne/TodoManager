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

            newID = Math.Max(newID, todo.ID);

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
                nameof(Todo.WorkSpan)
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
                todo.CompletionComment,
                todo.WorkSpan.Ticks.ToString()
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
            todo.WorkSpan = new TimeSpan((long)todoDictionary[nameof(Todo.WorkSpan)]);

            if(completionDate.CompareTo(new DateTime()) != 0) {
                todo.IsCompleted = true;
            }

            return todo;
        }

        /// <summary>
        /// パラメーターに入力した Todo と同じ ID のレコードを更新します。
        /// </summary>
        /// <param name="updatedTodo"></param>
        public void update(Todo updatedTodo) {
            var commandText = "UPDATE " + TABLE_NAME_TODOS + " "
                            + "SET " 
                            + nameof(Todo.Text) + " = '" + updatedTodo.Text + "', "
                            + nameof(Todo.Title) + " = '" + updatedTodo.Title + "', "
                            + nameof(Todo.Priority) + " = '" + updatedTodo.Priority + "', "
                            + nameof(Todo.CompletionDateTime) + " = '" + updatedTodo.CompletionDateTime.ToString() + "', "
                            + nameof(Todo.CompletionComment) + " = '" + updatedTodo.CompletionComment + "', "
                            + nameof(Todo.IsCompleted) + " = '" + updatedTodo.IsCompleted.ToString() + "', "
                            + nameof(Todo.CreationDateTime) + " = '" + updatedTodo.CreationDateTime.ToString() + "', "
                            + nameof(Todo.DueDateTime) + " = '" + updatedTodo.DueDateTime.ToString() + "' "
                            + "WHERE " + "id = " + updatedTodo.ID.ToString() + ";";

            dbHelper.executeNonQuery(commandText);
        }

        /// <summary>
        /// 指定したIDのレコードをデータベースから削除します
        /// </summary>
        /// <param name="id"></param>
        public void delete(int id) {
            dbHelper.executeNonQuery("DELETE FROM " + TABLE_NAME_TODOS + " WHERE id = " + id + ";");
        }

        /// <summary>
        /// タグを文字列のリストとして取得します。
        /// </summary>
        /// <param name="tagMaptableName">タグの名前が入っているテーブルの名前を入力します。</param>
        /// <param name="tagNameColumnName">タグの名前が入っている列名を入力します</param>
        /// <returns></returns>
        public List<string> getTags(string tagMaptableName, string tagNameColumnName) {
            var commandText = "SELECT " + tagNameColumnName + " FROM " + tagMaptableName + ";";
            var dics = dbHelper.select(commandText);
            List<String> tags = new List<String>();
            dics.ForEach(d => tags.Add((string)d[tagNameColumnName]));
            return tags;
        }

        public void addTag(string tagMapTableName,string tagNameColumnName, string tag) {
            long tagID = 0;
            if(dbHelper.getRecordCount(tagMapTableName) > 0) {
                tagID = dbHelper.getMaxInColumn(tagMapTableName, "id") + 1;
            }

            string[] columnNames = { "id", tagNameColumnName };
            string[] values = { tagID.ToString(), tag };

            dbHelper.insert(tagMapTableName, columnNames, values);
        }

        public List<Todo> getTodoFromTag(string tag) {
            var commandText = "WITH t1 AS"
                            + "(" + " " 
                            +   "SELECT tag_id, todo_id FROM tag_maps" + " "
                            +   "WHERE tag_id = (SELECT id FROM tags WHERE name = '" + tag + "')"
                            + ")" + " "
                            + "SELECT * FROM t1" + " "
                            + "INNER JOIN todos ON t1.todo_id = todos.id;"; 

            var dic = dbHelper.select(commandText);
            List<Todo> todos = new List<Todo>();
            dic.ForEach(d => { todos.Add(toTodo(d)); });
            return todos;
        }
    }
}
