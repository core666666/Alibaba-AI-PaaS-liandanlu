using alibaba.Model;
using AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Tea;
using Tea.Utils;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace alibaba.Controllers
{
    [Route("v1.0/aiPaaS/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly AITokenOptions _aiTokenOptions;
        private readonly IHttpClientFactory _httpClientFactory;

        public AiController(IHttpClientFactory httpClientFactory, IOptions<AITokenOptions> aiTokenOptions)
        {
            _httpClientFactory = httpClientFactory;
            _aiTokenOptions = aiTokenOptions.Value;
        }

        public static string AccessToken = "";
        private static DateTime AccessTokenExpiry = DateTime.MinValue;

        [NonAction]
        public static AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client CreateClient()
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config();
            config.Protocol = "https";
            config.RegionId = "central";
            return new AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client(config);
        }

        [NonAction]
        public void getAccessToken()
        {
            AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client client = CreateClient();
            AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest getAccessTokenRequest = new AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest
            {
                AppKey = _aiTokenOptions.AIAppKey,
                AppSecret = _aiTokenOptions.AIAppSecret,
            };
            try
            {
                var result = client.GetAccessToken(getAccessTokenRequest);
                AccessToken = result.Body.AccessToken;
                AccessTokenExpiry = DateTime.UtcNow.AddHours(2);
            }
            catch (TeaException err)
            {
                if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                {
                    // err 中含有 code 和 message 属性，可帮助开发定位问题
                }
            }
            catch (Exception _err)
            {
                TeaException err = new TeaException(new Dictionary<string, object>
                {
                    { "message", _err.Message }
                });
                if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                {
                    // err 中含有 code 和 message 属性，可帮助开发定位问题
                }
            }
        }

        [NonAction]
        public static AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Client AIPaasCreateClient()
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config();
            config.Protocol = "https";
            config.RegionId = "central";
            return new AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Client(config);
        }

        /// <summary>
        /// 问答
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost("g")]
        public async Task<Dictionary<string, object>> generate(LiandanluExclusiveModelRequest args)
        {
            if (DateTime.UtcNow > AccessTokenExpiry || string.IsNullOrEmpty(AccessToken))
            {
                getAccessToken();
            }
            AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Client client = AIPaasCreateClient();
            AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models.LiandanluExclusiveModelHeaders liandanluExclusiveModelHeaders = new AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models.LiandanluExclusiveModelHeaders();
            liandanluExclusiveModelHeaders.XAcsDingtalkAccessToken = AccessToken;
            AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models.LiandanluExclusiveModelRequest liandanluExclusiveModelRequest = new AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models.LiandanluExclusiveModelRequest
            {
                Module = args.Module,
                ModelId = args.ModelId,
                Prompt = args.Prompt,
                UserId = args.UserId,
            };
            try
            {
                var response = await client.LiandanluExclusiveModelWithOptionsAsync(liandanluExclusiveModelRequest, liandanluExclusiveModelHeaders, new AlibabaCloud.TeaUtil.Models.RuntimeOptions());
                return response.Body.Result;
            }
            catch (TeaException err)
            {
                if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                {
                    getAccessToken(); // Retry getting the AccessToken

                    // Retry the request with the new token
                    liandanluExclusiveModelHeaders.XAcsDingtalkAccessToken = AccessToken;
                    var retryResponse = await client.LiandanluExclusiveModelWithOptionsAsync(liandanluExclusiveModelRequest, liandanluExclusiveModelHeaders, new AlibabaCloud.TeaUtil.Models.RuntimeOptions());
                    return retryResponse.Body.Result;
                }
            }
            catch (Exception _err)
            {
                TeaException err = new TeaException(new Dictionary<string, object>
                {
                    { "message", _err.Message }
                });
                if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                {
                    // err 中含有 code 和 message 属性，可帮助开发定位问题
                }
            }
            return null;
        }

        /// <summary>
        /// 语音转文本
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("speech")]
        public async Task<IActionResult> recorder(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // 这里替换为您的appkey和token
            string appKey = _aiTokenOptions.appKey;
            string token = GetIntelligentSpeechTokenAsync().Result.token;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://nls-gateway-cn-shanghai.aliyuncs.com/stream/v1/asr?appkey={appKey}");
            request.Headers.Add("X-NLS-Token", token);

            request.Content = new ByteArrayContent(memoryStream.ToArray());
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                return Ok(new { translatedText = jsonResponse });
            }
            catch (HttpRequestException e)
            {
                // 异常处理，记录错误日志等
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// 因为阿里云没有提供 智能语音服务的 C#的SDK，只能是写openAPI进行访问
        /// 链接：https://help.aliyun.com/document_detail/113251.html?spm=a2c4g.72153.0.0.597c3bfbEZJhST
        /// </summary>
        /// <returns></returns>
        public async Task<TokenInfo> GetIntelligentSpeechTokenAsync()
        {
            // 所有请求参数
            var queryParamsDict = new Dictionary<string, string>
            {
                ["AccessKeyId"] = _aiTokenOptions.AccountAccessKey,
                ["Action"] = "CreateToken",
                ["Version"] = "2019-02-28",
                ["Format"] = "JSON",
                ["RegionId"] = _aiTokenOptions.Region,
                ["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                ["SignatureMethod"] = "HMAC-SHA1",
                ["SignatureVersion"] = "1.0",
                ["SignatureNonce"] = Guid.NewGuid().ToString(),
            };

            // 1.构造规范化的请求字符串
            var queryString = string.Join("&",
                queryParamsDict
                    .OrderBy(it => it.Key)
                    .Select(it => $"{WebUtility.UrlEncode(it.Key)}={WebUtility.UrlEncode(it.Value)}")
            );

            // 2.构造签名字符串
            var stringToSign = $"GET&{WebUtility.UrlEncode("/")}&{WebUtility.UrlEncode(queryString)}";

            // 3.计算签名
            string signBase64;
            using (var hash = new HMACSHA1(Encoding.UTF8.GetBytes(_aiTokenOptions.AccountSecretKey + "&")))
            {
                signBase64 = Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
                hash.Clear();
            }
            var signature = WebUtility.UrlEncode(signBase64);

            // 4.将签名加入到第1步获取的请求字符串
            var queryStringWithSign = $"Signature={signature}&{queryString}";

            // 5.发送HTTP GET请求,获取token。
            var request = (HttpWebRequest)WebRequest.Create($"https://nls-meta.cn-shanghai.aliyuncs.com/?{queryStringWithSign}");
            request.Method = "GET";
            request.Accept = "application/json";

            using var response = request.GetResponse();
            var statusCode = ((HttpWebResponse)response).StatusCode;

            if (statusCode != HttpStatusCode.OK)
            {
                var errorMessage = new StreamReader(response.GetResponseStream()).ReadToEnd();
                throw new Exception($"获取智能语音令牌失败。状态码：{statusCode}，错误信息：{errorMessage}");
            }

            var result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var tokenObj = JObject.Parse(result)["Token"] as JObject;
            var token = tokenObj["Id"].ToObject<string>();
            var expireTime = DateTimeOffset.FromUnixTimeSeconds(tokenObj["ExpireTime"].ToObject<long>()).UtcDateTime;

            return new TokenInfo()
            {
                accessKeyId = _aiTokenOptions.AccountSecretKey,
                accessKeySecret = _aiTokenOptions.AccountSecretKey,
                token = token,
                expireTime = expireTime
            };

        }
    }
}
