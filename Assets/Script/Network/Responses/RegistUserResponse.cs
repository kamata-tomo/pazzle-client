using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RegistUserResponse
{
    [JsonProperty("token")]
    public string APIToken { get; set; }
}

