using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ProviderStaminaRequest
{
    [JsonProperty("friend_id")]
    public int FriendId { get; set; }
}

