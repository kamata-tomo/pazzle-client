using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ProviderStaminaRequest.cs
using Newtonsoft.Json;

public class ProviderStaminaRequest
{
    [JsonProperty("friend_id")]
    public int FriendId { get; set; }
}

