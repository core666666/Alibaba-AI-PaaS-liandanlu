using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using TestProgram.Model;

namespace TestProgram.Controllers
{
    /// <summary>
    /// 海投网 - 校招网申
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class haichuangController : ControllerBase
    {
        /// <summary>
        /// 海投网 - 校招网申
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("haichuangDemo", Name = "haichuangDemo")]
        public async Task haichuangDemo(int page = 1)
        {

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://xyzp.haitou.cc/page-3");
            request.Headers.Add("Host", "xyzp.haitou.cc");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Pragma", "no-cache");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-User", "?1");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.Headers.Add("Cookie", "");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // 服务器指定了字符集为 UTF-8，所以在读取字符串时使用该字符集。
            var contentString = await response.Content.ReadAsStringAsync();


            // 创建HtmlDocument实例并加载HTML内容
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(contentString);

            // 查询所有的div元素。假设我们要找的div有个特定的class属性值 "my-class"
            var divs = htmlDoc.DocumentNode.SelectNodes("//div[@class='my-class']");

            if (divs != null)
            {
                foreach (var div in divs)
                {
                    // 对每个div执行操作，例如获取其内部的HTML或者文本内容
                    var divInnerHtml = div.InnerHtml;
                    var divInnerText = div.InnerText;

                    Console.WriteLine(divInnerHtml);
                    Console.WriteLine(divInnerText);
                }
            }
            else
            {
                Console.WriteLine("No divs with the specified class found.");
            }

            Console.WriteLine(contentString);
        }
    }
}
