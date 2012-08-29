using System.Text;
using System.Globalization;
using System.Web;

namespace Utl
{

    public static class HtmlExtensions
    {
        public static HtmlString GetHtmlString(string s)
        {
            return new HtmlString(s);
        }
    }
}