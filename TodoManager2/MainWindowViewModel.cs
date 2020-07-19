using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using TodoManager2.model;

namespace TodoManager2 {
    class MainWindowViewModel : BindableBase{

        public Todo CreatingTodo {
            get; set;
        }
    }
}
