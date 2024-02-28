using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;
using TestProgram.Model;

/*
    冀时安全小程序 - 答题
 */

namespace TestProgram.Controllers
{

    /// <summary>
    /// 小程序
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class jishianquanController : ControllerBase
    {
        private static string token = "";
        private static string useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 MicroMessenger/7.0.20.1781 NetType/WIFI MiniProgramEnv/Windows WindowsWechat/WMPF XWEB/8447";

        #region 练习模式答题 - 1分/次（每天10安全币）
        /// <summary>
        /// 练习模式答题 - 1分/次（每天10安全币）
        /// </summary>
        /// <param name="runTimes"></param>
        /// <returns></returns>
        [HttpGet("practiceGetAndSubmitQuestion", Name = "practiceGetAndSubmitQuestion")]
        public async Task<HttpResponseMessage> practiceGetAndSubmitQuestion(int runTimes = 10)
        {
            HttpResponseMessage lastResponse = null;
            try
            {
                for (int i = 0; i < runTimes; i++)
                {
                    var client = new HttpClient();
                    var response = await GetPracticeQuestionContent(client);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);

                        // 解析返回的内容并构造提交的内容
                        var submitData = ProcessResponseContent(responseBody);

                        // 随机等待20-40秒
                        Random rnd = new Random();
                        int delay = rnd.Next(20, 41) * 1000;
                        await Task.Delay(delay);

                        // 提交数据
                        await SubmitPracticeQuestionContent(client, submitData);

                        // 访问用户信息 - 防止判作弊
                        await GetUserInfo();
                    }

                    lastResponse = response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return lastResponse;
        }

        /// <summary>
        /// 获取练习问题
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> GetPracticeQuestionContent(HttpClient client)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/index");
                request.Headers.Add("Host", "data-design.hbzhengwu.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("xweb_xhr", "1");
                request.Headers.Add("api", "XBm6HzzfsYJlbiH");
                request.Headers.Add("User-Agent", useragent);
                request.Headers.Add("token", token);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Sec-Fetch-Site", "cross-site");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("Accept-Language", "zh-CN,zh");
                var content = new StringContent("{\"category_id\":\"1\",\"type\":\"exercise\"}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 提交练习答案
        /// </summary>
        /// <param name="client"></param>
        /// <param name="submitData"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> SubmitPracticeQuestionContent(HttpClient client, object submitData)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/submit");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh");
            var content = new StringContent(submitData.ToString());//new StringContent("{\"category_id\":\"1\",\"record\":\"56262ffdc5a37920b89e1404404c4d7d\",\"answer\":[{\"question_id\":\"308\",\"answers\":[\"709\"]},{\"question_id\":\"378\",\"answers\":[\"848\"]},{\"question_id\":\"3752\",\"answers\":[\"10369\"]},{\"question_id\":\"224\",\"answers\":[\"494\"]},{\"question_id\":\"3739\",\"answers\":[\"10318\"]}]}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return response.EnsureSuccessStatusCode();
        }
        #endregion

        #region 获取资讯 - 浏览资讯 - 获得安全币(每天20安全币)
        /// <summary>
        /// 获取资讯 - 浏览资讯 - 获得安全币(每天20安全币) - ※※※※※※※※※※※
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetArticleListAndReadThem", Name = "GetArticleListAndReadThem")]
        [DisplayName("获取资讯 - 浏览资讯 - 获得安全币(每天20安全币)")]
        public async Task GetArticleListAndReadThem(int pageIndex = 1, int pageSize = 30)
        {
            var response = await GetArticleList(pageIndex, pageSize);
            if (response.IsSuccessStatusCode)
            {
                var contentStr = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<ArticleListResponse>(contentStr);

                if (jsonResponse != null && jsonResponse.data != null)
                {
                    foreach (var article in jsonResponse.data)
                    {
                        await ReadArticle(article.identify);
                        await Task.Delay(new Random().Next(5000, 10001));  // 随机延迟5s到10s
                    }
                }
            }
        }
        private class ArticleListResponse
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<Article> data { get; set; }
        }

        private class Article
        {
            public string identify { get; set; }
            // 其他属性...
        }

        /// <summary>
        /// 获取资讯列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetArticleList", Name = "GetArticleList")]
        [NonAction]
        public async Task<HttpResponseMessage> GetArticleList(int pageIndex = 1, int pageSize = 20)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://data-design.hbzhengwu.com/api/general/article?page_size={pageSize}&page={pageIndex}&code=news");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }

        [HttpGet("ReadArticle", Name = "ReadArticle")]
        [NonAction]
        public async Task<HttpResponseMessage> ReadArticle(string id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://data-design.hbzhengwu.com/api/general/article/{id}");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }

        #endregion

        #region 获取视频 - 浏览视频 - 获得安全币(每天20安全币)
        /// <summary>
        /// 获取视频 - 浏览视频 - 获得安全币(每天20安全币) - ※※※※※※※※※※※
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetVideoListAndReadThem", Name = "GetVideoListAndReadThem")]
        [DisplayName("获取视频 - 浏览视频 - 获得安全币(每天20安全币)")]
        public async Task GetVideoListAndReadThem(int pageIndex = 1, int pageSize = 30)
        {
            var response = await GetVideoList(pageIndex, pageSize);
            if (response.IsSuccessStatusCode)
            {
                var contentStr = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<VideoListResponse>(contentStr);

                if (jsonResponse != null && jsonResponse.data != null)
                {
                    foreach (var video in jsonResponse.data.video)
                    {
                        await WatchVideo(video.video_id);
                        await Task.Delay(new Random().Next(5000, 10001));  // 随机延迟5s到10s
                    }
                }
            }
        }
        private class VideoListResponse
        {
            public int code { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        }

        private class Data
        {
            public string total { get; set; }
            public int current { get; set; }
            public List<Video> video { get; set; }
        }
        private class Video
        {
            public string video_id { get; set; }
            public string category_id { get; set; }
            public string title { get; set; }
            public string cover { get; set; }
            public string video_link { get; set; }
            public string duration { get; set; }
            public string identify { get; set; }
        }

        /// <summary>
        /// 获取视频分类列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVideoCategoryList", Name = "GetVideoCategoryList")]
        [NonAction]
        public async Task<HttpResponseMessage> GetVideoCategoryList(int pageIndex = 1, int pageSize = 20)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://data-design.hbzhengwu.com/api/video/category?category_id=1");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }

        /// <summary>
        /// 获取视频列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVideoList", Name = "GetVideoList")]
        [NonAction]
        public async Task<HttpResponseMessage> GetVideoList(int pageIndex = 1, int pageSize = 20)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://data-design.hbzhengwu.com/api/video/index?category_id=3&page_size={pageSize}&page={pageIndex}&keyword=");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }

        [HttpGet("WatchVideo", Name = "WatchVideo")]
        [NonAction]
        public async Task<HttpResponseMessage> WatchVideo(string id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/video/start");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent("{\"id\":id}", null, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }

        #endregion

        #region 获取个人信息
        /// <summary>
        /// 获取个人信息 - 防止判作弊
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserInfo", Name = "GetUserInfo")]
        [NonAction]
        public async Task<HttpResponseMessage> GetUserInfo()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://data-design.hbzhengwu.com/api/profile/detail");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return response;
        }
        #endregion

        #region 单人答题 - 10分/次
        /// <summary>
        /// 单人答题 - 10分/次 - ※※※※※
        /// </summary>
        /// <param name="runTimes"></param>
        /// <returns></returns>
        [HttpGet("GetAndSubmitQuestion", Name = "GetAndSubmitQuestion")]
        public async Task<HttpResponseMessage> GetAndSubmitQuestion(int runTimes = 25)
        {
            HttpResponseMessage lastResponse = null;
            try
            {
                for (int i = 0; i < runTimes; i++)
                {
                    var client = new HttpClient();
                    var response = await GetQuestionContent(client);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);

                        // 解析返回的内容并构造提交的内容
                        var submitData = ProcessResponseContent(responseBody);

                        // 随机等待20-40秒
                        Random rnd = new Random();
                        int delay = rnd.Next(20, 41) * 1000;
                        await Task.Delay(delay);

                        // 提交数据
                        await SubmitQuestionContent(client, submitData);

                        // 访问用户信息 - 防止判作弊
                        await GetUserInfo();
                    }

                    lastResponse = response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return lastResponse;
        }

        /// <summary>
        /// 获取问题
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> GetQuestionContent(HttpClient client)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/index");
                request.Headers.Add("Host", "data-design.hbzhengwu.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("xweb_xhr", "1");
                request.Headers.Add("api", "XBm6HzzfsYJlbiH");
                request.Headers.Add("User-Agent", useragent);
                request.Headers.Add("token", token);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Sec-Fetch-Site", "cross-site");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("Accept-Language", "zh-CN,zh");
                var content = new StringContent("{\"category_id\":\"1\",\"type\":\"single\"}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 提交答案
        /// </summary>
        /// <param name="client"></param>
        /// <param name="submitData"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> SubmitQuestionContent(HttpClient client, object submitData)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/submit");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh");
            var content = new StringContent(submitData.ToString());//new StringContent("{\"category_id\":\"1\",\"record\":\"56262ffdc5a37920b89e1404404c4d7d\",\"answer\":[{\"question_id\":\"308\",\"answers\":[\"709\"]},{\"question_id\":\"378\",\"answers\":[\"848\"]},{\"question_id\":\"3752\",\"answers\":[\"10369\"]},{\"question_id\":\"224\",\"answers\":[\"494\"]},{\"question_id\":\"3739\",\"answers\":[\"10318\"]}]}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return response.EnsureSuccessStatusCode();
        }

        //转换结果
        [NonAction]
        public JObject ProcessResponseContent(string responseContent)
        {
            // 将响应内容解析为 JObject
            JObject jsonResponse = JObject.Parse(responseContent);

            // 创建新的 JObject 来存储结果
            JObject result = new JObject();

            // 添加 category_id
            result["category_id"] = "1";
            // 添加 record
            result["record"] = jsonResponse["data"]["record"];

            // 创建答案的 JArray
            JArray answerArray = new JArray();

            // 遍历每个问题
            foreach (var question in jsonResponse["data"]["questions"])
            {
                JObject answerObject = new JObject();
                answerObject["question_id"] = question["question_id"].ToString();

                JArray answers = new JArray();
                // 查找所有正确答案的 answer_id
                foreach (var answer in question["answer_list"])
                {
                    if (answer["is_right"].ToString() == "1")
                    {
                        answers.Add(answer["answer_id"].ToString());
                    }
                }
                answerObject["answers"] = answers;

                // 将答案对象添加到答案数组
                answerArray.Add(answerObject);
            }

            // 将答案数组添加到结果对象
            result["answer"] = answerArray;

            return result;
        }
        #endregion

        #region 四人答题 - 20分/次
        /// <summary>
        /// 四人答题 - 分享
        /// </summary>
        /// <returns></returns>
        [HttpGet("multiShare", Name = "multiShare")]
        public async Task<string> multiShare()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/multiShare");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/70/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            var content = new StringContent("{\"category_id\":\"1\"}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            JObject jsonResponse = JObject.Parse(responseBody);
            return jsonResponse["data"]["record"].ToString();
        }

        /// <summary>
        /// 四人答题 - 20分/次 - ※※※※※※※※※※※
        /// </summary>
        /// <param name="runTimes"></param>
        /// <returns></returns>
        [HttpGet("GetAndSubmitFourQuestion", Name = "GetAndSubmitFourQuestion")]
        public async Task<HttpResponseMessage> GetAndSubmitFourQuestion(int runTimes = 25)
        {
            HttpResponseMessage lastResponse = null;
            try
            {
                for (int i = 0; i < runTimes; i++)
                {
                    // 获取 record
                    string record = await multiShare();

                    var client = new HttpClient();
                    var response = await GetFourQuestionContent(client, record);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);

                        // 解析返回的内容并构造提交的内容
                        var submitData = FourProcessResponseContent(responseBody);

                        // 随机等待20-40秒
                        Random rnd = new Random();
                        int delay = rnd.Next(20, 41) * 1000;
                        await Task.Delay(delay);

                        // 提交数据
                        await SubmitFourQuestionContent(client, submitData);

                        // 访问用户信息 - 防止判作弊
                        await GetUserInfo();
                    }

                    lastResponse = response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return lastResponse;
        }

        /// <summary>
        /// 获取问题 - 四人答题
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> GetFourQuestionContent(HttpClient client, string record)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://data-design.hbzhengwu.com/api/tester/multiPlayer?record={record}");
                request.Headers.Add("Host", "data-design.hbzhengwu.com");
                request.Headers.Add("Connection", "keep-alive");
                request.Headers.Add("xweb_xhr", "1");
                request.Headers.Add("api", "XBm6HzzfsYJlbiH");
                request.Headers.Add("User-Agent", useragent);
                request.Headers.Add("token", token);
                request.Headers.Add("Accept", "*/*");
                request.Headers.Add("Sec-Fetch-Site", "cross-site");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("Accept-Language", "zh-CN,zh");
                var content = new StringContent(string.Empty);
                request.Content = content;
                var response = await client.SendAsync(request);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                return response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 提交答案 - 四人答题
        /// </summary>
        /// <param name="client"></param>
        /// <param name="submitData"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<HttpResponseMessage> SubmitFourQuestionContent(HttpClient client, object submitData)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, "https://data-design.hbzhengwu.com/api/tester/multiPlayer");
            request.Headers.Add("Host", "data-design.hbzhengwu.com");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("xweb_xhr", "1");
            request.Headers.Add("api", "XBm6HzzfsYJlbiH");
            request.Headers.Add("User-Agent", useragent);
            request.Headers.Add("token", token);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Sec-Fetch-Site", "cross-site");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Referer", "https://servicewechat.com/wx1073e1fa063b0206/67/page-frame.html");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh");
            var content = new StringContent(submitData.ToString());//new StringContent("{\"category_id\":\"1\",\"record\":\"56262ffdc5a37920b89e1404404c4d7d\",\"answer\":[{\"question_id\":\"308\",\"answers\":[\"709\"]},{\"question_id\":\"378\",\"answers\":[\"848\"]},{\"question_id\":\"3752\",\"answers\":[\"10369\"]},{\"question_id\":\"224\",\"answers\":[\"494\"]},{\"question_id\":\"3739\",\"answers\":[\"10318\"]}]}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return response.EnsureSuccessStatusCode();
        }

        //转换结果
        [NonAction]
        public JObject FourProcessResponseContent(string responseContent)
        {
            // 将响应内容解析为 JObject
            JObject jsonResponse = JObject.Parse(responseContent);

            // 创建新的 JObject 来存储结果
            JObject result = new JObject();

            // 添加 category_id
            result["category_id"] = "1";
            // 添加 record
            result["record"] = jsonResponse["data"]["record"];

            // 创建答案的 JArray
            JArray answerArray = new JArray();

            // 遍历每个问题
            foreach (var question in jsonResponse["data"]["questions"])
            {
                JObject answerObject = new JObject();
                answerObject["question_id"] = question["question_id"].ToString();

                JArray answers = new JArray();
                // 查找所有正确答案的 answer_id
                foreach (var answer in question["answer_list"])
                {
                    if (answer["is_right"].ToString() == "1")
                    {
                        answers.Add(answer["answer_id"].ToString());
                    }
                }
                answerObject["answers"] = answers;

                // 将答案对象添加到答案数组
                answerArray.Add(answerObject);
            }

            // 将答案数组添加到结果对象
            result["answer"] = answerArray;

            return result;
        }
        #endregion
    }
}
