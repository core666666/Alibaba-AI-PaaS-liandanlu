namespace alibaba.Model
{
    public class AITokenOptions
    {
        // 阿里云账户
        public string AccountAccessKey { get; set; }
        // 阿里云账户
        public string AccountSecretKey { get; set; }
        public string Region { get; set; }

        //ai
        public string AIAppKey { get; set; }
        //ai
        public string AIAppSecret { get; set; }

        //speech 应用的appKey
        public string appKey { get; set; }
        //speech token
        public string apptoken { get; set; }

        public string ModelId { get; set; }
    }
}
