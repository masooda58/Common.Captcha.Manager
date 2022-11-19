using Common.Captcha.Manager.Config.Entity;
using Common.Captcha.Manager.Config.IServices.Image;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Common.Captcha.Manager.ApplicationServics.Image
{
    public class ThirdCaptchaImagProvider : ICaptchaImageProvider
    {
        private const int Margin = 10;
        private readonly CaptchaOptions _options;
        private Font? _font;
        private readonly Random _rand;


        public ThirdCaptchaImagProvider(IOptions<CaptchaOptions> options)
        {
            _options = options.Value;
            _rand = new Random();
        }

        public byte[] DrawCaptcha(string captcha, CaptchaImageParams captchaImageParams)
        {
            _font ??= getFont(captchaImageParams.FontName, captchaImageParams.FontSize);
            Bitmap bm = new Bitmap(captchaImageParams.Width, captchaImageParams.Height);
            Graphics gr = Graphics.FromImage(bm);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            var (width, height) = getImageSize(gr, captcha);
            var fColor = System.Drawing.ColorTranslator.FromHtml(captchaImageParams.ForeColor);
            var bColor = System.Drawing.ColorTranslator.FromHtml(captchaImageParams.BackColor);



            drawText(gr, captcha, captchaImageParams.Width, captchaImageParams.Height, fColor, bColor);
            DrawDisorderLine(gr, captchaImageParams.Width, captchaImageParams.Height);
            var bitMapByte = saveAsPng(bm);
            gr.Dispose();
            bm.Dispose();
            return bitMapByte ;
            throw new NotImplementedException();
        }

        private void drawText(Graphics gr, string captcha, int width, int height, Color foreColor, Color backColor)
        {
            #region backgrund

            RectangleF recF = new RectangleF(0, 0, width, height);
            Brush br = new HatchBrush(HatchStyle.SmallConfetti, foreColor, backColor);
            gr.FillRectangle(br, recF);

            #endregion

            #region Captcha draw

            // Center the text.
            //StringFormat string_format = new StringFormat();
            //string_format.Alignment = StringAlignment.Center;
            //string_format.LineAlignment = StringAlignment.Center;
            //// Convert the text into a path.
            //GraphicsPath graphics_path = new GraphicsPath();
            ////get siz match to box
           
           
            float fontSize = (width - Margin)/captcha.Length ;
            //do
            //{
            //    font_size -= 1;
            //    the_font = new Font(_font.FontFamily.Name, font_size, FontStyle.Bold, GraphicsUnit.Pixel);
            //    text_size = gr.MeasureString(captcha, the_font);
            //} while ((text_size.Width > width-Margin) || (text_size.Height > height-Margin));
            //graphics_path.AddString(captcha, _font.FontFamily, 1, font_size, recF, string_format);
            //// cose color

            //// Draw the text.
            //br = new HatchBrush(HatchStyle.Cross, foreColor, backColor);
            //gr.FillPath(br, graphics_path);
            SolidBrush fontBrush = new SolidBrush(Color.Black);
            Font the_font=new Font(_font.FontFamily,fontSize,FontStyle.Bold);
            for (int i = 0; i < captcha.Length; i++)
            {
                fontBrush.Color = GetRandomDeepColor();

                int shiftPx = (int)fontSize / 6;

                float x = i * fontSize;
                //+ _rand.Next(-shiftPx, shiftPx) + _rand.Next(-shiftPx, shiftPx);
                int maxY = (int)(height - fontSize-Margin);
                if (maxY < 0) maxY = 0;
                float y = _rand.Next(Margin, maxY);
                
                gr.DrawString(captcha[i].ToString(),the_font, fontBrush, x, y);

                #endregion
            }

            fontBrush.Dispose();
                br.Dispose();
                the_font.Dispose();

                // throw new NotImplementedException();
           
        }
        private Color GetRandomLightColor()
        {
            int low = 180, high = 255;

            int nRend = _rand.Next(high) % (high - low) + low;
            int nGreen = _rand.Next(high) % (high - low) + low;
            int nBlue = _rand.Next(high) % (high - low) + low;

            return Color.FromArgb(nRend, nGreen, nBlue);
        }
        private Color GetRandomDeepColor()
        {
            int redlow = 160, greenLow = 100, blueLow = 160;
            return Color.FromArgb(_rand.Next(redlow), _rand.Next(greenLow), _rand.Next(blueLow));
        }
        private Font getFont(string fontName, float fontSize)
        {

            var fontFamily = SystemFonts.DialogFont.FontFamily;
            return new Font(fontFamily.Name, fontSize);
            //if (string.IsNullOrWhiteSpace(_options.CustomFontPath))
            //{
            //    var fontFamily = SystemFonts.Get(fontName, CultureInfo.InvariantCulture);
            //    return new Font(fontFamily, fontSize);
            //}

            //var fontCollection = new FontCollection();
            //return fontCollection.Add(_options.CustomFontPath, CultureInfo.InvariantCulture).CreateFont(fontSize);

        }
        private (int Width, int Height) getImageSize(Graphics gr, string captcha)
        {
            if (_font is null)
            {
                throw new InvalidOperationException("font is null.");
            }
            //var captchaSize = TextMeasurer.Measure(message, new TextOptions(_font));

            //var width = (int)captchaSize.Width + Margin;
            //var height = (int)captchaSize.Height + Margin;
            SizeF text_size = gr.MeasureString(captcha, _font); ;
            var width = (int)text_size.Width + Margin;
            var height = (int)text_size.Height + Margin;
            return (width, height);
        }
        private byte[] saveAsPng(Bitmap image)
        {
            using var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
        void DrawDisorderLine(Graphics graph,int width,int height)
        {
            Pen linePen = new Pen(new SolidBrush(Color.Black), (float)0.5);
            for (int i = 0; i < _rand.Next(3, 6); i++)
            {
                linePen.Color = GetRandomDeepColor();

                Point startPoint = new Point(_rand.Next(0, width), _rand.Next(0, height));
                Point endPoint = new Point(_rand.Next(0, width), _rand.Next(0,height));
                graph.DrawLine(linePen, startPoint, endPoint);

                //Point bezierPoint1 = new Point(_rand.Next(0, width), _rand.Next(0, height));
                //Point bezierPoint2 = new Point(_rand.Next(0, width), _rand.Next(0, height));

                //graph.DrawBezier(linePen, startPoint, bezierPoint1, bezierPoint2, endPoint);
            }
        }
    }
}
