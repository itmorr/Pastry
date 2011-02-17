using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pastry.Models
{
    [Serializable()]
    public class Task : INotifyPropertyChanged, ISerializable
    {
        public string description { get; set; }
        public double hours { get; set; }

        public Task()
        {
        }

        public Task(string description, double hours)
        {
            this.description = description;
            this.hours = hours;
        }

        public Task(SerializationInfo info, StreamingContext ctxt)
        {
            this.description = (string)info.GetValue("description", typeof(string));
            this.hours = (double)info.GetValue("hours", typeof(double));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("description", description);
            info.AddValue("hours", hours);
        }

        #endregion
    }
}