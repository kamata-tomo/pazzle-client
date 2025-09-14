using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ClearStageRequest.cs
using Newtonsoft.Json;

public class ClearStageRequest
{
    [JsonProperty("stage_id")]
    public int StageId { get; set; }

    [JsonProperty("evaluation")]
    public int Evaluation { get; set; }

    [JsonProperty("collectibles")]
    public bool Collectibles { get; set; }
}
