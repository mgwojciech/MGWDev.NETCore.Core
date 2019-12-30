using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MGWDev.NETCore.Core.Model.SP
{
    public class ListItem
    {

        [JsonProperty("__metadata")]
        public Metadata Metadata { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public int EditorId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string ContentTypeId { get; set; }
    }
}
