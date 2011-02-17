using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pastry.ViewModels;
using Pastry.Models;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using System.Windows.Markup;

namespace Pastry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskViewModel taskViewModel;

        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Clipboard.SetDataObject(DumpControlTemplate(this.lvActualBillableToday), true);

            NotifyIcon ni = new NotifyIcon();

            ni.Icon = (Icon)Pastry.Resources.Goomba;
            ni.Visible = true;
            ni.MouseClick += new System.Windows.Forms.MouseEventHandler(ni_MouseClick);

            this.Top = SystemParameters.WorkArea.Height - this.Height;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
        }

        public string DumpControlTemplate(System.Windows.Controls.Control ctrl)  
        {  
            XmlWriterSettings settings = new XmlWriterSettings()  
            {  
                Indent = true,  
                NewLineOnAttributes = true  
            };  
   
            StringBuilder strbuild = new StringBuilder();  
            XmlWriter xmlwrite = XmlWriter.Create(strbuild, settings);  
            XamlWriter.Save(ctrl.Template, xmlwrite);  
            return strbuild.ToString();  
        }  

        void ni_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
                this.Visibility = Visibility.Hidden;
            else
                this.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskViewModel = new TaskViewModel();
            lvBillable.DataContext = taskViewModel.billableTasks;
            lvNonBillable.DataContext = taskViewModel.nonBillableTasks;
            lvActualBillableToday.DataContext = taskViewModel.actualBillableTasksToday;
            pbUtilization.DataContext = taskViewModel;
            lblUtilization.DataContext = taskViewModel;
            taskViewModel.loadTasks();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (rbBillable.IsChecked == true)
            {
                taskViewModel.addBillableTask(new Task(txtDescription.Text, double.Parse(txtHours.Text)));
            }
            else
            {
                taskViewModel.addNonBillableTask(new Task(txtDescription.Text, double.Parse(txtHours.Text)));
            }
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
    }
}
