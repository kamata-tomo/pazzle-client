using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TitleData.cs
using Newtonsoft.Json;

public class TitleData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}
