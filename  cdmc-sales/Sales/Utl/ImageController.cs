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
        public static ClientImage UploadImg(HttpRequestBase request, ClientImage img = null)
        {
            if (img == null) img = new ClientImage();

            var file = request.Files["attachments"];
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
                    var o = CH.GetDataById<ClientImage>(img.ID);
                    img.ImageData = o.ImageData;
                    img.ContentType = o.ContentType;
                }
                CH.Edit<ClientImage>(img);
            }
            else
                CH.Create<ClientImage>(img);
            return img;
        }

        public ActionResult DisplayClientImage(int id)
        {
            var image = CH.GetDataById<ClientImage>(id);
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
