using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// このクラスでは Todo オブジェクトをデータベースに書き込んだり、List として取り出したりする機能を提供します。
/// </summary>

namespace TodoManager2.model {

    public enum TagsTableColumnName{
        id,
        name
    }

    public enum TagMapsTableColumnName{
        id,
        tag_id,
        todo_id
    }

    public class TodoReaderWriter {

        private DatabaseHelper dbHelper;

        public readonly string todoTableName = "todos";
        public readonly string tagsTableName = "tags";
        public readonly string tagMapsTableName = "tag_maps";


        public TodoReaderWriter(DatabaseHelper dbHelper) {
            this.dbHelper = dbHelper;
        }

        public void add(Todo todo) {

            long newID = 0;

            if(dbHelper.getRecordCount(todoTableName) > 0) {
                newID = dbHelper.getMaxInColumn(todoTableName, "id") + 1;
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

            dbHelper.insert(todoTableName, columnNames, values);
        }

        public Todo getTodo(int id) {
            var commandText = "SELECT * FROM " + todoTableName + " WHERE id = " + id;
            var dics = dbHelper.select(commandText);

            if (dics.Count == 0) {
                throw new ArgumentException("引数に指定されたIDに該当するレコードがありませんでした");
            }
            Dictionary<string ,object> todoDictionary = dics[0];

            return toTodo(todoDictionary);
        }

        public List<Todo> getIncompleteTodos() {
            var commandText = "SELECT * FROM " + todoTableName + " " 
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
            var commandText = "SELECT * FROM " + todoTableName + " "
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
            var commandText = "UPDATE " + todoTableName + " "
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
            dbHelper.executeNonQuery("DELETE FROM " + todoTableName + " WHERE id = " + id + ";");
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

        public List<Todo> getTodoFromTag(List<Tag> tagList) {
            string tagIDColName = TagMapsTableColumnName.tag_id.ToString();
            string todoIDColName = TagMapsTableColumnName.todo_id.ToString();

            var tags = "";
            tagList.ForEach(t => tags += "'" + t.Content + "', " );
            tags = tags.Substring(0, tags.Length - 2);

            var commandText = "WITH t1 AS"
                            + "(" + " " 
                            +   "SELECT " + tagIDColName + ", " + todoIDColName + " "
                            +   "FROM " + tagMapsTableName + " "
                            +   "WHERE " + tagIDColName + " IN ("
                            +   "SELECT id FROM " + tagsTableName + " WHERE " + TagsTableColumnName.name + " IN (" + tags + "))"
                            + ")" + " "
                            + "SELECT * FROM t1" + " "
                            + "INNER JOIN " + todoTableName + " ON "
                            + "t1." + todoIDColName + " = " + todoTableName + ".id;"; 

            var dic = dbHelper.select(commandText);
            List<Todo> todos = new List<Todo>();
            dic.ForEach(d => { todos.Add(toTodo(d)); });
            return todos;
        }

        public void attachTag(long todoID, long tagID) {
            var searchDuplicateCommand = "SELECT id FROM " + tagMapsTableName + " "
                                       + "WHERE " + TagMapsTableColumnName.tag_id.ToString() + " = " + tagID.ToString() + " "
                                       + "AND " + TagMapsTableColumnName.todo_id + " = " + todoID.ToString() + ";";

            var dics = dbHelper.select(searchDuplicateCommand);

            if(dics.Count > 0) {
                return;
            }

            string[] columnNames = {
                TagMapsTableColumnName.tag_id.ToString(), TagMapsTableColumnName.todo_id.ToString() };
            string[] values = { tagID.ToString(), todoID.ToString() };
            dbHelper.insert(tagMapsTableName, columnNames, values);
        }

        public void detachTag(long todoID, long tagID) {
            var commandText = "DELETE FROM " + tagMapsTableName + " "
                            + "WHERE " + TagMapsTableColumnName.todo_id + " = " + todoID.ToString() + " "
                            + "AND " + TagMapsTableColumnName.tag_id + " = " + tagID.ToString() + ";";

            dbHelper.executeNonQuery(commandText);
        }

        public void deleteTag(string tag) {
            var commandText = "DELETE FROM " + tagsTableName + " WHERE " + TagsTableColumnName.name + " = '" + tag + "';";
            dbHelper.executeNonQuery(commandText);
        }

        public List<Todo> getTodo(TodoSearchOption searchOption) {
            string tagIDColName = TagMapsTableColumnName.tag_id.ToString();
            string todoIDColName = TagMapsTableColumnName.todo_id.ToString();

            var narrowDownTodoTableSQL = "SELECT * FROM " + todoTableName + " WHERE ";

            // 作成日時によってフィルタリング
            if(searchOption.SearchStartPoint != DateTime.MinValue) {
                narrowDownTodoTableSQL += nameof(Todo.CreationDateTime) + " > '" + searchOption.SearchStartPoint + "' AND ";
            }

            // 完了、未完了の Todo をフィルタリング
            if(searchOption.ShouldSelectComplitionTodo || searchOption.ShouldSelectIncompleteTodo) {
                if (searchOption.ShouldSelectComplitionTodo) {
                    narrowDownTodoTableSQL += nameof(Todo.IsCompleted) + " = 'True'";
                }

                if (searchOption.ShouldSelectIncompleteTodo) {
                    if (searchOption.ShouldSelectComplitionTodo) {
                        narrowDownTodoTableSQL += " OR ";
                    }
                    narrowDownTodoTableSQL += nameof(Todo.IsCompleted) + " = 'False'";
                }
            }

            var commandText = "";

            if(searchOption.Tags.Count > 0) {
                var tags = "";
                searchOption.Tags.ForEach(t => tags += "'" + t.Content + "', ");
                tags = tags.Substring(0, tags.Length - 2);

                string groupBy = "GROUP BY " + todoIDColName;
                if (!searchOption.TagSearchTypeIsOR) {
                    groupBy += " HAVING COUNT(*) = " + searchOption.Tags.Count + " ";
                }

                commandText = "WITH t1 AS"
                                + "(" + " "
                                + "SELECT " + tagIDColName + ", " + todoIDColName + ", COUNT(*)" + " "
                                + "FROM " + tagMapsTableName + " "
                                + "WHERE " + tagIDColName + " IN ("
                                + "SELECT id FROM " + tagsTableName + " WHERE " + TagsTableColumnName.name + " IN (" + tags + ") "
                                + ") " + groupBy
                                + "), t2 AS (" + narrowDownTodoTableSQL + ")"
                                + "SELECT * FROM t1" + " "
                                + "INNER JOIN t2 ON "
                                + "t1." + todoIDColName + " = t2.id "
                                + "ORDER BY " + nameof(Todo.CreationDateTime) + " "
                                + "LIMIT " + searchOption.ResultCountLimit + ";";
            }
            else {
                // tag による絞り込みが不要である場合、todoTable の絞り込みのためのsqlをそのままコマンドとして採用する
                commandText = narrowDownTodoTableSQL + ";";
            }

            var dic = dbHelper.select(commandText);
            List<Todo> todos = new List<Todo>();
            dic.ForEach(d => { todos.Add(toTodo(d)); });
            return todos;
        }
    }
}
