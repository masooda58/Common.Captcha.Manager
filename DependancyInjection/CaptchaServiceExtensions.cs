using Common.Captcha.Manager.ApplicationServics;
using Common.Captcha.Manager.ApplicationServics.Crypto;
using Common.Captcha.Manager.ApplicationServics.Image;
using Common.Captcha.Manager.ApplicationServics.Storege;
using Common.Captcha.Manager.Config.Entity;
using Common.Captcha.Manager.Config.IServices.Crypto;
using Common.Captcha.Manager.Config.IServices.Image;
using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Common.Captcha.Manager.DependancyInjection
{
    public static class CaptchaServiceExtensions
    {
        public static void AddCaptcha(
            this IServiceCollection services,
            Action<CaptchaOptions>? options = null)
        {
            configOptions(services, options);
    

           // services.AddScoped<ICaptchaImageProvider, CaptchaImageProvider>();
            services.TryAddSingleton<ICaptchaCryptoProvider, CaptchaCryptoProvider>();
            
            services.AddHttpContextAccessor();


            services.AddScoped<CaptchaService>();
        }

        private static void configOptions(IServiceCollection services, Action<CaptchaOptions>? options)
        {
            var captchaOptions = new CaptchaOptions();
            options?.Invoke(captchaOptions);
            setCaptchaStorageProvider(services, captchaOptions);
            setCaptchaImageProvider(services, captchaOptions);
            //  services.AddScoped<ICaptchaStorageProvider, CookieCaptchaStorageProvider>();
            services.TryAddSingleton(Options.Create(captchaOptions));
        }
        private static void setCaptchaStorageProvider(IServiceCollection services, CaptchaOptions captchaOptions)
        {
            if (captchaOptions.CaptchaStorageProvider == null)
            {
                services.AddScoped<ICaptchaStorageProvider, CookieCaptchaStorageProvider>();
            }
            else
            {
                services.AddScoped(typeof(ICaptchaStorageProvider), captchaOptions.CaptchaStorageProvider);
            }
        }
        private static void setCaptchaImageProvider(IServiceCollection services, CaptchaOptions captchaOptions)
        {
            if (captchaOptions.CaptchaImageProvider == null)
            {
                services.AddScoped<ICaptchaImageProvider, CaptchaImageProvider>();
            }
            else
            {
                services.AddScoped(typeof(ICaptchaImageProvider), captchaOptions.CaptchaImageProvider);
            }

        }
    }
}
