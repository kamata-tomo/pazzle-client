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

    [JsonProperty("clear")]
    public bool Clear { get; set; }

    [JsonProperty("evaluation")]
    public int? Evaluation { get; set; }

    [JsonProperty("collectibles")]
    public bool? Collectibles { get; set; }
}
