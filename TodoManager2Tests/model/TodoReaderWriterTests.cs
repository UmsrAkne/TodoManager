﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            return dbHelper;

        }

        [TestMethod()]
        public void getTodoTest() {
            var todoReaderWriter = new TodoReaderWriter(getDatabaseHelper());

            var todo1 = new Todo();
            todo1.IsCompleted = true;
            todo1.Title = "testTitle";
            todo1.Text = "testText";

            todoReaderWriter.add(todo1);
            var todo = todoReaderWriter.getTodo(0);

            Assert.AreEqual(todo1.Title, "testTitle");
            Assert.AreEqual(todo1.Text, "testText");
            Assert.AreEqual(todo1.ID, 0);
        }

        [TestCleanup()]
        public void cleanup() {
            System.IO.File.Delete("testDB" + ".sqlite");
        }
    }
}