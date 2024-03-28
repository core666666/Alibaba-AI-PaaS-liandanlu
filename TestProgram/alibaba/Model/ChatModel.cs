using AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models;

namespace alibaba.Model
{
    public class ChatModel : LiandanluExclusiveModelRequest
    {
        /// <summary>
        /// chatMemo：1    炼丹炉：2
        /// </summary>
        public int chatType { get; set; }
    }

    //{
    //"data": "阿里",
    //"isFinished": false,
    //"outputStreamFinished": false
    //}

    public class ChatMemoResponse
    {
        public string data { get; set; }
        public bool isFinished { get; set; }
        public bool outputStreamFinished { get; set; }
    }
}
