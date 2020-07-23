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

        private Todo creatingTodo = new Todo();
        public Todo CreatingTodo {
            get => creatingTodo;
            private set {
                creatingTodo = value;
                RaisePropertyChanged(nameof(CreatingTodo));
            }
        }

        private List<Todo> todoList = new List<Todo>();
        public List<Todo> TodoList {
            get => todoList;
            set {
                todoList = value;
                RaisePropertyChanged();
            }
        }

        private List<Tag> tagList = new List<Tag>();
        public List<Tag> TagList {
            get => tagList;
            set {
                tagList = value;
                RaisePropertyChanged();
            }
        }

        private readonly string databaseName = "TodoDatabase";
        private DatabaseHelper databaseHelper;
        private TodoReaderWriter todoReaderWriter;

        public MainWindowViewModel() {
            databaseHelper = new DatabaseHelper(databaseName);
            todoReaderWriter = new TodoReaderWriter(databaseHelper);
            buildDatabase();

            TodoList = todoReaderWriter.getTodosWithin(DateTime.MinValue, DateTime.Now);
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
            databaseHelper.addNotNullColumn(todoTableName, nameof(Todo.WorkSpan),"INTEGER");

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
        public DelegateCommand AddTodoCommand {
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

                        todoReaderWriter.add(todo);
                        CreatingTodo = new Todo();
                        reloadTodoListCommand.Execute();
                    },
                    () => {
                        return CreatingTodo.Title != "";
                    }
                ).ObservesProperty(() => CreatingTodo.Title));
            }
        }

        private DelegateCommand<Todo> rewriteTodoCommand;
        public DelegateCommand<Todo> RewriteTodoCommand {
            get {
                return rewriteTodoCommand ?? (rewriteTodoCommand = new DelegateCommand<Todo>(
                    (Todo paramTodo) => todoReaderWriter.update(paramTodo)));
            }
        }

        private DelegateCommand reloadTodoListCommand;
        public DelegateCommand ReloadTodoListCommand {
            get {
                return reloadTodoListCommand ?? (reloadTodoListCommand = new DelegateCommand(
                    () => TodoList = todoReaderWriter.getTodosWithin(DateTime.MinValue, DateTime.Now)));
            }
        }
    }
}
