using Framework.Infrastructure.Interfaces.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Framework.Infrastructure.Interfaces.Core
{
    public class WorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _distributedCache;

        public WorkContext(IHttpContextAccessor httpContextAccessor, IDistributedCache distributedCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _distributedCache = distributedCache;
        }

        public async Task<long> GetUserId()
        {
            return (await GetUserData())["id"].ToObject<long>();
        }

        public async Task<string> GetUserName()
        {
            return (await GetUserData())["name"].ToObject<string>();
        }

        public async Task<string> GetUserNick()
        {
            return (await GetUserData())["nick"].ToObject<string>();
        }

        public async Task<object> GetUserType()
        {
            return (await GetUserData())["role"];
        }

        public async Task<T> Get<T>(string key)
        {
            return (await GetUserData())[key].ToObject<T>();
        }

        private async Task<JObject> GetUserData()
        {
            var token = _httpContextAccessor.GetHeader("Authorization");
            if (string.IsNullOrEmpty(token)) return null;

            token = token.Replace("Bearer ", "");
            string userKey;
            string userDataString;
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(token));
                var strResult = BitConverter.ToString(result);
                userKey = $"token-key-{strResult.Replace("-", "")}";
            }
            var userDataByte = await _distributedCache.GetAsync(userKey);
            if (userDataByte != null)
            {
                userDataString = Encoding.UTF8.GetString(userDataByte);
                return JsonConvert.DeserializeObject<JObject>(userDataString);
            }

            var json = Jose.JWT.Payload(token);
            var obj = JObject.Parse(json);
            userDataString = JsonConvert.SerializeObject(obj);
            await _distributedCache.SetAsync(userKey, Encoding.UTF8.GetBytes(userDataString), new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(5)));
            return obj;
        }
    }
}
