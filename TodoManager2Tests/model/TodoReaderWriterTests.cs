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
            var updatedTodo = new Todo(testCreationDate,2);
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

            List<Todo> todoList = new List<Todo>(new Todo[] {  new Todo(), new Todo(), new Todo() });
            todoList.ForEach(t => todoReaderWriter.add(t));
            Assert.AreEqual(dbHelper.getRecordCount("todos"), 3);

            todoReaderWriter.delete(1);

            Assert.AreEqual(dbHelper.getRecordCount("todos"), 2);

            todoReaderWriter.delete(0);
            todoReaderWriter.delete(1);
            todoReaderWriter.delete(2);

            Assert.AreEqual(dbHelper.getRecordCount("todos"), 0);
        }
    }
}