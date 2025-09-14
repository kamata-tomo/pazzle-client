using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// StoreFriendRequest.cs
using Newtonsoft.Json;

public class StoreFriendRequest
{
    [JsonProperty("recipient_id")]
    public int RecipientId { get; set; }
}

