using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MooDL.Models
{
    class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        public CookieWebClient() => CookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }
            return request;
        }
    }
}
