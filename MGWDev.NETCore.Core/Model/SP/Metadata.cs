using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGWDev.NETCore.Core.Model.SP
{
    public class Metadata
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
