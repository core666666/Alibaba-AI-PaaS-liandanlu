using AlibabaCloud.SDK.Dingtalkai_paa_s_1_0.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        private readonly IHttpClientFactory _httpClientFactory;

        public AiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
        public static void getAccessToken()
        {
            AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client client = CreateClient();
            AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest getAccessTokenRequest = new AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest
            {
                //这里填写AppKey和AppSecret
                AppKey = "",
                AppSecret = "",
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
                // 直接返回 response.Body.Result。
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
            string appKey = "";
            string token = "";

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
    }
}
