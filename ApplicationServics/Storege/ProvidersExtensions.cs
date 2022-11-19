using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Captcha.Manager.Config.IServices.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Common.Captcha.Manager.ApplicationServics.Storege
{
    public static  class ProvidersExtensions
    {
        public static string GetSalt(this HttpContext context, ICaptchaCryptoProvider captchaProtectionProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (captchaProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(captchaProtectionProvider));
            }

            var userAgent = (string)context.Request.Headers[HeaderNames.UserAgent];
            var issueDate = DateTime.Now.ToString("yyyy_MM_dd", CultureInfo.InvariantCulture);
            var name = typeof(ProvidersExtensions).Name;
            var salt = $"::{issueDate}::{name}::{userAgent}";
            return captchaProtectionProvider.Hash(salt).HashString;
        }
    }
}
