using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
// RegistUserRequest.cs
public class RegistUserRequest
{
    [JsonProperty("name")]
    public string Name { get; set; }
}


