using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TodoManager2.model {
    public class Todo {

        public Todo() {

        }

        public Todo(DateTime creationDate, long uniqueID) {
            CreationDateTime = creationDate;
        }

        public Todo(DateTime creationDate, DateTime completionDate, long uniqueID) : this(creationDate, uniqueID) {
            CompletionDateTime = completionDate;
        }

        public DateTime CreationDateTime { private set; get; } = DateTime.Now;

        public string CreationDateString {
            get {
                return CreationDateTime.ToString();
            }
        }

        public string Title { get; set; }

        private bool isCompleted = false;
        public bool IsCompleted {
            get {
                return isCompleted;
            }
            set {
                isCompleted = value;
                if (isCompleted) {
                    if (CompletionDateTime == DateTime.MinValue) {
                        CompletionDateTime = DateTime.Now;
                    }
                }
            }
        }

        public string Text { get; set; }

        public string CompletionComment { get; set; }

        public DateTime CompletionDateTime { get; private set; } = new DateTime();

        public DateTime DueDateTime { get; set; } = new DateTime();

        public TimeSpan Span { get; set; } = new TimeSpan();

        public List<String> Tags { get; set; } = new List<string>();

        public string Priority { get; set; } = "D";

        /// <summary>
        /// データベースに定義されているプライマリーキーのIDと同じ値です。
        /// 新規でオブジェクトを作成した場合のデフォルトは０になっています。
        /// </summary>
        public long ID { get; set; } = 0;

    }
}
