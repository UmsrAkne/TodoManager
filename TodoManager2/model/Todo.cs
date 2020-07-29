using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Mvvm;

namespace TodoManager2.model {
    public class Todo : BindableBase{

        public Todo() {

        }

        public Todo(DateTime creationDate, long uniqueID) {
            CreationDateTime = creationDate;
            ID = uniqueID;
        }

        public Todo(DateTime creationDate, DateTime completionDate, long uniqueID) : this(creationDate, uniqueID) {
            CompletionDateTime = completionDate;
        }

        public DateTime CreationDateTime { private set; get; } = DateTime.Now;

        public string CreationDateString {
            get {
                return CreationDateTime.ToString("yy/MM/dd HH:mm");
            }
        }

        private string title = "";
        public string Title {
            get => title;
            set {
                RaisePropertyChanged(nameof(Title));
                title = value;
            }
        }

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

                RaisePropertyChanged();
            }
        }

        public string Text { get; set; }

        public string CompletionComment { get; set; }

        private DateTime completionDateTime = new DateTime();
        public DateTime CompletionDateTime {
            get => completionDateTime;
            private set {
                completionDateTime = value;
                RaisePropertyChanged(nameof(CompletionDateTimeString));
            }
        }

        /// <summary>
        /// 完了日時の日付を初期化します。
        /// 完了日時が初期状態ということは、未完了状態と同義です。そのため IsCompleted も false にセットされますので注意してください。
        /// </summary>
        public void ResetCompletionDateTime() {
            isCompleted = false;
            CompletionDateTime = new DateTime();
        }

        public String CompletionDateTimeString {
            get {
                return (CompletionDateTime != DateTime.MinValue) ? CompletionDateTime.ToString("MM/dd HH:mm") : "";
            }
        }

        private int dueDateTime;
        public int DueDayNumber {
            get => dueDateTime;
            set {
                DueDateTime = DateTime.Today.AddDays(1 + value) ;
                dueDateTime = value;
            }
        }

        public DateTime DueDateTime { get; set; }
        public String DueDateTimeString {
            get => DueDateTime.ToString("MM/dd");
        }

        public int WorkSpanMinutes {
            get => (int)WorkSpan.TotalMinutes;
            set {
                WorkSpan = new TimeSpan(0, value, 0);
            }
        }

        public TimeSpan WorkSpan { get; set; } = new TimeSpan();

        public List<Tag> Tags { get; set; } = new List<Tag>(new Tag[] { new Tag(""),new Tag(""),new Tag("")});

        public string Priority { get; set; } = "D";

        /// <summary>
        /// データベースに定義されているプライマリーキーのIDと同じ値です。
        /// 新規でオブジェクトを作成した場合のデフォルトは０になっています。
        /// </summary>
        public long ID { get; set; } = 0;

    }
}
