using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MGWDev.NETCore.Core.Common
{
    public interface IAuthorizationHelper
    {
        void AuthenticateRequest(HttpWebRequest request);
    }
}
