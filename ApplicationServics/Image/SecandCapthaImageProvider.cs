using Common.Captcha.Manager.Config.IServices.Image;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Common.Captcha.Manager.ApplicationServics.Image
{
    public class SecandCapthaImageProvider : ICaptchaImageProvider
    {
        public byte[] DrawCaptcha(string captcha, CaptchaImageParams captchaImageParams)
        {
            Bitmap bm = new Bitmap(captchaImageParams.Width, captchaImageParams.Height);
            Graphics gr = Graphics.FromImage(bm);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            RectangleF recF = new RectangleF(0, 0, captchaImageParams.Width, captchaImageParams.Height);
            Brush br;
            var fColor = System.Drawing.ColorTranslator.FromHtml(captchaImageParams.ForeColor);
            var bColor = System.Drawing.ColorTranslator.FromHtml(captchaImageParams.BackColor);
            br = new HatchBrush(HatchStyle.SmallConfetti, fColor, bColor);
            gr.FillRectangle(br, recF);
            SizeF text_size;
            Font the_font;
            float font_size = captchaImageParams.Width + 1;
            do
            {
                font_size -= 1;
                the_font = new Font(captchaImageParams.FontName, font_size, FontStyle.Bold, GraphicsUnit.Pixel);
                text_size = gr.MeasureString(captcha, the_font);
            }
            while ((text_size.Width > captchaImageParams.Width) || (text_size.Height > captchaImageParams.Height));
            // Center the text.
            StringFormat string_format = new StringFormat();
            string_format.Alignment = StringAlignment.Center;
            string_format.LineAlignment = StringAlignment.Center;

            // Convert the text into a path.
            GraphicsPath graphics_path = new GraphicsPath();
            graphics_path.AddString(captcha, the_font.FontFamily, 1, the_font.Size, recF, string_format);

            //Make random warping parameters.
            Random rnd = new Random();
            PointF[] pts =
            {
                new PointF
                    ((float)rnd.Next(captchaImageParams.Width) / 4, (float)rnd.Next(captchaImageParams.Height) / 4),
                new PointF(captchaImageParams.Width - (float)rnd.Next(captchaImageParams.Width) / 4, (float)rnd.Next(captchaImageParams.Height) / 4), 
                new PointF((float)rnd.Next(captchaImageParams.Width) / 4, captchaImageParams.Height - (float)rnd.Next(captchaImageParams.Height) / 4), 
                new PointF(captchaImageParams.Width - (float)rnd.Next(captchaImageParams.Width) / 4, captchaImageParams.Height - (float)rnd.Next(captchaImageParams.Height) / 4)
            };
            Matrix mat = new Matrix();
            graphics_path.Warp(pts, recF, mat, WarpMode.Perspective, 0);

            // Draw the text.
            br = new HatchBrush(HatchStyle.LargeConfetti, Color.LightGray, Color.DarkGray);
            gr.FillPath(br, graphics_path);

            // Mess things up a bit.
            int max_dimension = System.Math.Max(captchaImageParams.Width, captchaImageParams.Height);
            for (int i = 0; i <= (int)captchaImageParams.Width * captchaImageParams.Height / 30; i++)
            {
                int X = rnd.Next(captchaImageParams.Width);
                int Y = rnd.Next(captchaImageParams.Height);
                int W = (int)rnd.Next(max_dimension) / 50;
                int H = (int)rnd.Next(max_dimension) / 50;
                gr.FillEllipse(br, X, Y, W, H);
            }
            for (int i = 1; i <= 5; i++)
            {
                int x1 = rnd.Next(captchaImageParams.Width);
                int y1 = rnd.Next(captchaImageParams.Height);
                int x2 = rnd.Next(captchaImageParams.Width);
                int y2 = rnd.Next(captchaImageParams.Height);
                gr.DrawLine(Pens.DarkGray, x1, y1, x2, y2);
            }
            for (int i = 1; i <= 5; i++)
            {
                int x1 = rnd.Next(captchaImageParams.Width);
                int y1 = rnd.Next(captchaImageParams.Height);
                int x2 = rnd.Next(captchaImageParams.Width);
                int y2 = rnd.Next(captchaImageParams.Height);
                gr.DrawLine(Pens.LightGray, x1, y1, x2, y2);
            }
            graphics_path.Dispose();
            br.Dispose();
            the_font.Dispose();
            gr.Dispose();
            MemoryStream ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }

}
