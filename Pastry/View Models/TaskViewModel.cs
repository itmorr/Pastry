using System.Collections.ObjectModel;
using Pastry.Models;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Data;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pastry.ViewModels
{
    public class ListViewItemViewModel
    {
        public string description { get; set; }
        public int hours { get; set; }
        public DeleteCommand deleteCommand { get; set; }
        public CompleteCommand completeCommand { get; set; }
        private TaskViewModel parent;
        public Task task { get; set; } // The task that this view model gets data from

        public ListViewItemViewModel(TaskViewModel parent, Task task)
        {
            this.task = task;
            this.parent = parent;
            this.deleteCommand = new DeleteCommand(parent, this);
            this.completeCommand = new CompleteCommand(parent, this);
        }

        public class DeleteCommand : ICommand
        {
            TaskViewModel parent;
            ListViewItemViewModel item;

            protected internal DeleteCommand(TaskViewModel parent, ListViewItemViewModel item)
            {
                this.parent = parent;
                this.item = item;
            }

            public void Execute(object parameter)
            {
                parent.deleteListViewItem(item);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }

        public class CompleteCommand : ICommand
        {
            TaskViewModel parent;
            ListViewItemViewModel item;

            protected internal CompleteCommand(TaskViewModel parent, ListViewItemViewModel item)
            {
                this.parent = parent;
                this.item = item;
            }

            public void Execute(object parameter)
            {
                parent.completeListViewItem(item);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

        }

    }

    public class TaskViewModel : INotifyPropertyChanged
    {

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private double progress;
        private string progressPercent;
        private string progressPercentForeground;
        private int maxProgress;
        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (value != progress || value == 0)
                {
                    progress = value;
                    ProgressPercent = Math.Floor((((double)value/(double)MaxProgress)*100)) + "%";
                    if (value < (0.15) * maxProgress)
                    {
                        ProgressPercentForeground = "Red";
                    }
                    else if (value < (0.60) * maxProgress)
                    {
                        ProgressPercentForeground = "WhiteSmoke";
                    }
                    else
                    {
                        ProgressPercentForeground = "Green";
                    }
                    OnPropertyChanged(new PropertyChangedEventArgs("Progress"));
                }
            }
        }
        public int MaxProgress
        {
            get
            {
                return maxProgress;
            }
            set
            {
                if (value != maxProgress)
                {
                    maxProgress = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MaxProgress"));
                }
            }
        }
        public string ProgressPercent
        {
            get
            {
                return progressPercent;
            }
            set
            {
                if (value != progressPercent)
                {
                    progressPercent = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgressPercent"));
                }
            }
        }
        public string ProgressPercentForeground
        {
            get
            {
                return progressPercentForeground;
            }
            set
            {
                if (value != progressPercentForeground)
                {
                    progressPercentForeground = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgressPercentForeground"));
                }
            }
        }

        public ObservableCollection<ListViewItemViewModel> billableTasks{ get; set; }
        public ObservableCollection<ListViewItemViewModel> nonBillableTasks { get; set; }
        public ObservableCollection<ListViewItemViewModel> actualBillableTasksToday { get; set; }
        private string saveFile = "data.dat";

        public TaskViewModel()
        {
            billableTasks = new ObservableCollection<ListViewItemViewModel>();
            nonBillableTasks = new ObservableCollection<ListViewItemViewModel>();
            actualBillableTasksToday = new ObservableCollection<ListViewItemViewModel>();

            progressPercentForeground = "Red";
            maxProgress = 8;
            progress = 0;
        }

        public void loadTasks()
        {
            loadData();

            //loadBillableTasks();

            computeProgress();
        }

        public void loadBillableTasks()
        {
            billableTasks.Add(new ListViewItemViewModel(this, new Task("billable task", 5)));
            billableTasks.Add(new ListViewItemViewModel(this, new Task("billable task", 10)));
        }

        public void loadNonBillableTasks()
        {
            nonBillableTasks.Add(new ListViewItemViewModel(this, new Task("nonbillable task", 1)));
        }

        public void loadActualBillableTasksToday()
        {
            actualBillableTasksToday.Add(new ListViewItemViewModel(this, new Task("actual task", 2)));
        }

        public void saveData()
        {
            Stream stream = File.Open(saveFile, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            ObservableCollection<ObservableCollection<Task>> toWrite = new ObservableCollection<ObservableCollection<Task>>();
            ObservableCollection<Task> billableTasksToWrite = new ObservableCollection<Task>();
            ObservableCollection<Task> nonBillableTasksToWrite = new ObservableCollection<Task>();
            ObservableCollection<Task> actualBillableTasksTodayToWrite = new ObservableCollection<Task>();
            foreach (ListViewItemViewModel billableTask in billableTasks)
            {
                billableTasksToWrite.Add(billableTask.task);
            }
            foreach (ListViewItemViewModel nonBillableTask in nonBillableTasks)
            {
                nonBillableTasksToWrite.Add(nonBillableTask.task);
            }
            foreach (ListViewItemViewModel actualBillableTaskToday in actualBillableTasksToday)
            {
                actualBillableTasksTodayToWrite.Add(actualBillableTaskToday.task);
            }
            toWrite.Add(billableTasksToWrite);
            toWrite.Add(nonBillableTasksToWrite);
            toWrite.Add(actualBillableTasksTodayToWrite);
            bFormatter.Serialize(stream, toWrite);
            stream.Close();
        }

        public void loadData()
        {
            Stream stream;
            try
            {
                stream = File.Open(saveFile, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                return;
            }

            if (stream.Length == 0) return; // Special case maybe

            BinaryFormatter bFormatter = new BinaryFormatter();

            ObservableCollection<ObservableCollection<Task>> fromWrite = new ObservableCollection<ObservableCollection<Task>>();
            ObservableCollection<Task> billableTasksFromWrite = new ObservableCollection<Task>();
            ObservableCollection<Task> nonBillableTasksFromWrite = new ObservableCollection<Task>();
            ObservableCollection<Task> actualBillableTasksTodayFromWrite = new ObservableCollection<Task>();

            fromWrite = (ObservableCollection<ObservableCollection<Task>>)bFormatter.Deserialize(stream);
            billableTasksFromWrite = fromWrite[0];
            nonBillableTasksFromWrite = fromWrite[1];
            actualBillableTasksTodayFromWrite = fromWrite[2];
            foreach (Task billableTask in billableTasksFromWrite)
            {
                billableTasks.Add(new ListViewItemViewModel(this, billableTask));
            }
            foreach (Task nonBillableTask in nonBillableTasksFromWrite)
            {
                nonBillableTasks.Add(new ListViewItemViewModel(this, nonBillableTask));
            }
            foreach (Task actualBillableTaskToday in actualBillableTasksTodayFromWrite)
            {
                actualBillableTasksToday.Add(new ListViewItemViewModel(this, actualBillableTaskToday));
            }
            stream.Close();
        }

        public void deleteListViewItem(ListViewItemViewModel item)
        {
            if (billableTasks.Contains(item)) billableTasks.Remove(item);
            if (nonBillableTasks.Contains(item)) nonBillableTasks.Remove(item);
            if (actualBillableTasksToday.Contains(item)) actualBillableTasksToday.Remove(item);
            computeProgress();
        }

        public void completeListViewItem(ListViewItemViewModel item)
        {
            if (billableTasks.Contains(item))
            {
                billableTasks.Remove(item);
                actualBillableTasksToday.Add(item);
            }
            if (nonBillableTasks.Contains(item)) nonBillableTasks.Remove(item);

            computeProgress();
        }

        public void computeProgress()
        {
            double expectedHours = 0;
            // Recompute progress.
            if (billableTasks != null)
            {
                foreach (ListViewItemViewModel billableItem in billableTasks)
                {
                    expectedHours += billableItem.task.hours;
                }
                foreach (ListViewItemViewModel actualBillableItem in actualBillableTasksToday)
                {
                    expectedHours += actualBillableItem.task.hours;
                }
            }
            Progress = expectedHours;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            saveData();
            PropertyChanged.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void addBillableTask(Task task)
        {
            billableTasks.Add(new ListViewItemViewModel(this, task));
            computeProgress();
        }

        public void addNonBillableTask(Task task)
        {
            nonBillableTasks.Add(new ListViewItemViewModel(this, task));
            computeProgress();
        }
    }
}