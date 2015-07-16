using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using TestFileService.FileService;


namespace FileManagerViewModel
{

    public class DeleteFileCommand : ICommand
    {
        public DeleteFileCommand()
        {
            _canExecute = true;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is bool )
                _canExecute = (bool)parameter;
            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            _canExecute = false;
            var flvm = (FolderListVM)parameter;
            if (!string.IsNullOrEmpty(flvm.SelectedFile))
            {
                
                var fs = new FileServiceClient();
                fs.DeleteFileCompleted += (sender, args) =>
                {

                    var arg = args;

                    flvm.CommandLoadFolders.Execute(flvm);
                };
                
                string what = Path.Combine(flvm.ClientRepository, flvm.SelectedFolder) + "\\" + flvm.SelectedFile;
                fs.DeleteFileAsync(what);
            }
            _canExecute = true;
        }
    }
    public class CreateFolderCommand : ICommand
    {
        public CreateFolderCommand()
        {

            _canExecute = true;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is bool )
                _canExecute = (bool)parameter;
            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            _canExecute = false;
            var flvm = (FolderListVM)parameter;
            if (!string.IsNullOrEmpty(flvm.FolderToCreate))
            {
                var fs = new FileServiceClient();
                fs.CreateFolderCompleted += (sender, args) => flvm.CommandLoadFolders.Execute(flvm);
                fs.CreateFolderAsync(Path.Combine(flvm.ClientRepository,flvm.FolderToCreate));
            }
            _canExecute = true;
        }
    }

    public class SendFileCommand : ICommand
    {
        
        public bool CanExecute(object parameter)
        {
            if (parameter is bool)
                _canExecute = (bool)parameter;
          
            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;
        //public string status { get; set; }
        public void Execute(object parameter)
        {
            _canExecute = false;
          //  status = "sending";
            byte[] b;
            var dialog = new OpenFileDialog();
            
            dialog.Multiselect = false;
            
             var flvm = (FolderListVM)parameter;
            if (string.IsNullOrEmpty(flvm.SelectedFolder))
            {
                MessageBox.Show("Please select a folder.");
                return;
            }
            
            bool? fileselected = dialog.ShowDialog();
            if (fileselected.HasValue && fileselected.Value)
            {
                var fileinfo = dialog.File;
                using (FileStream reader = fileinfo.OpenRead())
                { 
                    var fs = new FileServiceClient();
                    b = new byte[fileinfo.Length];
                    reader.Read(b, 0, (int)fileinfo.Length);
                
                    string fileName = fileinfo.Name;
                    fs.SendFileCompleted += (sender, args) =>
                    {
                        var res = (string) args.Result;
                        if (res != "success")
                        {
                            MessageBox.Show("Error uploading: " + res);
                        }
                        else
                        {
                            flvm.CommandListFiles.Execute(flvm);
                        }
                    };
                    try
                    {
                        string filepath = Path.Combine(flvm.ClientRepository, flvm.SelectedFolder) + "\\" + fileName;
                        fs.CheckFileExistsCompleted += (sender, args) =>
                        {
                            var Exists = (bool)args.Result;
                            if (!Exists)
                            {

                                fs.SendFileAsync(b, filepath);
                            }
                            else
                            {
                                var resu = MessageBox.Show("File already exists, would you like to replace?",
                                    "FileExists", MessageBoxButton.OKCancel);
                                if (resu == MessageBoxResult.OK)
                                {
                                    fs.SendFileAsync(b, filepath);
                                }
                            }
                        };
                        fs.CheckFileExistsAsync(filepath);
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    //        status = filebytes;
                }
            }

            

            _canExecute = true;
        }
    }


    public class SendFileDroppedCommand : ICommand
    {

        public bool CanExecute(object parameter)
        {
            if (parameter is bool)
                _canExecute = (bool)parameter;

            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;
        //public string status { get; set; }
        public void Execute(object parameter)
        {
            _canExecute = false;
            //  status = "sending";
            byte[] b;
            var flvm = (FolderListVM)parameter;
            if (string.IsNullOrEmpty(flvm.SelectedFolder))
            {
                MessageBox.Show("Please select a folder.");
                return;
            }
                
                using (FileStream reader = flvm.DroppedFileInfo.OpenRead())
                {
                    var fs = new FileServiceClient();
                    b = new byte[flvm.DroppedFileInfo.Length];
                    reader.Read(b, 0, (int)flvm.DroppedFileInfo.Length);

                    string fileName = flvm.DroppedFileInfo.Name;
                    fs.SendFileCompleted += (sender, args) =>
                    {
                        var res = (string)args.Result;
                        if (res != "success")
                        {
                            MessageBox.Show("Error uploading: " + res);
                        }
                        else
                        {
                            flvm.CommandListFiles.Execute(flvm);
                        }
                    };
                    fs.SendFileAsync(b, Path.Combine(flvm.ClientRepository, flvm.SelectedFolder) + "\\" + fileName);


                    //        status = filebytes;
                }
            



            _canExecute = true;
        }
    }

    public class GetFileCommand : ICommand
    {
        public GetFileCommand()
        {
            _canExecute = true;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _canExecute = false;
         
                    var flvm = (FolderListVM)parameter;
                    var fs = new FileServiceClient();
                    var dialog = new SaveFileDialog();
                    string ext =Path.GetExtension(flvm.SelectedFile);
                    dialog.Filter = "*" + ext + "|*" + ext;
                    
                    //   dialog.DefaultFileName = flvm.SelectedFile;
                    bool? filesel = dialog.ShowDialog();
                    fs.GetFileCompleted += (sender, args) =>
                    {
                        if (filesel.Value == true)
                        {
                            using (var stream = dialog.OpenFile())
                            {
                                var b = args.Result;
                                stream.Write(b, 0, b.Length);
                                stream.Close();
                            }  
                            
                        }
                    };
                    fs.GetFileAsync(Path.Combine(flvm.ClientRepository, flvm.SelectedFolder) + "\\" + flvm.SelectedFile);
                   
                
           
          

            _canExecute = true;
        }
    }

    public class ListFilesCmd : ICommand
    {
        public ListFilesCmd()
        {
            _canExecute = true;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _canExecute = false;
            var flvm = (FolderListVM) parameter;
            var fs = new FileServiceClient();
            var p = flvm.Parent;
            flvm.Filelist.Clear();
            fs.ListFilesCompleted += (sender, args) =>
            {
                var lst = args.Result;
                foreach (var l in lst)
                {
                    flvm.Filelist.Add(l);
                    flvm.FilelistStr.Add(l.FileName);
                }
                
            };
            fs.ListFilesAsync(Path.Combine(flvm.ClientRepository,flvm.SelectedFolder),flvm.FileExtension);
            
            
           
            _canExecute = true;
        }
    }
    public class LoadFolders: ICommand
    {

        public LoadFolders()
        {
            _canExecute = true;
        }
        public bool CanExecute(object parameter)
        {

            return _canExecute;
        }

      
        private bool _canExecute;
        public event EventHandler CanExecuteChanged;
        
        public void Execute(object parameter)
        {

            _canExecute = false;
            var fs = new FileServiceClient();
            var flvm = (FolderListVM)parameter;
            fs.GetFoldersCompleted += (sender, args) =>
            {
                if (parameter is FolderListVM)
                {
                    
                    if (flvm.Folders == null)
                        flvm.Folders = new ObservableCollection<FolderData>();
                    flvm.Folders.Clear();
                    foreach (var l in args.Result)
                    {
                        //var dt = new DataTemplate();
                        //dt.DataType = string;
                        string subOnly = l.Replace(flvm.ClientRepository + "\\", "");
                        flvm.Folders.Add(new FolderData() { FolderName = subOnly });
                    }

                    flvm.PropChanged("Folders");
                }
            };
            fs.GetFoldersAsync(flvm.ClientRepository);
           

            _canExecute = true;




        }
    }
}
