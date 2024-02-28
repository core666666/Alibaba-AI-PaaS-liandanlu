namespace TestProgram.Model
{
    public class Answer_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string answer_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string question_id { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public string answer_content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_right { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string answer_position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_at { get; set; }
    }

    public class QuestionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string question_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string exam_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string question_type { get; set; }
        /// <summary>
        /// 人体在电磁场作用下，由于(    )将使人体受到不同程度的伤害。
        /// </summary>
        public string question_title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string question_position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_deleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updated_at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Answer_listItem> answer_list { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string record { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<QuestionsItem> questions { get; set; }
    }

    public class Root
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 请求成功
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }

}
