using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestFileService;
using TestFileService.FileService;


namespace FileManagerViewModel
{
    public class FolderListVM : INotifyPropertyChanged
    {

        

        public FolderListVM(SecurityModeEnum smode)
        {
	    // Initialize.
         
            this.Folder = new ObservableCollection<string>();
            this.Filelist = new ObservableCollection<Files>();
            this.FilelistStr = new ObservableCollection<string>();
            FoldersDt = new DataTemplate();
            SecurityMode = smode;
            //_commandLoadFolders = new LoadFolders();
            //_commandListFiles = new ListFiles();
        }
        private DataTemplate FoldersDt { get; set; }
        public virtual string ClientRepository { get; set; }
        /// <summary>
        /// Event raised when a property changes.
        /// </summary>
        /// 
        /// 
        /// 
        public object check { get; set; }
        public FolderListVM This { get { return this; }  }
        public object Parent { get; set; }      
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _selectedFolder;

        public string SelectedFolder
        {
            get { return _selectedFolder; }
            set { if (_selectedFolder == value) return;
                _selectedFolder = value;
                this.OnPropertyChanged("SelectedFolder");
            }

        }
        private string _selectedFile;

        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (_selectedFile == value) return;
                _selectedFile = value;
                this.OnPropertyChanged("SelectedFile");
            }

        }

        private string _fileExtension;
        public string FileExtension
        {
            get { return _fileExtension; }
            set
            {
                _fileExtension = value;
                this.OnPropertyChanged("FileExtension");
            }
        }

        private string _folderToCreate;
        public string FolderToCreate
        {
            get { return _folderToCreate; }
            set
            {
                _folderToCreate = value;
                this.OnPropertyChanged("FolderToCreate");
            }
        }
        
        public enum SecurityModeEnum
        {
            ReadOnly,
            ReadWrite
        }

        private SecurityModeEnum _securityMode;
        public SecurityModeEnum SecurityMode
        {
            get { return _securityMode; }
            set
            {
                _securityMode = value;
                CommandDeleteFile.CanExecute(GetSecurity(_securityMode));
                CommandSendFile.CanExecute(GetSecurity(_securityMode));
                CommandCreateFolder.CanExecute(GetSecurity(_securityMode));
                
                this.OnPropertyChanged("SecurityMode");
                this.OnPropertyChanged("CommandSendFile");
            }
            
        }
        private ObservableCollection<string> _fileListStr;
        public ObservableCollection<string> FilelistStr
        {
            get { return _fileListStr; }
            private set
            {
                if (_fileListStr == value)
                {
                    return;
                }

                _fileListStr = value;
                this.OnPropertyChanged("FilelistStr");
            }
        }
        private ObservableCollection<Files> _fileList;
        public ObservableCollection<Files> Filelist
        {
            get { return _fileList; }
            private set
            {
                if (_fileList == value)
                {
                    return;
                }

                _fileList = value;
                this.OnPropertyChanged("Filelist");
            }
        }
        public FileInfo DroppedFileInfo { get; set; }
        private ObservableCollection<string> _Folder;
        public ObservableCollection<string> Folder
        {
            get { return _Folder; }
            private set
            {
                if (Folder == value)
                {
                    return;
                }

                _Folder = value;
                this.OnPropertyChanged("Folder");
            }
        }

        private bool GetSecurity(SecurityModeEnum sm)
        {
            if (sm == SecurityModeEnum.ReadOnly)
                return false;
            else
            {
                return true;
            }
        }
        private ObservableCollection<FolderData> _folders;
        public ObservableCollection<FolderData> Folders
        {
            get { return _folders; }
            set
            {

                _folders = value;
                this.OnPropertyChanged("Folders");
            }
        }


        private ICommand _commandLoadFolders;
        public ICommand CommandLoadFolders
        {
            get
            {
                if (_commandLoadFolders == null)
                    _commandLoadFolders = new LoadFolders();
                
                return _commandLoadFolders;
            }
            set
            {
                _commandLoadFolders = value;
            }
        }
        private ICommand _commandListFiles;
        public ICommand CommandListFiles
        {
            get
            {
                if (_commandListFiles == null)
                    _commandListFiles = new ListFilesCmd();

                return _commandListFiles;
            }
            set
            {
                _commandListFiles = value;
            }
        }

        private ICommand _commandGetFile;
        public ICommand CommandGetFile
        {
            get
            {
                if (_commandGetFile == null)
                    _commandGetFile = new GetFileCommand();

                return _commandGetFile;
            }
            set
            {
                _commandGetFile = value;
            }
        }
        private ICommand _commandSendFile;
        public ICommand CommandSendFile
        {
            get
            {
                if (_commandSendFile == null)
                    _commandSendFile = new SendFileCommand();
                _commandSendFile.CanExecute(GetSecurity(this.SecurityMode));
                return _commandSendFile;
            }
            set
            {
                _commandSendFile = value;
            }
        }
        private ICommand _commandSendFileDropped;
        public ICommand CommandSendFileDropped
        {
            get
            {
                if (_commandSendFileDropped == null)
                    _commandSendFileDropped = new SendFileDroppedCommand();
                _commandSendFileDropped.CanExecute(GetSecurity(this.SecurityMode));
                return _commandSendFile;
            }
            set
            {
                _commandSendFileDropped = value;
            }
        }

        private ICommand _commandCreateFolder;
        public ICommand CommandCreateFolder
        {
            get
            {
                if (_commandCreateFolder == null)
                    _commandCreateFolder = new CreateFolderCommand();
                _commandCreateFolder.CanExecute(GetSecurity(this.SecurityMode));
                return _commandCreateFolder;
            }
            set
            {
                _commandCreateFolder = value;
            }
        }
        private ICommand _commandDeleteFile;
        public ICommand CommandDeleteFile
        {
            get
            {
                if (_commandDeleteFile == null)
                    _commandDeleteFile = new DeleteFileCommand();
                _commandDeleteFile.CanExecute(GetSecurity(this.SecurityMode));
                return _commandDeleteFile;
            }
            set
            {
                _commandDeleteFile = value;
            }
        }


        internal void PropChanged(string p)
        {
            this.OnPropertyChanged(p);
        }
    }
}
