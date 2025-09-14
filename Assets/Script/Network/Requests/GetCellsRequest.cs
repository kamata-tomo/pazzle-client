using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GetCellsRequest.cs
using Newtonsoft.Json;

public class GetCellsRequest
{
    [JsonProperty("stage_id")]
    public int StageId { get; set; }
}

