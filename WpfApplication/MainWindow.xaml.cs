using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ConsoleApplication;
using Microsoft.Win32;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseGeneratePathButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt| All files (*.*)|*.*";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() == true)
            {
                generatePathTextBox.Text = dialog.FileName; 
            }
        }

        private void ChooseSortPathButton_OnClick(object sender, RoutedEventArgs e)
        {            
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt| All files (*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == true)
            {
                sortPathTextBox.Text = dialog.FileName; 
            }
        }

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var canGenerate = true;

            if (canGenerate)
            {
                canGenerate = Path.IsPathRooted(generatePathTextBox.Text);
            }

            if (canGenerate)
            {
                try
                {
                    using (File.Create(generatePathTextBox.Text)){ }
                    File.Delete(generatePathTextBox.Text);
                }
                catch
                {
                    canGenerate = false;
                }
            }

            if (canGenerate)
            {
                try
                {
                    if ( int.Parse(lineLengthTextBox.Text) < 0 ||
                        long.Parse(fileSizeTextBox.Text) < 0)
                    {
                        canGenerate = false;
                    }
                    else
                    {
                        if (!seedTextBox.Text.Equals(""))
                        {
                            int.Parse(seedTextBox.Text);
                        }
                    }
                }
                catch
                {
                    canGenerate = false;
                }
            }

            if (canGenerate == false)
            {
                MessageBox.Show("Can't generate file: " + "\"" + generatePathTextBox.Text + "\""
                    , "Error"
                    ,MessageBoxButton.OK
                    ,MessageBoxImage.Error
                    ,MessageBoxResult.OK
                    ,MessageBoxOptions.None
                    );
            }
            else
            {
                
                var fileFullName = generatePathTextBox.Text;
                var lineLength = int.Parse(lineLengthTextBox.Text);
                var fileSize = long.Parse(fileSizeTextBox.Text);
                
                var seed = seedTextBox.Text.Equals("") ? 
                    new Random().Next() 
                    : 
                    int.Parse(seedTextBox.Text);
                
                FileGenerator generator = new TextGenerator(seed, lineLength, fileSize);
                var start = DateTime.Now;
                generator.GenerateFile(fileFullName);
                showDone(DateTime.Now-start);
            }
        }

        private void showDone(TimeSpan timeSpan)
        {
            MessageBox.Show("Operation is done! It took: " + timeSpan
                , "Done"
                ,MessageBoxButton.OK
                ,MessageBoxImage.Information
                ,MessageBoxResult.OK
                ,MessageBoxOptions.None
            );
        }
        private void SortButton_OnClick(object sender, RoutedEventArgs e)
        {
            var canSort = true;
            if (canSort)
            {
                canSort = Path.IsPathRooted(sortPathTextBox.Text);
            }

            if (canSort)
            {
                try
                {
                    using (File.OpenWrite(sortPathTextBox.Text)){ }
                }
                catch
                {
                    canSort = false;
                }
            }
            
            if (canSort)
            {
                try
                {
                    canSort = int.Parse(bucketSizeTextBox.Text) > 0;
                }
                catch
                {
                    canSort = false;
                }
            }
            
            if (canSort == false)
            {
                MessageBox.Show("Can't sort file: " + "\"" + sortPathTextBox.Text + "\""
                    , "Error"
                    ,MessageBoxButton.OK
                    ,MessageBoxImage.Error
                    ,MessageBoxResult.OK
                    ,MessageBoxOptions.None
                );
            }
            else
            {
             
                
                var bucketSize = long.Parse(bucketSizeTextBox.Text);
                
                FileSorter sorter = new MergeSorter(bucketSize);
                var start = DateTime.Now;
                sorter.sort(sortPathTextBox.Text);
                showDone(DateTime.Now-start);
                
            }
        }
    }
}