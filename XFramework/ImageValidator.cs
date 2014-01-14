using System;
using System.Drawing;

namespace XFramework
{
    /// <summary>
    /// 图片验证码
    /// </summary>
    public class ImageValidator
    {
        #region 属性

        /// <summary>
        /// 验证码的字符类型，0-纯数字，1-数字加字母
        /// </summary>
        public int CharType = 1;

        /// <summary>
        /// 验证码的长度
        /// </summary>
        public int Length = 4;

        /// <summary>
        /// 生成图片验证码的宽度
        /// </summary>
        public int ImgWidth = 65;

        /// <summary>
        /// 生成图片验证码的长度
        /// </summary>
        public int ImgHeight = 28;

        //随机的数字  
        char[] NumberGroup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        //随机的字母，不包含字母i,l,o
        char[] LetterGroup = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造行数
        /// </summary>
        public ImageValidator()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="charType">验证码的字符类型，0-纯数字，1-数字加字母；默认1</param>
        /// <param name="length">验证码的长度；默认4</param>
        public ImageValidator(int charType, int length)
        {
            CharType = charType;
            Length = length;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="charType">验证码的字符类型，0-纯数字，1-数字加字母；默认1</param>
        /// <param name="length">验证码的长度；默认4</param>
        /// <param name="imgWidth">生成图片验证码的宽度；默认65px</param>
        /// <param name="imgHeight">生成图片验证码的高度；默认28px</param>
        public ImageValidator(int charType, int length, int imgWidth, int imgHeight)
        {
            CharType = charType;
            Length = length;
            ImgWidth = imgWidth;
            ImgHeight = imgHeight;
        }

        #endregion

        #region 获取验证码图片

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Bitmap GenerateImage()
        {
            string s = GetVerifyCode();

            Bitmap bitmap = new Bitmap(ImgWidth, ImgHeight);

            StringFormat format = new StringFormat(StringFormatFlags.NoClip) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            Graphics g = Graphics.FromImage(bitmap);

            #region 背景加噪点

            Color color = Color.FromArgb(250, 250, 250);

            Brush brush = new SolidBrush(color);

            g.FillRectangle(brush, 0, 0, ImgWidth, ImgHeight);

            #endregion

            #region 画背景干扰线

            color = GetEllipseColor();

            brush = new SolidBrush(color);

            Pen pen = new Pen(brush, 2);

            int mixMax = GetRandomNum(15, 20);

            for (int i = 0; i < mixMax; i++)
            {
                g.DrawEllipse(pen, GetRandomNum(1, ImgWidth), GetRandomNum(1, ImgHeight), GetRandomNum(1, ImgWidth), GetRandomNum(1, ImgHeight));
            }

            #endregion

            #region 画文字和阴影文字

            //随机转动角度
            const int randAngle = 35;

            //字体
            Font fontCode = new Font("仿宋", 23, FontStyle.Bold, GraphicsUnit.Pixel), fontShadow = new Font("微软雅黑", 9, FontStyle.Italic, GraphicsUnit.Pixel);

            //画笔
            SolidBrush brushCode = new SolidBrush(GetCodeColor()), brushShadow = new SolidBrush(GetShadowColor());

            //转动的角度
            float angleCode = 0, angleShadow = 0;

            //画笔起始点
            Point point = new Point(10, 12);

            //字符在画布上的起始点
            int x = 1, y = 0;

            for (int i = 0; i < Length; i++)
            {
                y = GetRandomNum(2, 4);

                //文字的转动度数
                angleCode = GetRandomNum(-randAngle, randAngle);

                //阴影文字的转动度数
                angleShadow = GetRandomNum(-10, 10);

                if (i == 0 && angleCode > 0) angleCode = -angleCode;

                //移动光标到指定位置
                g.TranslateTransform(point.X, point.Y);

                #region 画文字和阴影文字

                g.RotateTransform(angleCode);

                g.DrawString(s[i].ToString(), fontShadow, brushShadow, x + 1, y + 1, format);

                g.RotateTransform(angleShadow);
                g.DrawString(s[i].ToString(), fontCode, brushCode, x, y, format);
                g.RotateTransform(-angleShadow);

                g.RotateTransform(-angleCode);

                #endregion

                //移动光标到指定位置
                g.TranslateTransform(4, -point.Y);
            }

            #endregion

            return bitmap;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取文字颜色
        /// </summary>
        /// <returns></returns>
        private Color GetCodeColor()
        {
            Color[] colorGroup = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.FromArgb(251, 137, 3), Color.Brown, Color.DarkCyan, Color.Purple };

            return colorGroup[GetRandomNum(0, colorGroup.Length)];
        }

        /// <summary>
        /// 获取阴影文字颜色
        /// </summary>
        /// <returns></returns>
        private Color GetShadowColor()
        {
            return Color.FromArgb(GetRandomNum(0, 256), GetRandomNum(0, 256), GetRandomNum(0, 256));
        }

        /// <summary>
        /// 获取干扰线颜色
        /// </summary>
        /// <returns></returns>
        private Color GetEllipseColor()
        {
            return Color.FromArgb(GetRandomNum(200, 256), GetRandomNum(200, 256), GetRandomNum(200, 256));
        }

        /// <summary>
        /// 获取指定长度的随机字符字符串
        /// </summary>
        /// <returns></returns>
        private string GetVerifyCode()
        {
            string rtnRst = string.Empty;

            do
            {
                rtnRst += GetChar(CharType);

            } while (rtnRst.Length < Length);

            return rtnRst;
        }

        /// <summary>
        /// 获取随机字符
        /// </summary>
        /// <param name="vCodeType"></param>
        /// <returns></returns>
        private char GetChar(int vCodeType)
        {
            if (vCodeType == 0)
            {
                return NumberGroup[GetRandomNum(0, NumberGroup.Length)];
            }
            else
            {
                if (GetRandomNum(0, 100) < 50) return GetChar(0);
                else return LetterGroup[GetRandomNum(0, LetterGroup.Length)];
            }
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="min">随机最小值</param>
        /// <param name="max">随机最大值（结果不包含此值）</param>
        /// <returns></returns>
        private int GetRandomNum(int min, int max)
        {
            int seed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

            Random random = new Random(seed);

            return random.Next(min, max);
        }

        #endregion
    }
}