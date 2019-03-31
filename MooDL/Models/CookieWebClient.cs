using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MooDL.Models
{
    class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; }

        public CookieWebClient() => CookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest r)
                r.CookieContainer = CookieContainer;
            return request;
        }
    }
}
