using System;
using System.IO;
using Common.Captcha.Manager.ApplicationServics;
using Common.Captcha.Manager.ApplicationServics.Image;
using Common.Captcha.Manager.ApplicationServics.Storege;
using Common.Captcha.Manager.Config.ConfigObject;
using Common.Captcha.Manager.Config.Enum;
using Common.Captcha.Manager.Config.IServices.Image;
using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.AspNetCore.Http;

namespace Common.Captcha.Manager.Config.Entity
{
    public class CaptchaOptions
    {
        public CaptchaNoise CaptchaNoise { get; set; } = new();
        public CaptchaType CaptchaType { get; set; } = CaptchaType.LetterAndNumber;
        public int CaptchaLength { get; set; } = 5;
        public CaptchaImageParams CaptchaImageParams { get; set; } = new();
        public SameSiteMode SameSiteMode { get; set; } = SameSiteMode.Strict;
        public string? EncryptionKey { get; set; }
        public int AbsoluteExpirationMinutes { get; set; } = 7;
        public Type? CaptchaStorageProvider { get; set; }
        public Type? CaptchaImageProvider { get; set; }
        /// <summary>
        /// You can introduce a custom font here.
        /// </summary>
        public string? CustomFontPath { get; set; }
        public CaptchaOptions AbsoluteExpiration(int minutes)
        {
            AbsoluteExpirationMinutes = minutes;

            return this;
        }
        public CaptchaOptions WithEncryptionKey(string key)
        {
            EncryptionKey = key;

            return this;
        }
        public CaptchaOptions WithNoise(float pixelsDensity, int linesCount)
        {
            CaptchaNoise = new CaptchaNoise
            {
                NoiseLinesCount = linesCount,
                NoisePixelsDensity = pixelsDensity
            };
            return this;
        }
        public CaptchaOptions UseCaptchaImageProvider<T>() where T : ICaptchaImageProvider
        {
            CaptchaImageProvider = typeof(T);
            return this;
        }
        public CaptchaOptions UseCustomStorageProvider<T>() where T : ICaptchaStorageProvider
        {
            CaptchaStorageProvider = typeof(T);
            return this;
        }
        public CaptchaOptions UseCookieStorageProvider(SameSiteMode sameSite = SameSiteMode.Strict)
        {
            SameSiteMode = sameSite;
            CaptchaStorageProvider = typeof(CookieCaptchaStorageProvider);
            return this;
        }
        public CaptchaOptions UseMemoryCacheStorageProvider()
        {
            CaptchaStorageProvider = typeof(MemoryCacheCaptchaStorageProvider);
            return this;
        }
        public CaptchaOptions UseCustomFont(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"`{fullPath}` file not found!");
            }

            CustomFontPath = fullPath;
            return this;
        }

    }
}
