using Newtonsoft.Json;
using System.Collections.Generic;

public class FriendRequestData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("requesting_user")]
    public RequestingUserData RequestingUser { get; set; }
}

public class RequestingUserData
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("titles")]
    public List<TitleData> Titles { get; set; }
}
