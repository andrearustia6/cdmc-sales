using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public class EntityUtl
    {
        public static string GetFullName(EntityBase e)
        {
            var pe = e.GetType().GetProperty("Name_EN");
            var pc = e.GetType().GetProperty("Name_CH");
            string en = pe.GetValue(e, null).ToString();
            string ch = pc.GetValue(e, null).ToString();

            if (!string.IsNullOrEmpty(en) && !string.IsNullOrEmpty(ch))
            {
                return en + "|" + ch;
            }
            else
            {
                return en + ch;
            }
        }

        public static bool CheckPropertyAllNull(object o, params string[] properties)
        {
            foreach (var p in properties)
            {
                var property = o.GetType().GetProperty(p);
                var v = property.GetValue(o, null).ToString();
                if (!string.IsNullOrEmpty(v))
                    return false;
            }
            return true;
        }
    }
}
