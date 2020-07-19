using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using TodoManager2.model;

namespace TodoManager2 {
    class MainWindowViewModel : BindableBase{

        public Todo CreatingTodo {
            get; set;
        } = new Todo();

        private readonly string databaseName = "TodoDatabase";
        private DatabaseHelper databaseHelper;
        private TodoReaderWriter todoReaderWriter;

        public MainWindowViewModel() {
            databaseHelper = new DatabaseHelper(databaseName);
            todoReaderWriter = new TodoReaderWriter(databaseHelper);
            buildDatabase();
        }

        private void buildDatabase() {
            string textType = "TEXT";
            string todoTableName = todoReaderWriter.todoTableName;
            databaseHelper.createTable(todoTableName);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.Title),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.Text),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.IsCompleted),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.Priority),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.CreationDateTime),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.DueDateTime),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.CompletionComment),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.CompletionDateTime),textType);
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.WorkSpan),textType);

            string tagsTableName = todoReaderWriter.tagsTableName;
            databaseHelper.createTable(tagsTableName);
            databaseHelper.addNotNullColumn(tagsTableName, TagsTableColumnName.name.ToString(),textType);

            string integerType = "INTEGER";
            string tagMapsTableName = todoReaderWriter.tagMapsTableName;
            databaseHelper.createTable(tagMapsTableName);
            databaseHelper.addNotNullColumn(tagMapsTableName, TagMapsTableColumnName.tag_id.ToString(),integerType);
            databaseHelper.addNotNullColumn(tagMapsTableName, TagMapsTableColumnName.todo_id.ToString(),integerType);
        }

        private DelegateCommand addTodoCommand;
        public DelegateCommandBase AddTodoCommand {
            get {
                return addTodoCommand ?? (addTodoCommand = new DelegateCommand(
                    () => {
                        Todo todo = new Todo(DateTime.Now, 0);
                        todo.Title = CreatingTodo.Title;
                        todo.Text = CreatingTodo.Text;
                        todo.WorkSpan = CreatingTodo.WorkSpan;
                        todo.Priority = CreatingTodo.Priority;
                        todo.IsCompleted = CreatingTodo.IsCompleted;
                        todo.DueDayNumber = CreatingTodo.DueDayNumber;
                    }
                ));
            }
        }
    }
}
