using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using paperplusBlobStorageTest.Models;

namespace paperplusBlobStorageTest.Interface
{
    public interface IFileProvider
    {
        /// <summary>
        /// Gets the path of the first level sub directories given a parent folder or blob.
        /// </summary>
        /// <returns></returns>
        List<FolderDirectoryModel> GetSubFolders(string container, string parentPath);
        /// <summary>
        /// Return list of files or blobs given the name of a directory/path
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        List<FileViewModel> GetFileList(string container, string folderName);
        /// <summary>
        /// delete file using url.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="fileUrl"></param>
        void DeleteFile(string container, string fileUrl);
        /// <summary>
        /// Creates copy in destination, then deletes source 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="directory"></param>
        void MoveOrRenameFile(string container ,string source, string directory);
    }
}