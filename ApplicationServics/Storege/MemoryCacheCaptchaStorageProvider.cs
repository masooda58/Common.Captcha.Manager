using System;
using Common.Captcha.Manager.Config.Entity;
using Microsoft.Extensions.Caching.Memory;
using Common.Captcha.Manager.Config.IServices.Crypto;
using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Captcha.Manager.ApplicationServics.Storege
{
    public class MemoryCacheCaptchaStorageProvider : ICaptchaStorageProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ILogger<MemoryCacheCaptchaStorageProvider> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly CaptchaOptions _options;

        /// <summary>
        /// Represents the storage to save the captcha tokens.
        /// </summary>
        public MemoryCacheCaptchaStorageProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            IMemoryCache memoryCache,
            ILogger<MemoryCacheCaptchaStorageProvider> logger,
            IOptions<CaptchaOptions> options)
        {
            _captchaProtectionProvider = captchaProtectionProvider ??
                                         throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;

            _logger.LogDebug("Using the MemoryCacheCaptchaStorageProvider.");
        }

        public void Add(HttpContext context, string token, string value)
        {
            value = _captchaProtectionProvider.Encrypt($"{value}{context.GetSalt(_captchaProtectionProvider)}");
            _memoryCache.Set(token, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_options.AbsoluteExpirationMinutes),
                Size = 1 // the size limit is the count of entries
            });
        }

        public bool Contains(HttpContext context, string token)
        {
            return _memoryCache.TryGetValue(token, out string _);
        }

        public string? GetValue(HttpContext context, string token)
        {
            if (!_memoryCache.TryGetValue(token, out string cookieValue))
            {
                _logger.LogDebug("Couldn't find the captcha cookie in the request.");
                return null;
            }

            _memoryCache.Remove(token);
            var decryptedValue = _captchaProtectionProvider.Decrypt(cookieValue);
            return decryptedValue?.Replace(context.GetSalt(_captchaProtectionProvider), string.Empty,
                StringComparison.Ordinal);
        }

        public void Remove(HttpContext context, string token)
        {
            _memoryCache.Remove(token);
        }
    }
}
