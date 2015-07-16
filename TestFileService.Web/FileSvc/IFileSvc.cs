using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace TestFileService.Web.FileSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    
    [DataContract]
    public partial class Files
    {
        long fileSize = 0;
        string fileName = "";

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
    }

    [ServiceContract]
    public interface IFileSvc
    {

     

        [OperationContract]
        IQueryable<Files> List(string virtualPath);

        [OperationContract]
        IQueryable<string> ListFolders(string virtualPath);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
  
}
