using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// SaveData.cs
using Newtonsoft.Json;

public class SaveData
{
    [JsonProperty("UserName")]
    public string UserName { get; set; }

    [JsonProperty("APIToken")]
    public string APIToken { get; set; }
}
