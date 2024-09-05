using System.Text.Json.Serialization;

namespace CDO.Models
{
    public class RouteHelp
    {
        //[JsonPropertyName("roles")]
        public List<string> roles { get; set; }

        //[JsonPropertyName("url")]
        public string url { get; set; }

        public string name { get; set; }
    }
}
