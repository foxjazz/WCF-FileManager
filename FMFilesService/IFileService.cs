using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FilesService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        byte[] GetFile(string virtualPath);
        [OperationContract]
        Stream GetFileStream(string virtualPath);
        [OperationContract]
        string SendFile(byte[] b, string Path);
        
       
        //[OperationContract]
        //UploadResponse Upload(UploadRequest uploadRequest);
         [OperationContract]
        bool CheckFileExists( string Path);
        [OperationContract]
        string CreateFolder(string Path);
        [OperationContract]
        string RenameFolder(string SourceFolder,string DestinationFolder);
        [OperationContract]
        string DeleteFolder(string Path);

        [OperationContract]
        string DeleteFile(string virtualPath);
        [OperationContract]
        IQueryable<string> GetFolders(string virtualPath);
        [OperationContract]
        IQueryable<Files> List(string virtualPath);
        [OperationContract]
        IQueryable<Files> ListFiles(string virtualPath,string Ext);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Files
    {
        long fileSize = 0;
        string fileName = "";
        private string fileDateModified;
        private DateTime fileDateCreated;
        [DataMember]
        public long FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        [DataMember]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        [DataMember]
        public DateTime FileDateCreated
        {
            get { return fileDateCreated; }
            set { fileDateCreated = value; }

        }
        
    }
    //[MessageContract]
    //public class UploadRequest
    //{
    //    [MessageHeader(MustUnderstand = true)]
    //    public string FileName { get; set; }

    //    [MessageBodyMember(Order = 1)]
    //    public Stream Stream { get; set; }
    //}

    //[MessageContract]
    //public class UploadResponse
    //{
    //    [MessageBodyMember(Order = 1)]
    //    public bool UploadSucceeded { get; set; }
    //}

    //}
}
