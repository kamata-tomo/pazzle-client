using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// RegistUserResponse.cs
using Newtonsoft.Json;

public class RegistUserResponse
{
    [JsonProperty("token")]
    public string APIToken { get; set; }
}

