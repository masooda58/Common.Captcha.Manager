using System;
using Common.Captcha.Manager.Config.Entity;
using Common.Captcha.Manager.Config.IServices.Crypto;
using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Captcha.Manager.ApplicationServics.Storege
{
   public class CookieCaptchaStorageProvider:ICaptchaStorageProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ILogger<CookieCaptchaStorageProvider> _logger;
        private readonly CaptchaOptions _options;

        /// <summary>
        /// Represents the storage to save the captcha tokens.
        /// </summary>
        public CookieCaptchaStorageProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            ILogger<CookieCaptchaStorageProvider> logger,
            IOptions<CaptchaOptions> options)
        {
            _captchaProtectionProvider = captchaProtectionProvider ?? throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;

            _logger.LogDebug("Using the CookieCaptchaStorageProvider.");
        }
        public void Add(HttpContext context, string token, string value)
        {
            value = _captchaProtectionProvider.Encrypt($"{value}{context.GetSalt(_captchaProtectionProvider)}");
            context.Response.Cookies.Append(token, value, getCookieOptions(context));
        }

        public bool Contains(HttpContext context, string token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Request.Cookies.ContainsKey(token);
        }

        public string? GetValue(HttpContext context, string token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Request.Cookies.TryGetValue(token, out var cookieValue))
            {
                _logger.LogDebug("Couldn't find the captcha cookie in the request.");
                return null;
            }

            Remove(context, token);

            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                _logger.LogDebug("Couldn't find the captcha cookie's value in the request.");
                return null;
            }

            var decryptedValue = _captchaProtectionProvider.Decrypt(cookieValue);
            return decryptedValue?.Replace(context.GetSalt(_captchaProtectionProvider), string.Empty, StringComparison.Ordinal);
        }

        public void Remove(HttpContext context, string token)
        {
            if (Contains(context, token))
            {
                context.Response.Cookies.Delete(token, getCookieOptions(context));
            }
        }
        private CookieOptions getCookieOptions(HttpContext context)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Path = context.Request.PathBase.HasValue ? context.Request.PathBase.ToString() : "/",
                Secure = context.Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_options.AbsoluteExpirationMinutes),
                IsEssential = true,
                SameSite = _options.SameSiteMode
            };
        }
    }
}
