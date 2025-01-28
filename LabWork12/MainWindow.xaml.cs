using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FileSearchApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FolderLabel.Content = $"Текущая папка: {Directory.GetCurrentDirectory()}";
        }
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    FolderLabel.Content = $"Текущая папка: {dialog.SelectedPath}";
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilesListBox.Items.Clear();
            string folderPath = FolderLabel.Content.ToString().Replace("Текущая папка: ", "");
            string searchPattern = FileNamePartTextBox.Text;

            if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(searchPattern))
            {
                MessageBox.Show("Укажите каталог и часть имени файла.");
                return;
            }

            bool searchInSubfolders = AllSubfoldersRadioButton.IsChecked == true;

            long? minSize = null, maxSize = null;
            if (ConsiderSizeCheckBox.IsChecked == true)
            {
                if (long.TryParse(MinSizeTextBox.Text, out long min) && min > 0)
                    minSize = min * 1024;

                if (long.TryParse(MaxSizeTextBox.Text, out long max) && max > 0)
                    maxSize = max * 1024;
            }

            DateTime? creationDate = null;
            if (ConsiderDateCheckBox.IsChecked == true)
            {
                creationDate = CreationDatePicker.SelectedDate;
            }

            FilesListBox.Items.Clear();

            var files = FindFiles(folderPath, searchPattern, searchInSubfolders, minSize, maxSize, creationDate);

            foreach (var file in files)
            {
                FilesListBox.Items.Add(file);
            }
        }

        private IEnumerable<string> FindFiles(string folderPath, string searchPattern, bool searchInSubfolders,
                                              long? minSize, long? maxSize, DateTime? creationDate)
        {
            var searchOption = searchInSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.EnumerateFiles(folderPath, $"*{searchPattern}*", searchOption);

            if (minSize.HasValue || maxSize.HasValue)
            {
                files = files.Where(f =>
                {
                    var fileInfo = new FileInfo(f);
                    bool sizeValid = (!minSize.HasValue || fileInfo.Length >= minSize.Value) &&
                                     (!maxSize.HasValue || fileInfo.Length <= maxSize.Value);
                    return sizeValid;
                });
            }

            if (creationDate.HasValue)
            {
                files = files.Where(f =>
                {
                    var fileInfo = new FileInfo(f);
                    return fileInfo.CreationTime >= creationDate.Value;
                });
            }

            return files;
        }

        private void SizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MinSizeTextBox.IsEnabled = MaxSizeTextBox.IsEnabled = true;
        }

        private void SizeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MinSizeTextBox.IsEnabled = MaxSizeTextBox.IsEnabled = false;
        }

        private void DateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CreationDatePicker.IsEnabled = true;
        }

        private void DateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CreationDatePicker.IsEnabled = false;
        }
    }
}
