using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using paperplusBlobStorageTest.Interface;
using paperplusBlobStorageTest.Models;

namespace paperplusBlobStorageTest.Controllers
{
    public class FileStorageProvider : IFileProvider
    {
        private CloudBlobContainer GetBlobContainer(string container)
        {
            var accountName = ConfigurationManager.AppSettings ["AzureStorageAccountName"];
            var keyValue = ConfigurationManager.AppSettings ["AzureStorageKeyName"];

            var credentials = new StorageCredentials(accountName, keyValue);
            var storageAccount = new CloudStorageAccount(credentials, useHttps: true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(container);

            return blobContainer;
    }
        /// <inheritdoc cref="IFileProvider.GetSubFolders"/>
        public List<FolderDirectoryModel> GetSubFolders(string container, string parentPath)
        {
            var blobContainer = GetBlobContainer(container);
            var dir = blobContainer.GetDirectoryReference(parentPath);

            var fileList = new List<FolderDirectoryModel>();

            foreach (IListBlobItem item in dir.ListBlobs(false, BlobListingDetails.Metadata))
            {

                if(item.GetType().Name != "CloudBlockBlob")
                {
                    fileList.Add(new FolderDirectoryModel
                    {
                        FolderName = item.StorageUri.PrimaryUri.Segments.Last().TrimEnd('/'),
                        FolderDirectory = item.StorageUri.PrimaryUri.AbsoluteUri,
                        BlobContainer = blobContainer
                    });
                }
                
            }

            return fileList;
        }

        /// <inheritdoc cref="IFileProvider.GetFileList"/>
        public List<FileViewModel> GetFileList(string container, string folderName)
        {
            var blobContainer = GetBlobContainer(container);
            var fileList = new List<FileViewModel>();
            var blobs = blobContainer.ListBlobs(prefix:folderName, true, BlobListingDetails.All).Cast<CloudBlockBlob>();

            foreach (var blob in blobs)
            {
                var fileName = blob.Name.Split('/').Last();

                fileList.Add(new FileViewModel
                {
                    FileName = fileName,
                    AzureUrl = blob.Uri.AbsoluteUri,
                    FileSize = SizeSuffix(blob.Properties.Length)
                });
            }

            return fileList;
        }

        string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            string[] sizeSuffixes =
                { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                sizeSuffixes[mag]);
        }

        /// <inheritdoc cref="IFileProvider.DeleteFile"/>
        public void DeleteFile(string container, string fileUrl)
        {
            var blobContainer = GetBlobContainer(container);

            var blob = blobContainer.GetBlockBlobReference(fileUrl);
            blob.DeleteIfExists();
        }


        public void MoveOrRenameFile(string container, string source, string directory)
        {
            var blobContainer = GetBlobContainer(container);
            var blob = blobContainer.GetBlockBlobReference(source);

            var targetDirectory = blobContainer.GetBlockBlobReference(directory);

            targetDirectory.StartCopy(blob);
            blob.Delete();
        }
    }
}