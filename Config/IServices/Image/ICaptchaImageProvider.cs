using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Captcha.Manager.ApplicationServics.Image;

namespace Common.Captcha.Manager.Config.IServices.Image
{
    public interface ICaptchaImageProvider
    {
        byte[] DrawCaptcha(string captcha, CaptchaImageParams captchaImageParams);
    }
}
