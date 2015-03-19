using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace MyApprovals.Util
{
    class CookieStore
    {
        public static void StoreCookies(CookieCollection cc)
        {
            IEnumerable<Cookie> ic = cc.Cast<Cookie>();
            foreach (Cookie cookie in ic)
                CookieStore.StoreCookie(cookie);
        }

        public static void StoreCookie(Cookie c)
        {
            Settings settings = new Settings();
            settings.AddOrUpdateValue("cookie-name", c.Name);
            settings.AddOrUpdateValue("cookie-value", c.Value);
            settings.Save();
        }

        public static Cookie GetStoredCookie()
        {
            Settings settings = new Settings();
            string name = settings.GetValueOrDefault<string>("cookie-name", null);
            string value = settings.GetValueOrDefault<string>("cookie-value", null);
            if (name == null)
                return null;
            return new Cookie(name, value);
        }

        
    }
}
