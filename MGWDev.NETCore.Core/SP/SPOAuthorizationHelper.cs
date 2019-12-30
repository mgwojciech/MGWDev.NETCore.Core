using MGWDev.NETCore.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace MGWDev.NETCore.Core.SP
{
    public class SPOAuthorizationHelper : IAuthorizationHelper
    {
        protected string Login { get; set; }
        protected string Password { get; set; }
        protected string SiteUrl { get; set; }
        private string AuthorizationToken { get; set; }
        private string RequestDigest { get; set; }
        public SPOAuthorizationHelper(string login, string password, string siteUrl)
        {
            Login = login;
            Password = password;
            SiteUrl = siteUrl.TrimEnd('/');
            string loginToken = GetLoginToken();
            AuthorizationToken = GetAuthorizationToken(loginToken);
            RequestDigest = GetRequestDigest(AuthorizationToken);
        }
        public void AuthenticateRequest(HttpWebRequest request)
        {
            request.Headers.Add("Cookie", "SPOIDCRL=" + AuthorizationToken);
            request.Headers.Add("X-RequestDigest", RequestDigest);
        }

        protected virtual string GetRequestDigest(string authorizationToken)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp($"{SiteUrl}/_api/contextinfo");
            webRequest.Method = "POST";
            webRequest.Headers.Add("Cookie", "SPOIDCRL=" + authorizationToken);
            webRequest.ContentLength = 0;
            WebResponse response = webRequest.GetResponse();
            string responseString = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
            }
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(responseString);

            return xmlResponse.GetElementsByTagName("d:FormDigestValue")[0].InnerText;
        }

        protected virtual string GetAuthorizationToken(string loginToken)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp("https://mwdevsb.sharepoint.com/_vti_bin/idcrl.svc/");
            webRequest.Headers.Add("Authorization", "BPOSIDCRL " + loginToken);
            webRequest.Headers.Add("X-IDCRL_ACCEPTED", "t");

            HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
            string setCookie = response.Headers["Set-Cookie"];
            return setCookie.Replace("SPOIDCRL=", "");

        }

        protected virtual string GetLoginToken()
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp("https://login.microsoft.com/rst2.srf");
            webRequest.ContentType = "application/soap+xml";
            webRequest.Method = "POST";
            string body = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<S:Envelope xmlns:S=""http://www.w3.org/2003/05/soap-envelope"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" 
xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" 
xmlns:wsa=""http://www.w3.org/2005/08/addressing"" xmlns:wst=""http://schemas.xmlsoap.org/ws/2005/02/trust"">
  <S:Header>
    <wsa:Action S:mustUnderstand=""1"">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</wsa:Action>
    <wsa:To S:mustUnderstand=""1"">https://login.microsoftonline.com/rst2.srf</wsa:To>
    <ps:AuthInfo xmlns:ps=""http://schemas.microsoft.com/LiveID/SoapServices/v1"" Id=""PPAuthInfo"">
      <ps:BinaryVersion>5</ps:BinaryVersion>
      <ps:HostingApp>Managed IDCRL</ps:HostingApp>
    </ps:AuthInfo>
    <wsse:Security>
            <wsse:UsernameToken wsu:Id=""user"">
                <wsse:Username>{Login}</wsse:Username>
                <wsse:Password>{Password}</wsse:Password>
            </wsse:UsernameToken>
            <wsu:Timestamp Id=""Timestamp"">
                <wsu:Created>2019-12-30T12:10:46.6281129Z</wsu:Created>
                <wsu:Expires>2019-12-31T12:10:46.6281129Z</wsu:Expires>
            </wsu:Timestamp>
</wsse:Security>
  </S:Header>
  <S:Body>
    <wst:RequestSecurityToken xmlns:wst=""http://schemas.xmlsoap.org/ws/2005/02/trust"" Id=""RST0"">
      <wst:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</wst:RequestType>
      <wsp:AppliesTo>
        <wsa:EndpointReference>
          <wsa:Address>sharepoint.com</wsa:Address>
        </wsa:EndpointReference>
      </wsp:AppliesTo>
      <wsp:PolicyReference URI=""MBI""></wsp:PolicyReference>
    </wst:RequestSecurityToken>
  </S:Body>
</S:Envelope>";
            using (Stream requestStream = webRequest.GetRequestStream())
            {
                using (StreamWriter writer = new StreamWriter(requestStream))
                {
                    writer.Write(body);
                }
            }

            WebResponse response = webRequest.GetResponse();
            string responseString = "";
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
            }

            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(responseString);

            return xmlResponse.GetElementsByTagName("wsse:BinarySecurityToken")[0].InnerText;
        }
    }
}
