using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// StageData.cs
using Newtonsoft.Json;

public class StageData
{
    [JsonProperty("stage_id")]
    public int StageId { get; set; }

    [JsonProperty("chapter_num")]
    public int ChapterNum { get; set; }

    [JsonProperty("stage_num")]
    public int StageNum { get; set; }

    [JsonProperty("shuffle_count")]
    public int ShuffleCount { get; set; }

    [JsonProperty("reference_value_1")]
    public int reference_value_1 { get; set; }

    [JsonProperty("reference_value_2")]
    public int reference_value_2 { get; set; }

    [JsonProperty("reference_value_3")]
    public int reference_value_3 { get; set; }

    [JsonProperty("clear")]
    public bool Clear { get; set; }

    [JsonProperty("evaluation")]
    public int? Evaluation { get; set; }

    [JsonProperty("collectibles")]
    public bool? Collectibles { get; set; }
}
