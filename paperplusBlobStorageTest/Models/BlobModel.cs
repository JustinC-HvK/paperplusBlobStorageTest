using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;

namespace paperplusBlobStorageTest.Models
{
    public class FileViewModel
    {

        public string FileName { get; set; }
        public string AzureUrl { get; set; }
        public string FileSize { get; set; }

    }

    public class FolderDirectoryModel
    {

        public string FolderName { get; set; }
        public string FolderDirectory { get; set; }
        public CloudBlobContainer BlobContainer { get; set; }

    }

}