using MGWDev.NETCore.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MGWDev.NETCore.Core.SP
{
    public class SPOHttpClient : IHttpClient
    {
        public IAuthorizationHelper AuthorizationHelper { get; set; }
        public string SiteUrl { get; protected set; }
        public string Accept { get; set; } = "application/json";
        public string ContentType { get; set; } = "application/json;odata=verbose";
        public SPOHttpClient(string login, string password, string siteUrl)
        {
            SiteUrl = siteUrl;
            AuthorizationHelper = GetAuthorizationHelper(login, password, siteUrl);
        }

        protected virtual IAuthorizationHelper GetAuthorizationHelper(string login, string password, string siteUrl)
        {
            return new SPOAuthorizationHelper(login, password, siteUrl);
        }

        public T GetData<T>(string endpointUrl)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(SiteUrl + endpointUrl);
            webRequest.Accept = Accept;
            AuthorizationHelper.AuthenticateRequest(webRequest);
            WebResponse response = webRequest.GetResponse();
            return DeserializeResponse<T>(response);
        }

        protected virtual T DeserializeResponse<T>(WebResponse response)
        {
            string responseString = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            return JsonConvert.DeserializeObject<T>(responseString);
        }
        protected virtual string SerializeRequest<T>(T request)
        {
            return JsonConvert.SerializeObject(request);
        }

        public T PostData<T, U>(string endpointUrl, U data, Dictionary<string, string> additionalHeaders = null)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(SiteUrl + endpointUrl);
            webRequest.Method = "POST";
            webRequest.Accept = Accept;
            webRequest.ContentType = ContentType;
            if(additionalHeaders != null)
            {
                foreach(var header in additionalHeaders)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }
            if (data != null)
            {
                string serializedRequest = SerializeRequest(data);
                using(Stream requestStream = webRequest.GetRequestStream())
                {
                    using(StreamWriter writer = new StreamWriter(requestStream))
                    {
                        writer.Write(serializedRequest);
                    }
                }
            }
            else
            {
                webRequest.ContentLength = 0;
            }

            AuthorizationHelper.AuthenticateRequest(webRequest);
            try
            {
                WebResponse response = webRequest.GetResponse();
                return DeserializeResponse<T>(response);
            }
            catch(WebException wEx)
            {
                using (Stream exStream = wEx.Response.GetResponseStream())
                {
                    using(StreamReader reader = new StreamReader(exStream))
                    {
                        throw new Exception("Server returned " + wEx.Status.ToString(), new Exception(reader.ReadToEnd()));
                    }
                }
            }
            catch(Exception ex)
            {
                return default(T);
            }
        }
    }
}
