using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FileManagerViewModel;
using TestFileService.FileService;

namespace TestFileService
{
    public partial class FileManagerUC : UserControl
    {
        /**
         * When implementing the usercontrol... Get the ReadWrite or ReadOnly mode from the silverlight common roles location.
         * 
        */
        public FileManagerUC()
        {
        
            InitializeComponent();
            DataContext = flVM = new FolderListVM(FolderListVM.SecurityModeEnum.ReadWrite);
            flVM.ClientRepository = "TestRepo";

            
          
            flVM.CommandLoadFolders.Execute(flVM);


        }

        void FileManagerUC_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (DesignerProperties.IsInDesignModeProperty != null)
            {
            
                
               
            }

        }
        public FolderListVM flVM { get; set; }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //flVM.PropChanged("Folders");
            //foreach (var s in flVM.Folders)
            //{
            //    flb2.Items.Add(s);
            //    FolderListBox.Items.Add(s.FolderName);
            //}
        }

        private void FolderListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var dtf = (FolderData)FolderListBox.SelectedItem;
            if (dtf != null && !string.IsNullOrEmpty(dtf.FolderName))
                flVM.SelectedFolder = dtf.FolderName;

          
            flVM.CommandListFiles.Execute(flVM);
        }

        private void flb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var f = (Files)flb2.SelectedValue;

            if (f != null)
                flVM.SelectedFile = f.FileName;

        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Extension bound
            if (e.Key == Key.Enter)
            {
                flVM.FileExtension = tbExt.Text;
                flVM.CommandListFiles.Execute(flVM);
            }
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if ((e.Data != null) &&
                (e.Data.GetDataPresent(DataFormats.FileDrop)))
            {
                var files = (FileInfo[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    flVM.DroppedFileInfo = files[0];
                    flVM.CommandSendFileDropped.Execute(flVM);
                    
                }
            }
        }

        private void flb2_Drop(object sender, DragEventArgs e)
        {
            if ((e.Data != null) &&
              (e.Data.GetDataPresent(DataFormats.FileDrop)))
            {
                var files = (FileInfo[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1)
                {
                    flVM.DroppedFileInfo = files[0];
                    flVM.CommandSendFileDropped.Execute(flVM);

                }
            }
        }
    }
}
