using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// AcquisitionResponse.cs
using Newtonsoft.Json;

public class AcquisitionResponse
{
    [JsonProperty("title_id")]
    public int TitleId { get; set; }

    [JsonProperty("title")]
    public TitleDetail Title { get; set; }
}

public class TitleDetail
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

