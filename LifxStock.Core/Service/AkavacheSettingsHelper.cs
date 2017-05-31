using Akavache;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LifxStock.Core.Service
{
    public class AkavacheSettingsHelper
    {
        private static readonly string ApplicationName = "LifxStockmonitor";

        public AkavacheSettingsHelper()
        {
            BlobCache.ApplicationName = ApplicationName;
        }
        
        public async Task<string> GetLifxToken()
        {
            try
            {
                var loginInfo = await BlobCache.Secure.GetLoginAsync("lifxHost");
                return loginInfo.Password;
            }
            catch
            {
                return string.Empty;
            }
        }
        
        public async Task SaveLifxToken(string value)
        {
            await BlobCache.Secure.SaveLogin("user", value, "lifxHost");
        }
    }
}