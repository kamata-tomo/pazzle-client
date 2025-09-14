using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// StaminaData.cs
using Newtonsoft.Json;

public class StaminaData
{
    [JsonProperty("current_stamina")]
    public int CurrentStamina { get; set; }

    [JsonProperty("max_stamina")]
    public int MaxStamina { get; set; }
}
