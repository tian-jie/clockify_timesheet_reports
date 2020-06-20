using Infrastructure.Core.Logging;
using Innocellence.Activity.ViewModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

/*
 * 
 * 中文字号VS英文字号(磅)VS像素值的对应关系：
    八号＝5磅(7px) ==(5/72)*96=6.67 =6px
    七号＝5.5磅 ==(5.5/72)*96=7.3 =7px
    小六＝6.5磅 ==(6.5/72)*96=8.67 =8px
    六号＝7.5磅 ==(7.5/72)*96=10px
    小五＝9磅 ==(9/72)*96=12px
    五号＝10.5磅 ==(10.5/72)*96=14px
    小四＝12磅 ==(12/72)*96=16px
    四号＝14磅 ==(14/72)*96=18.67 =18px
    小三＝15磅 ==(15/72)*96=20px
    三号＝16磅 ==(16/72)*96=21.3 =21px
    小二＝18磅 ==(18/72)*96=24px
    二号＝22磅 ==(22/72)*96=29.3 =29px
    小一＝24磅 ==(24/72)*96=32px
    一号＝26磅 ==(26/72)*96=34.67 =34px
    小初＝36磅 ==(36/72)*96=48px
    初号＝42磅 ==(42/72)*96=56px
 * 
 * */

namespace Innocellence.Activity.Services
{

    public class CheckinScreenService
    {

        private static float _aspectRatio = 1.8F;
        private static long _pointVariance = 20000;
        private static ILogger _log = LogManager.GetLogger("WeChat");

        public static CheckinScreenConfig CreateCheckinScreenConfig(int width, int height, string word, Font font = null)
        {
            Color background = Color.White;
            Color brushColor = Color.FromArgb(255, 25, 25, 25);

            float pound = 18;
            if (font == null)
            {
                font = new Font("Arial Black", pound);
            }

            float lastPound = pound;
            font = AdjustFontSizeToHeight(height, font, pound, lastPound);

            SolidBrush semiTransBrush = new SolidBrush(brushColor);

            Bitmap img = new Bitmap(width, height);

            var wordSizeFs = new List<RectangleF>();

            MemoryStream ms = new MemoryStream();
            using (Graphics g = Graphics.FromImage(img))
            {
                // 计算总宽度，为了计算中心点
                SizeF fullSizeF = g.MeasureString(word, font);
                g.Clear(Color.White);
                float left = (width - fullSizeF.Width) / 2;
                float top = -(height - height / _aspectRatio);
                g.DrawString(word, font, semiTransBrush, new PointF(left, top));

                // 哎，每个字的宽度加起来要大于总宽度啊，所以只能挨个点找啦
                // 一整列都没有点的，作为一个隔断
                int wordIndex = 0;
                bool lastHasWord = false; // 记录上一列是否处于有字状态

                float currentWordLeft = 0F;
                for (int x = 0; x < width; x++)
                {
                    bool hasWord = false; // 记录当前是否处于有字状态
                    for (int y = 0; y < height; y++)
                    {
                        var pixel = img.GetPixel(x, y);
                        long diff = (pixel.R - brushColor.R) * (pixel.R - brushColor.R) + (pixel.G - brushColor.G) * (pixel.G - brushColor.G) + (pixel.B - brushColor.B) * (pixel.B - brushColor.B);

                        if (diff < _pointVariance)
                        {
                            // 这一列有点，记录有点的情况
                            hasWord = true;
                            break;
                        }
                    }

                    // 刚跑完一列，看这列的情况：
                    // 如果有点，并且上一列没点，则开始新一个字；
                    // 如果有点，并且上一列有点，则继续；
                    // 如果没点，并且上一列没点，则继续；
                    // 如果没点，并且上一列有点，则上一个字结束。
                    if (!(hasWord ^ lastHasWord))
                    {
                        // case 2和3
                        // 如果有点，并且上一列有点，则继续；
                        // 如果没点，并且上一列没点，则继续；
                        continue;
                    }
                    else if (hasWord && !lastHasWord)
                    {
                        // case 1
                        // 如果有点，并且上一列没点，则开始新一个字；
                        currentWordLeft = x;

                    }
                    else if (!hasWord && lastHasWord)
                    {
                        // case 4
                        // 如果没点，并且上一列有点，则上一个字结束。
                        wordSizeFs.Add(new RectangleF(currentWordLeft, 0, x - currentWordLeft+1, height));
                        wordIndex++;
                    }

                    lastHasWord = hasWord;
                }

                //float curLeft = left;
                //foreach (var c in word)
                //{
                //    var sizef = g.MeasureString(c.ToString(), font);
                //    RectangleF r = new RectangleF(new PointF(curLeft, 0), sizef);
                //    wordSizeFs.Add(r);
                //    curLeft += sizef.Width;
                //}

                //img.Save(ms, ImageFormat.MemoryBmp);

                //img.Save("E:\\1.bmp", ImageFormat.Bmp);
                g.Save();
            }

            // 然后从这里开始创建CheckinScreenConfig结构
            var checkinScreenConfig = new CheckinScreenConfig()
                {
                    Width = width,
                    Height = height,
                    Seq = new int[width * height]
                };

            //// 全部清零
            //foreach (var p in checkinScreenConfig.Seq)
            //{
            //    p = 0;
            //}

            int seq = 1;

            // 开始每个字的点进行排序。从left开始到left+width区间的所有点，遍历，增加seq
            foreach (var ws in wordSizeFs)
            {
                for (int y = 0; y < (int)height; y++)
                {
                    for (int x = (int)ws.Left; x < (int)(ws.Left + ws.Width); x++)
                    {
                        _log.Debug("Getting pixel: x={0}, y={1}", x, y);
                        var pixel = img.GetPixel(x, y);
                        // 求方差
                        long diff = (pixel.R - brushColor.R) * (pixel.R - brushColor.R) + (pixel.G - brushColor.G) * (pixel.G - brushColor.G) + (pixel.B - brushColor.B) * (pixel.B - brushColor.B);
                        if (diff < 5000)
                        {
                            checkinScreenConfig.Seq[y * width + x] = seq++;
                        }
                    }
                }
            }

            //using (StreamWriter sw = new StreamWriter("e:\\1.csv"))
            //{
            //    for (int y = 0; y < (int)height; y++)
            //    {
            //        for (int x = 0; x < width; x++)
            //        {
            //            sw.Write(checkinScreenConfig.Seq[y * width + x]);
            //            sw.Write(",");
            //        }
            //        sw.WriteLine();
            //    }
            //}



            return checkinScreenConfig;
        }

        private static Font AdjustFontSizeToHeight(int height, Font font, float pound, float lastPound)
        {
            //计算pound以适应height
            while (font.Height < height)
            {
                lastPound = pound;
                pound += 0.5F;
                font = new Font("Arial Black", pound);
            }
            while (font.Height > height)
            {
                lastPound = pound;
                pound -= 0.5F;
                font = new Font("Arial Black", pound);
            }
            pound *= _aspectRatio;
            font = new Font("Arial Black", pound);
            return font;
        }
    }
}