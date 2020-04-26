using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Flurl.Http;
using Framework.Common.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Framework.Infrastructure.Middleware.Core
{
    public class ApiSubActionFilter : IAsyncActionFilter
    {
        private readonly string _url;
        public ApiSubActionFilter(IConfigurationRoot configurationRoot)
        {
            _url = configurationRoot["SP_BasicSetting:Fabio"];
        }

        private static string BuildParamStr(Dictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
            {
                return "";
            }
            var ascDic = param.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            var sb = new StringBuilder();
            foreach (var item in ascDic)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    sb.Append(item.Key.ToUnderscoreCase()).Append("=").Append(item.Value).Append("&");
                }
            }
            return sb.ToString().Substring(0, sb.ToString().Length - 1);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Path.HasValue)
            {
                var path = context.HttpContext.Request.Path.Value.ToLower(); ;
                if (path.IndexOf("/open.", StringComparison.Ordinal) > -1)
                {
                    var appKey = context.HttpContext.GetHeader("app_key");
                    var appSecret = context.HttpContext.GetHeader("app_secret");
                    var timeStamp = context.HttpContext.GetHeader("time_stamp");
                    var sign = context.HttpContext.GetHeader("sign");
                    var dic = new Dictionary<string, string>
                    {
                        {"appKey", appKey}, {"appSecret", appSecret}, {"dateTime", timeStamp}
                    };
                    var preSign = BuildParamStr(dic);
                    if (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret) || string.IsNullOrEmpty(timeStamp) || string.IsNullOrEmpty(sign))
                    {
                        var data = new { code = 401, is_success = false, msg = "公共参数不能为空" };
                        context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
                        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(data), Encoding.UTF8);
                        return;
                    }
                    var result = await $"{_url}/school_precaution_org/inside.org.merchant.valid?appKey={HttpUtility.UrlEncode(appKey)}&appSecret={HttpUtility.UrlEncode(appSecret)}&preSign={HttpUtility.UrlEncode(preSign)}&sign={HttpUtility.UrlEncode(sign)}".GetJsonAsync<SubmitResult>();
                    if (result.IsSuccess)
                    {
                        context.HttpContext.Items.Add(new KeyValuePair<object, object>("orgId", result.Data));
                        await next();
                        return;
                    }
                    else
                    {
                        var data = new { code = 401, is_success = false, msg = result.Message };
                        context.HttpContext.Response.ContentType = "application/json;charset=utf-8";
                        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(data), Encoding.UTF8);
                        return;
                    }
                }
            }
            await next();
        }
    }
}
