using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;

namespace Utl
{
    public class ImageController : Controller
    {
        public static Image UploadImg(HttpRequestBase request, Image img = null)
        {
            var file = request.Files["attachments"];
           
 
            if (img == null) img = new Image();
            if (file!=null && !string.IsNullOrWhiteSpace(file.FileName))
            {
                byte[] buf = new byte[file.ContentLength];

                file.InputStream.Read(buf, 0, file.ContentLength);
                img.ImageData = buf;
                img.ContentType = file.ContentType;
            }

            if (img.ID > 0)
            {
                if (file == null)
                {
                    var old = CH.DB.Images.AsNoTracking().FirstOrDefault(i => i.ID == img.ID);
                    img.ImageData = old.ImageData;
                    img.ContentType = old.ContentType;
                }
             
                CH.Edit<Image>(img);
            }
            else
                CH.Create<Image>(img);
            return img;
        }
        [AllowAnonymous]
        public ActionResult DisplayImage(int id)
        {
            var image = CH.GetDataById<Image>(id);
            if (image != null)
            {
                byte[] imageData = image.ImageData;
                if (imageData == null) return null;
                return File(imageData, image.ContentType);
            }
            return null;
        }

    }
}
