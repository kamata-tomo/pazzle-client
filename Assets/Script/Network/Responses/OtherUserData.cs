using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class OtherUserData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("titles")]
    public List<TitleData> Titles { get; set; }

    [JsonProperty("is_friend")]
    public bool IsFriend { get; set; }

    [JsonProperty("is_request_sent")]
    public bool IsRequestSent { get; set; }

    [JsonProperty("is_request_received")]
    public bool IsRequestReceived { get; set; }
}
