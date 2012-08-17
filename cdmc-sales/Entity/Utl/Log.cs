using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utilities
{
    public class Log
    {
        public static void LogError(Exception ex, Object errorClass)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
        }
    }
}