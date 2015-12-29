using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.DynamicData;
using FilesService.Properties;

namespace FilesService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
   //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
   //      ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)] 
    public class FileService : IFileService
    {
        public FileService()
        {
            RepositoryDirectory = Settings.Default.RepositoryLocation;
        }
       

        //public UploadResponse Upload(UploadRequest request)
        //{
        //    try
        //    {

        //        // Try to create the upload directory if it does not yet exist
               
     
        //        // Check if a file with the same filename is already 
        //        // present in the upload directory. If this is the case 
        //        // then delete this file
        //        string path = Path.Combine(RepositoryDirectory, request.FileName);
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //        }
   
        //      // Read the incoming stream and save it to file,8192
        //        //const int bufferSize = 4096;
        //      const int bufferSize = 2048;
        //      byte[] buffer = new byte[bufferSize];
        //      using (FileStream outputStream = new FileStream(path,
        //            FileMode.Create, FileAccess.Write))
        //        {
        //            int bytesRead = request.Stream.Read(buffer, 0, bufferSize);
        //            while (bytesRead > 0)
        //            {
        //                outputStream.Write(buffer, 0, bytesRead);
        //                bytesRead = request.Stream.Read(buffer, 0, bufferSize);
        //            }
        //            outputStream.Close();
        //        }
        //        return new UploadResponse
        //                   {
        //                       UploadSucceeded = true
        //                   };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new UploadResponse
        //                   {
        //                       UploadSucceeded = false
        //                   };
        //    }
        //}

        /// <summary>
        /// Gets or sets the repository directory.
        /// </summary>
        public string RepositoryDirectory { get; set; }

        public string DeleteFolder(string folder)
        {
            try
            {
                IEnumerable<string> items = Directory.EnumerateFileSystemEntries(Path.Combine(RepositoryDirectory, folder));
                if (items.Any())
                    return "directory not empty";
                Directory.Delete(Path.Combine(RepositoryDirectory, folder));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }
        public string RenameFolder(string SourceFolder,string DestinationFolder)
        {
            try
            {
                Directory.Move(Path.Combine(RepositoryDirectory, SourceFolder),Path.Combine(RepositoryDirectory, DestinationFolder));

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }
        public string CreateFolder(string folder)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(RepositoryDirectory, folder));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "success";
        }

        public bool CheckFileExists(string filename)
        {
            string filePath = Path.Combine(RepositoryDirectory, filename);
            if (File.Exists(filePath))
                {
                    return true;
                }
            else
                {
                    return false;
                }
               

            
        }
        public string SendFile(byte[] b, string virtualPath)
        {
            try
            {
                string filePath = Path.Combine(RepositoryDirectory, virtualPath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                fs.Write(b,0,b.Length);
                fs.Flush();
                fs.Close();


            }
            catch (Exception ex)
            {
                return "error: " +  ex.Message;
            }
            return "success";
        }

        public string UploadFile(Stream stream, string virtualPath)
        {
            try
            {
                string filePath = Path.Combine(RepositoryDirectory, virtualPath);
                byte[] bytearray = new byte[stream.Length];
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                int bytesRead = stream.Read(bytearray, 0, bytearray.Length);

                fs.Write(bytearray, 0, (int)stream.Length);


            }
            catch (Exception ex)
            {
                return "error: " + ex.Message;
            }
            return "success";
        }
        public byte[] GetFile(string virtualPath)
        {
            string filePath = Path.Combine(RepositoryDirectory, virtualPath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));

            //SendFileRequested(virtualPath);
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var b = new byte[fs.Length];
            fs.Read(b, 0, b.Length);
            return b;
        }
        public Stream GetFileStream(string virtualPath)
        {
            string filePath = Path.Combine(RepositoryDirectory, virtualPath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return fs;
        }
        ///// <summary>
        ///// Uploads a file into the repository
        ///// </summary>
        //public void PutFile(FileUploadMessage msg)
        //{
        //    string filePath = Path.Combine(RepositoryDirectory, msg.VirtualPath);
        //    string dir = Path.GetDirectoryName(filePath);

        //    if (!Directory.Exists(dir))
        //        Directory.CreateDirectory(dir);

        //    using (var outputStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        msg.DataStream.CopyTo(outputStream);
        //    }

        //    SendFileUploaded(filePath);
        //}

        /// <summary>
        /// Deletes a file from the repository
        /// </summary>
        public string DeleteFile(string virtualPath)
        {
            string path="";
            try
            {
                string filePath = Path.Combine(RepositoryDirectory, virtualPath);
                path = filePath;
                if (File.Exists(filePath))
                {
                    // SendFileDeleted(virtualPath);
                    File.Delete(filePath);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "Delete failed: " + ex.Message + " path=" + path;
            }
        }

        /// <summary>
        /// Lists files from the repository at the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path. This can be null to list files from the root of
        /// the repository.</param>
        public IQueryable<Files> List(string virtualPath)
        {
            try
            {
                string basePath = RepositoryDirectory;

                if (!string.IsNullOrEmpty(virtualPath))
                    basePath = Path.Combine(RepositoryDirectory, virtualPath);

                DirectoryInfo dirInfo = new DirectoryInfo(basePath);
                FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);

                var vv = (from f in files
                    select new StorageFileInfo()
                    {
                        Size = f.Length,
                        VirtualPath =
                            f.FullName.Substring(f.FullName.IndexOf(RepositoryDirectory) + RepositoryDirectory.Length +
                                                 1),
                        FileName = f.Name
                    }).ToArray();


                var lff = new List<Files>();
                foreach (var info in vv)
                {
                    var ff = new Files();
                    ff.FileName = info.FileName;
                    ff.FileSize = info.Size;
                    lff.Add(ff);
                }
                return lff.AsQueryable();
            }
            catch (Exception ex)
            {
                var lff = new List<Files>();
                var ff = new Files();
                ff.FileName = ex.Message;
                lff.Add(ff);
                return lff.AsQueryable();
            }
        }
        public IQueryable<Files> ListFiles(string virtualPath,string ext)
        {
            try
            {
                string basePath = RepositoryDirectory;
                if (string.IsNullOrEmpty(ext))
                    ext = "*";
                if (!string.IsNullOrEmpty(virtualPath))
                    basePath = Path.Combine(RepositoryDirectory, virtualPath);

                DirectoryInfo dirInfo = new DirectoryInfo(basePath);
                
                FileInfo[] files = dirInfo.GetFiles("*." + ext, SearchOption.TopDirectoryOnly);
                var vv = (from f in files

                          select new StorageFileInfo()
                          {
                             
                              Size = f.Length,
                              VirtualPath =
                                  f.FullName.Substring(f.FullName.IndexOf(RepositoryDirectory) + RepositoryDirectory.Length +
                                                       1),
                              FileName = f.Name,
                              FileDateCreated = f.CreationTime
                              

                          }).ToArray();


                var lff = new List<Files>();
                foreach (var info in vv)
                {
                    var ff = new Files();
                    ff.FileName = info.FileName;
                    ff.FileSize = info.Size;
                    ff.FileDateCreated = info.FileDateCreated;
                    lff.Add(ff);
                }
                return lff.AsQueryable();
            }
            catch (Exception ex)
            {
                var lff = new List<Files>();
                var ff = new Files();
                ff.FileName = ex.Message;
                lff.Add(ff);
                return lff.AsQueryable();
            }
        }
       


        #region Events

        /// <summary>
        /// Raises the FileRequested event.
        /// </summary>
        //protected void SendFileRequested(string vPath)
        //{
        //    if (FileRequested != null)
        //        FileRequested(this, new FileEventArgs(vPath));
        //}

        /// <summary>
        /// Raises the FileUploaded event
        /// </summary>
        //protected void SendFileUploaded(string vPath)
        //{
        //    if (FileUploaded != null)
        //        FileUploaded(this, new FileEventArgs(vPath));
        //}

        ///// <summary>
        ///// Raises the FileDeleted event.
        ///// </summary>
        //protected void SendFileDeleted(string vPath)
        //{
        //    if (FileDeleted != null)
        //        FileDeleted(this, new FileEventArgs(vPath));
        //}

        #endregion

        public IQueryable<string> GetFolders(string subLocation)
        {
            try
            {
                if (!Directory.Exists(RepositoryDirectory))
                    Directory.CreateDirectory(RepositoryDirectory);

                if(!Directory.Exists(Path.Combine(RepositoryDirectory,subLocation)))
                    Directory.CreateDirectory(Path.Combine(RepositoryDirectory, subLocation));

                var v = Directory.GetDirectories(Path.Combine(RepositoryDirectory, subLocation));
                string dataReturn = "";
                var folders = v.Select(s => s.Replace(RepositoryDirectory + "\\", "")).ToList();

                return folders.AsQueryable();
            }
            catch (Exception ex)
            {
                var folders = new List<string>();
                folders.Add(ex.Message);
                return folders.AsQueryable();
            }
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
    }
}
