using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ShowUserResponse.cs
using Newtonsoft.Json;

public class ShowUserResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("experience")]
    public int Experience { get; set; }

    [JsonProperty("item_quantity")]
    public int ItemQuantity { get; set; }

    [JsonProperty("stamina")]
    public StaminaData Stamina { get; set; }
}

