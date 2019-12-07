using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace LocalProject.Controllers
{
    public class ImageController : Controller
    {
        [HttpPost]
        public ActionResult Add(HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string filePath = Path.Combine(
                    Server.MapPath("~/UserUploads/"), fileName
                    );

                ImageFile.SaveAs(filePath);

                return Json(new { location = Path.Combine("/UserUploads/", fileName)});
            }

            return Json(new { location = "" });
        }
    }
}