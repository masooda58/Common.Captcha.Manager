using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Captcha.Manager.Config.Entity;
using Common.Captcha.Manager.Config.IServices.Crypto;
using EasyCaching.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Captcha.Manager.ApplicationServics.Storege
{
   public class EasyCahingStorageProvider : ICaptchaStorageProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ILogger<EasyCahingStorageProvider> _logger;
        private readonly IEasyCachingProviderBase _memoryCache;
        private readonly CaptchaOptions _options;
        public EasyCahingStorageProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            IEasyCachingProviderBase memoryCache,
            ILogger<EasyCahingStorageProvider> logger,
            IOptions<CaptchaOptions> options)
        {
            _captchaProtectionProvider = captchaProtectionProvider ??
                                         throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;

            _logger.LogDebug("Using the EasyCachingStorageProvider.");
        }
        public void Add(HttpContext context, string token, string value)
        {
            value = _captchaProtectionProvider.Encrypt($"{value}{context.GetSalt(_captchaProtectionProvider)}");
            _memoryCache.Set(token, value, TimeSpan.FromMinutes(_options.AbsoluteExpirationMinutes));
        }

        public bool Contains(HttpContext context, string token)
        {
            return _memoryCache.Get<string>(token).HasValue;
        }

        public string GetValue(HttpContext context, string token)
        {
           var cookieValue = _memoryCache.Get<string>(token);
            if (!cookieValue.HasValue)
            {
                _logger.LogDebug("Couldn't find the captcha cookie in the request.");
                return null;
            }

            _memoryCache.Remove(token);
            var decryptedValue = _captchaProtectionProvider.Decrypt(cookieValue.Value);
            return decryptedValue?.Replace(context.GetSalt(_captchaProtectionProvider), string.Empty,
                StringComparison.Ordinal);
        }

        public void Remove(HttpContext context, string token)
        {
            _memoryCache.Remove(token);
        }
    }
}
