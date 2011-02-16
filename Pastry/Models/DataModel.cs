using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Input;
using System.Collections;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Pastry.Models
{
    public class DataModel : INotifyPropertyChanged
    {
        const string localDatabasePath = @"";
        const string localDatabaseName = "Database.sdf";
        const string localDatabasePass = "";
        const string localDatabaseTable = "tasks";
        ArrayList tuples = new ArrayList();

        public DataModel()
        {
        }

        public class TaskTuple
        {
            public string description { get; set; }
            public int hours { get; set; }
            public int type { get; set; }

            public TaskTuple(string description, int hours, int type)
            {
                this.description = description;
                this.hours = hours;
                this.type = type;
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}