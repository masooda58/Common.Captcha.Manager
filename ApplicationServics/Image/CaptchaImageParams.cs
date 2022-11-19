using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Captcha.Manager.ApplicationServics.Image
{
    /// <summary>
    /// Captcha's Image Params
    /// </summary>
    [Serializable]
    public class CaptchaImageParams
    {
        /// <summary>
        /// The encrypted text
        /// </summary>
        public string Text { set; get; } = default!;

        /// <summary>
        /// A random number
        /// </summary>
        public string RndDate { set; get; } = default!;

        /// <summary>
        /// ForeColor of the captcha's text
        /// </summary>
        public string ForeColor { set; get; } = "#1B0172";

        /// <summary>
        /// BackColor of the captcha's text
        /// </summary>
        public string BackColor { set; get; } = "";

        /// <summary>
        /// FontSize of the captcha's text
        /// </summary>
        public float FontSize { set; get; } = 12;

        /// <summary>
        /// FontName of the captcha's text
        /// </summary>
        public string FontName { set; get; } = "Tahoma";

        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
    }
}

