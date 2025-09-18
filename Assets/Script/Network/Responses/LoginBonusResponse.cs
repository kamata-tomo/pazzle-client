using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// LoginBonusResponse.cs
using Newtonsoft.Json;

public class LoginBonusResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("bonus")]
    public int Bonus { get; set; }

    [JsonProperty("item_quantity")]
    public int ItemQuantity { get; set; }

    [JsonProperty("login_streak")]
    public int LoginStreak { get; set; }
}
