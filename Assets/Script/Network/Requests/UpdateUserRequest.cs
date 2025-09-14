using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UpdateUserRequest.cs
public class UpdateUserRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("experience")]
    public int Experience { get; set; }

    [JsonProperty("item_quantity")]
    public int ItemQuantity { get; set; }
}

