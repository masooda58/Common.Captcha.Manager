using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Common.Captcha.Manager.Config.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CaptchaType
    {
        [Description("فقط حروف")]
        Letter,
        [Description("فقط عدد")]
        Number,
        [Description("حرف و عدد")]
        LetterAndNumber
    }
}
