using System;
using System.Net;

namespace MooDL.Models.Web
{
    internal class CookieWebClient : WebClient
    {
        public CookieWebClient() => CookieContainer = new CookieContainer();
        public CookieContainer CookieContainer { get; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest r)
                r.CookieContainer = CookieContainer;
            return request;
        }
    }
}