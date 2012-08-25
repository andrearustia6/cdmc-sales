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
            if (file == null) return null;
 
            if (img == null) img = new Image();
            if (!string.IsNullOrWhiteSpace(file.FileName))
            {
                byte[] buf = new byte[file.ContentLength];

                file.InputStream.Read(buf, 0, file.ContentLength);
                img.ImageData = buf;
                img.ContentType = file.ContentType;
            }

            if (img.ID > 0)
            {
                if (img.ImageData == null)
                {
                    var o = CH.GetDataById<Image>(img.ID);
                    img.ImageData = o.ImageData;
                    img.ContentType = o.ContentType;
                }
                CH.Edit<Image>(img);
            }
            else
                CH.Create<Image>(img);
            return img;
        }

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
