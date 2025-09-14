using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// StageCellData.cs
using Newtonsoft.Json;

public class StageCellData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("stage_id")]
    public int StageId { get; set; }

    [JsonProperty("piece_type")]
    public int PieceType { get; set; }

    [JsonProperty("x")]
    public int X { get; set; }

    [JsonProperty("y")]
    public int Y { get; set; }

    [JsonProperty("collectibles")]
    public bool Collectibles { get; set; }
}

