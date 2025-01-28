using System;
using System.IO;
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

namespace LabWork12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pathLabel.Content = Directory.GetCurrentDirectory();
        }

        private void catalogButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.ShowDialog();
                pathLabel.Content = folderBrowserDialog.SelectedPath;
            }
        }

        private void findButton_Click(object sender, RoutedEventArgs e)
        {
            string partialName = particalNameLabel.Text;
            DirectoryInfo directoryInfo = new DirectoryInfo(pathLabel.Content as string);
            FileInfo[] files = directoryInfo.GetFiles("*" + partialName + "*.*");
            foreach (FileInfo foundFile in files)
            {
                string fullName = foundFile.FullName;
                //fileDataGrid.I;
            }
        }
    }
}