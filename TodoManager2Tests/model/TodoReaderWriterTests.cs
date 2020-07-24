using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoManager2.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoManager2.model.Tests {
    [TestClass()]
    public class TodoReaderWriterTests {
        [TestMethod()]
        public void addTest() {
            System.IO.File.Delete("testDB" + ".sqlite");
            TodoReaderWriter todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            var todo1 = new Todo();
            todoReaderWriter.add(todo1);
        }

        private DatabaseHelper getDatabaseHelper() {
            var dbHelper = new DatabaseHelper("testDB");
            dbHelper.addNotNullColumn("todos", nameof(Todo.Title), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.Text), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.CreationDateTime), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.CompletionDateTime), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.DueDateTime), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.Priority), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.IsCompleted), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.CompletionComment), "TEXT");
            dbHelper.addNotNullColumn("todos", nameof(Todo.WorkSpan), "INTEGER");

            return dbHelper;

        }

        [TestMethod()]
        public void getTodoTest() {
            var todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            var todo1 = new Todo();
            todo1.IsCompleted = true;
            todo1.Title = "testTitle";
            todo1.Text = "testText";
            todo1.WorkSpan = new TimeSpan(0, 10, 0);

            todoReaderWriter.add(todo1);
            var todo = todoReaderWriter.getTodo(0);

            Assert.AreEqual(todo.Title, "testTitle");
            Assert.AreEqual(todo.Text, "testText");
            Assert.AreEqual(todo.ID, 0);
            Assert.AreEqual(todo.WorkSpan, new TimeSpan(0, 10, 0));
        }

        [TestCleanup()]
        public void cleanup() {
            System.IO.File.Delete("testDB" + ".sqlite");
        }

        [TestMethod()]
        public void getIncompleteTodosTest() {
            cleanup();
            var todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            Todo[] todos = { new Todo(), new Todo(), new Todo() };
            List<Todo> testTodoList = new List<Todo>(new Todo[] { new Todo(), new Todo(), new Todo(), new Todo() });
            testTodoList[3].IsCompleted = true;

            testTodoList.ForEach(t => todoReaderWriter.add(t));
            List<Todo> resultTodoList = todoReaderWriter.getIncompleteTodos();
            Assert.AreEqual(resultTodoList.Count, 3);
        }

        [TestMethod()]
        public void getTodosWithinTest() {
            cleanup();
            var todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            List<Todo> testTodoList = new List<Todo>();
            testTodoList.Add(new Todo(DateTime.Now, 1));
            testTodoList.Add(new Todo(DateTime.Now, 2));
            testTodoList.Add(new Todo(new DateTime(DateTime.Now.Ticks + (100 * 10000 * 10)), 3));
            testTodoList.Add(new Todo(new DateTime(DateTime.Now.Ticks - (100 * 10000 * 30)), 4));

            testTodoList.ForEach(t => todoReaderWriter.add(t));
            List<Todo> resultTodoList = todoReaderWriter.getTodosWithin(new DateTime(DateTime.Now.Ticks - 100 * 10000 * 10), DateTime.Now);
            Assert.AreEqual(resultTodoList.Count, 2);

        }

        [TestMethod()]
        public void updateTest() {
            cleanup();
            var todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            List<Todo> todoList = new List<Todo>();
            todoList.Add(new Todo(DateTime.Now, 1));
            todoList.Add(new Todo(DateTime.Now, 2));
            todoList.Add(new Todo(DateTime.Now, 3));

            todoList[1].Text = "testText";

            todoList.ForEach(t => todoReaderWriter.add(t));

            var ts = todoReaderWriter.getIncompleteTodos();

            Assert.AreEqual(todoReaderWriter.getTodo(2).Text, "testText");

            var testCreationDate = new DateTime(DateTime.Now.Ticks + 100 * 100 * 100 * 100);
            var updatedTodo = new Todo(testCreationDate, 2);
            updatedTodo.Text = "updated";
            updatedTodo.Title = "updateTitle";
            todoReaderWriter.update(updatedTodo);

            Assert.AreEqual(todoReaderWriter.getTodo(2).Text, "updated");
            Assert.AreEqual(todoReaderWriter.getTodo(2).Title, "updateTitle");
            Assert.AreEqual(todoReaderWriter.getTodo(2).CreationDateTime.ToString(), testCreationDate.ToString());
        }

        [TestMethod()]
        public void deleteTest() {
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            List<Todo> todoList = new List<Todo>(new Todo[] { new Todo(), new Todo(), new Todo() });
            todoList.ForEach(t => todoReaderWriter.add(t));
            Assert.AreEqual(dbHelper.getRecordCount("todos"), 3);

            todoReaderWriter.delete(1);

            Assert.AreEqual(dbHelper.getRecordCount("todos"), 2);

            todoReaderWriter.delete(0);
            todoReaderWriter.delete(1);
            todoReaderWriter.delete(2);

            Assert.AreEqual(dbHelper.getRecordCount("todos"), 0);
        }

        [TestMethod()]
        public void getTagsTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable("tag_maps");
            dbHelper.addNotNullColumn("tag_maps", "name", "TEXT");

            var tags = todoReaderWriter.getTags("tag_maps", "name");
        }

        [TestMethod()]
        public void addTagTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable("tag_maps");
            dbHelper.addNotNullColumn("tag_maps", "name", "TEXT");
            todoReaderWriter.addTag("tag_maps", "name", "tag1");
            todoReaderWriter.addTag("tag_maps", "name", "tag2");

            var tags = todoReaderWriter.getTags("tag_maps", "name");
            Assert.AreEqual(tags[0], "tag1");
            Assert.AreEqual(tags[1], "tag2");
        }

        [TestMethod()]
        public void getTodoFromTagTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable("tags");
            dbHelper.addNotNullColumn("tags", "name", "TEXT");
            todoReaderWriter.addTag("tags", "name", "testTag1");
            todoReaderWriter.addTag("tags", "name", "testTag2");

            dbHelper.createTable("tag_maps");
            dbHelper.addNotNullColumn("tag_maps", "todo_id", "INTEGER");
            dbHelper.addNotNullColumn("tag_maps", "tag_id", "INTEGER");

            string[] tagMapColumnNames = { "todo_id", "tag_id" };
            string[] tagMapValues1 = { "0", "0" };
            string[] tagMapValues2 = { "0", "1" };
            string[] tagMapValues3 = { "1", "1" };

            dbHelper.insert("tag_maps", tagMapColumnNames, tagMapValues1);
            dbHelper.insert("tag_maps", tagMapColumnNames, tagMapValues2);
            dbHelper.insert("tag_maps", tagMapColumnNames, tagMapValues3);

            Todo[] testTodos = { new Todo(), new Todo() };
            testTodos[0].Title = "testTodo0のタイトル";
            testTodos[1].Title = "testTodo1のタイトル";
            todoReaderWriter.add(testTodos[0]);
            todoReaderWriter.add(testTodos[1]);

            var tags1 = new List<Tag>(new Tag[] { new Tag("testTag1") });
            var tags2 = new List<Tag>(new Tag[] { new Tag("testTag2") });
            var allTags = new List<Tag>(new Tag[] { new Tag("testTag1"), new Tag("testTag2") });

            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags1).Count, 1);
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags2).Count, 2);
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(allTags).Count, 3);

        }

        [TestMethod()]
        public void attachTagTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable(todoReaderWriter.tagMapsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.tag_id.ToString(), "INTEGER");
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.todo_id.ToString(), "INTEGER");

            todoReaderWriter.attachTag(0, 0);
            todoReaderWriter.attachTag(0, 1);
            todoReaderWriter.attachTag(1, 1);
            todoReaderWriter.attachTag(1, 1);

            // 4回タグの付与を行う。重複を一つ含むため、最終的なレコード数は 3 になる。
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagMapsTableName), 3);

            dbHelper.createTable(todoReaderWriter.tagsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "TEXT");

            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "tag0");
            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "tag1");

            Todo[] todos = { new Todo(), new Todo() };
            todos[0].Title = "todo0title";
            todos[1].Title = "todo1title";

            todoReaderWriter.add(todos[0]);
            todoReaderWriter.add(todos[1]);

            List<Tag> tags0 = new List<Tag>(new Tag[] { new Tag("tag0") });
            List<Tag> tags1 = new List<Tag>(new Tag[] { new Tag("tag1") });

            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags0).Count, 1);
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags1).Count, 2);
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags0)[0].Title, "todo0title");
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags1)[0].Title, "todo0title");
            Assert.AreEqual(todoReaderWriter.getTodoFromTag(tags1)[1].Title, "todo1title");
        }

        [TestMethod()]
        public void detachTagTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable(todoReaderWriter.tagMapsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.tag_id.ToString(), "INTEGER");
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.todo_id.ToString(), "INTEGER");

            todoReaderWriter.attachTag(0, 0);
            todoReaderWriter.attachTag(0, 1);
            todoReaderWriter.attachTag(1, 2);

            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagMapsTableName), 3);
            todoReaderWriter.detachTag(0, 0);
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagMapsTableName), 2);
            todoReaderWriter.detachTag(0, 1);
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagMapsTableName), 1);

            //  仮に消す対象が存在しなくても正常に処理は終了する。
            todoReaderWriter.detachTag(0, 1);
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagMapsTableName), 1);

            var dics = dbHelper.select("select * FROM " + todoReaderWriter.tagMapsTableName)[0];
            Assert.AreEqual(dics[TagMapsTableColumnName.todo_id.ToString()], (long)1);
            Assert.AreEqual(dics[TagMapsTableColumnName.tag_id.ToString()], (long)2);
        }

        [TestMethod()]
        public void deleteTagTest() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable(todoReaderWriter.tagsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "TEXT");

            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "testTag1");
            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "testTag2");
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagsTableName), 2);

            todoReaderWriter.deleteTag("testTag1");
            Assert.AreEqual(dbHelper.getRecordCount(todoReaderWriter.tagsTableName), 1);

            var tags = todoReaderWriter.getTags(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString());
            Assert.AreEqual(tags[0], "testTag2");
        }

        [TestMethod()]
        public void getTodoTest1() {
            cleanup();
            var dbHelper = getDatabaseHelper();
            var todoReaderWriter = new TodoReaderWriter(dbHelper);

            dbHelper.createTable(todoReaderWriter.tagsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "TEXT");

            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "testTag1");
            todoReaderWriter.addTag(todoReaderWriter.tagsTableName, TagsTableColumnName.name.ToString(), "testTag2");

            dbHelper.createTable(todoReaderWriter.tagMapsTableName);
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.tag_id.ToString(), "INTEGER");
            dbHelper.addNotNullColumn(todoReaderWriter.tagMapsTableName, TagMapsTableColumnName.todo_id.ToString(), "INTEGER");

            var todos = new List<Todo>(new Todo[] {
                new Todo(new DateTime(2010,01,01),0), 
                new Todo(new DateTime(2010,01,01),1), 
                new Todo(new DateTime(2010,01,01),2), 
                new Todo(DateTime.Now,3),
                new Todo(DateTime.Now,4)
            });

            todos[0].IsCompleted = true;
            todos[1].IsCompleted = true;
            todos.ForEach(t => todoReaderWriter.add(t));
            todoReaderWriter.attachTag(0, 0);
            todoReaderWriter.attachTag(1, 0);
            todoReaderWriter.attachTag(1, 1);
            todoReaderWriter.attachTag(2, 1);
            todoReaderWriter.attachTag(3, 1);

            var todoSearchOption = new TodoSearchOption();
            todoSearchOption.TagSearchTypeIsOR = true;

            // タグ指定なしで実行した場合は全todoが返る
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count,5);

            todoSearchOption.Tags = new List<Tag>(new Tag[] { new Tag("testTag1"), new Tag("testTag2") });
            var tt = todoReaderWriter.getTodo(todoSearchOption);
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count,4); // タグがついていないTodoが一つあるので、5 - 1 で 4

            
            todoSearchOption.Tags = new List<Tag>(new Tag[] { new Tag("testTag1") });
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count,2); 

            todoSearchOption.Tags = new List<Tag>(new Tag[] { new Tag("testTag2") });
            todoSearchOption.ShouldSelectComplitionTodo = false;
            todoSearchOption.SearchStartPoint = new DateTime(2015, 01, 01);
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count,1);

            todoSearchOption = new TodoSearchOption();
            todoSearchOption.Tags = new List<Tag>(new Tag[] { new Tag("testTag1"), new Tag("testTag2") });
            todoSearchOption.TagSearchTypeIsOR = false;
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count, 1);

            todoSearchOption.TagSearchTypeIsOR = true;
            todoSearchOption.ResultCountLimit = 2;
            Assert.AreEqual(todoReaderWriter.getTodo(todoSearchOption).Count, 2);
        }
    }
}