using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// FriendData.cs
using Newtonsoft.Json;

public class FriendData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("experience")]
    public int Experience { get; set; }

    [JsonProperty("is_provides_stamina")]
    public bool IsProvidesStamina { get; set; }

    [JsonProperty("current_stamina")]
    public int CurrentStamina { get; set; }

    [JsonProperty("max_stamina")]
    public int MaxStamina { get; set; }

    [JsonProperty("titles")]
    public List<TitleData> Titles { get; set; }
}

