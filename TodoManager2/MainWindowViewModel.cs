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
