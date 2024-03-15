using Newtonsoft.Json;

namespace alibaba.Model
{
    public class TokenInfo
    {
        [JsonProperty("accessKeyId")]
        public string accessKeyId { get; set; }

        [JsonProperty("accessKeySecret")]
        public string accessKeySecret { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }

        [JsonProperty("expireTime")]
        public DateTime expireTime { get; set; }
    }
}
