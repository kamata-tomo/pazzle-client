using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// StaminaChangeRequest.cs
using Newtonsoft.Json;

public class StaminaChangeRequest
{
    [JsonProperty("reason_id")]
    public int ReasonId { get; set; }

    [JsonProperty("amount")]
    public int? Amount { get; set; }
}

