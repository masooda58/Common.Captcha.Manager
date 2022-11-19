using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Captcha.Manager.Config.ConfigObject
{
    /// <summary>
    /// Defines options of the captcha's noise
    /// </summary>
    public class CaptchaNoise
    {
        /// <summary>
        /// Defines density of the noise's pixels.
        /// Its default value is 25.
        /// </summary>
        public float NoisePixelsDensity { set; get; } = 25;

        /// <summary>
        /// Defines the number of random noise's lines.
        /// Its default value is 3.
        /// </summary>
        public int NoiseLinesCount { set; get; } = 3;
    }
}
