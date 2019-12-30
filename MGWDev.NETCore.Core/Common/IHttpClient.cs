using System;
using System.Collections.Generic;
using System.Text;

namespace MGWDev.NETCore.Core.Common
{
    public interface IHttpClient
    {
        T GetData<T>(string endpointUrl);
        T PostData<T, U>(string endpointUrl, U data, Dictionary<string,string> additionalHeaders = null);
    }
}
