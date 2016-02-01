using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentInspector.ViewModels;
using MahApps.Metro.Controls;

namespace DocumentInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowViewModel();
            DataContext = _vm;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _vm.Closing();
        }
    }
}
