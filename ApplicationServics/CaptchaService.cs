using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Captcha.Manager.ApplicationServics.Image;
using Common.Captcha.Manager.Config.Entity;
using Common.Captcha.Manager.Config.Enum;
using Common.Captcha.Manager.Config.IServices.Image;
using Common.Captcha.Manager.Config.IServices.Storege;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Common.Captcha.Manager.ApplicationServics
{
   public class CaptchaService
   {
       private readonly CaptchaOptions _options;
       private readonly ICaptchaImageProvider _captchaImage;
       private readonly ICaptchaStorageProvider _captchaStorageProvider;
       private readonly IHttpContextAccessor _httpContextAccessor;


       public CaptchaService( IOptions<CaptchaOptions> options, 
           ICaptchaImageProvider captchaImage,
           ICaptchaStorageProvider captchaStorageProvider, IHttpContextAccessor httpContextAccessor)
       {
           _captchaImage = captchaImage;
           _captchaStorageProvider = captchaStorageProvider;
           _httpContextAccessor = httpContextAccessor;

           _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
       }

       private string GenerateStringCaptchaCode()
       {
           string letters = _options.CaptchaType switch
           {
               CaptchaType.Letter => "ABCDEFGHJKLMNPRTUVWXYZ",
               CaptchaType.Number => "2346789",
               CaptchaType.LetterAndNumber => "2346789ABCDEFGHJKLMNPRTUVWXYZ",
               _ => throw  new ArgumentNullException($"نوع کپچا معتبر نیست")
           };
            
           Random rand = new Random();
           int maxRand = letters.Length;

           StringBuilder sb = new StringBuilder();

           for (int i = 0; i < _options.CaptchaLength-1; i++)
           {
               int index = rand.Next(maxRand);
               sb.Append(letters[index]);
           }

           return sb.ToString();
       }

       public byte[] GenerateCaptchaImage(string token,CaptchaImageParams? imageParams=null)
       {
           string captcha = GenerateStringCaptchaCode();
           _captchaStorageProvider.Add(_httpContextAccessor.HttpContext,token,captcha);

           if (imageParams == null)
           {
               return _captchaImage.DrawCaptcha(captcha, _options.CaptchaImageParams);
           }
           
           return _captchaImage.DrawCaptcha(captcha,imageParams);
       }

       public bool CaptchValidator(string captcha,string token)
       {
          return (captcha == _captchaStorageProvider.GetValue(_httpContextAccessor.HttpContext, token)) ;
       }
   }
}
