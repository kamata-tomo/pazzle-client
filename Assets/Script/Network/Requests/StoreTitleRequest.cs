using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class StoreTitleRequest
{
    [JsonProperty("title_id")]
    public int TitleId { get; set; }
}

