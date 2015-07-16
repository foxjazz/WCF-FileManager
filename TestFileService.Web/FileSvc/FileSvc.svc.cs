using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace TestFileService.Web.FileSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FileSvc : IFileSvc
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public IQueryable<string> ListFolders(string path)
        {
            var s = Directory.GetDirectories("c:\\ClientFiles");
            return s.AsQueryable();

        }
        public IQueryable<Files> List(string virtualPath)
        {
            string RepositoryDirectory = "c:\\ClientFiles";
            
            string basePath = RepositoryDirectory;

            if (!string.IsNullOrEmpty(virtualPath))
                basePath = Path.Combine(RepositoryDirectory, virtualPath);

            DirectoryInfo dirInfo = new DirectoryInfo(basePath);
            FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

            var vv = (from f in files
                      select new StorageFileInfo()
                      {
                          Size = f.Length,
                          VirtualPath = f.FullName.Substring(f.FullName.IndexOf(RepositoryDirectory) + RepositoryDirectory.Length + 1)
                      }).ToArray();


            var lff = new List<Files>();
            foreach (var info in vv)
            {
                var ff = new Files();
                ff.FileName = info.VirtualPath;
                ff.FileSize = info.Size;
                lff.Add(ff);
            }
            return lff.AsQueryable();
        }

        private class StorageFileInfo
        {
            /// <summary>
            /// Gets or sets the virtual path to the file
            /// </summary>
            public string VirtualPath { get; set; }

            /// <summary>
            /// Gets or sets the size of the file (in bytes)
            /// </summary>
            public long Size { get; set; }

        }
    }
}
