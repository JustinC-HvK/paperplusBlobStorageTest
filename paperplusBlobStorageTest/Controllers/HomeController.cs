using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using paperplusBlobStorageTest.Interface;

namespace paperplusBlobStorageTest.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult GetFolder()
        {
            var bsp = new BlobStorageProvider();
            var folders = bsp.GetSubFolders("data", "media/images");

            return View(folders);
        }

        public ActionResult GetFileList()
        {
            ViewBag.Message = "Your application description page.";

            var bsp = new BlobStorageProvider();
            var files = bsp.GetFileList("data","media/images");

            


            return View(files);
        }

        public ActionResult Delete()
        {
            ViewBag.Message = "Your contact page.";

            var bsp = new BlobStorageProvider();
            bsp.DeleteFile("data", "media/images/!!!!!!TESTING.jpg");

            return View();
        }

        public ActionResult MoveFile()
        {
            ViewBag.Message = "Your contact page.";

            var bsp = new BlobStorageProvider();
            bsp.MoveOrRenameFile("data", "media/images/!!!!!!TESTING.jpg", "media/images/Content/!!!!!!TESTING.jpg");

            return View();
        }
    }
}