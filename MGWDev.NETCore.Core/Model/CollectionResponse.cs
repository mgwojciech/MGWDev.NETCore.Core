using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGWDev.NETCore.Core.Model
{
    public class CollectionResponse<T>
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }
    }
}
