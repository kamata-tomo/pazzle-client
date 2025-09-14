using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// StoreFriendAcceptRequest.cs
using Newtonsoft.Json;

public class StoreFriendAcceptRequest
{
    [JsonProperty("requesting_user_id")]
    public int RequestingUserId { get; set; }
}
