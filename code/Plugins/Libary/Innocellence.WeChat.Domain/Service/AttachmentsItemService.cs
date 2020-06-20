using Innocellence.WeChat.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using System.IO;
using Innocellence.WeChat.Domain.Common;
using System.Text.RegularExpressions;
using Innocellence.WeChat.Domain.ViewModel;
using Infrastructure.Core.Logging;
using System.Media;
using Infrastructure.Web.ImageTools;
using System.Drawing;

namespace Innocellence.WeChat.Domain.Service
{
    public class AttachmentsItemService : BaseService<SysAttachmentsItem>, IAttachmentsItemService
    {
        ILogger log = LogManager.GetLogger("AttachmentsItemService");

        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<SysAttachmentsItem, bool>> predicate = a => a.Id == id && a.IsDeleted == false;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

        public T GetByMediaId<T>(string mediaId) where T : IViewModel, new()
        {
            log.Debug("get by media id :{0}", mediaId);
            Expression<Func<SysAttachmentsItem, bool>> predicate = a => a.MediaId.Equals(mediaId, StringComparison.CurrentCultureIgnoreCase) && a.IsDeleted == false;
            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();
            log.Debug("get by media result :{0}", t == null ? "null" : t.Id.ToString());
            return t;
        }

        public List<T> GetList<T>(Expression<Func<SysAttachmentsItem, bool>> predicate) where T : IViewModel, new()
        {
            var list = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n)));
            return list.ToList();
        }

        public SysAttachmentsItem ThumbImageAndInsertIntoDB(AttachmentsItemPostProperty p)
        {
            string saveFullName = p.SaveFullName;
            string fileName = p.FileName;
            string targetFilePath = p.TargetFilePath;
            string uploadFileType = p.UploadFileType;
            string saveDir = Path.Combine(p.ServerPath, targetFilePath);
            FileInfo fi = new FileInfo(Path.Combine(saveDir, saveFullName));
            bool isUpdate = false;
            SysAttachmentsItem itemView = new SysAttachmentsItem();
            if (!string.IsNullOrEmpty(p.ViewId) && !"null".Equals(p.ViewId.ToLower()))
            {
                itemView.Id = int.Parse(p.ViewId);
                isUpdate = true;
            }
            itemView.AppId = p.AppId;
            itemView.AttachmentTitle = fileName;
            itemView.Extension = fi.Extension;
            itemView.FileSize = fi.Length;
            itemView.CreateTime = DateTime.Now;
            itemView.UserId = p.UserName;
            itemView.UserName = p.UserName;
            itemView.AttachmentUrl = Path.Combine(targetFilePath, saveFullName).Replace("\\", "/");
            itemView.MediaId = p.MediaId;
            if (uploadFileType.Contains("video"))
            {
                itemView.Type = (int)AttachmentsType.VIDEO;
                itemView.Description = p.Description;
                //itemView.Duration = GetDuration(Path.Combine(saveDir, saveFullName));
                itemView.ThumbUrl = string.IsNullOrWhiteSpace(p.VideoCoverSrc) ? string.Empty : p.VideoCoverSrc.Replace("\\", "/");
            }
            else if (uploadFileType.Contains("image"))
            {
                itemView.Type = (int)AttachmentsType.IMAGE;
                itemView.ThumbUrl = itemView.AttachmentUrl.Insert(itemView.AttachmentUrl.LastIndexOf('.'), "_T");

                using (Stream fs = fi.Open(FileMode.Open))
                {
                    using (Image tempimage = Image.FromStream(fs, true))
                    {
                        itemView.Width = tempimage.Width;//宽
                        itemView.Height = tempimage.Height;//高
                    }
                }
                //压缩逻辑已放入到Upload 时
                //MakeThumbnailForUploadImage(p.ServerPath, targetFilePath, 1024, 768, "W", saveFullName, itemView);
            }
            else if (uploadFileType.Contains("audio") || uploadFileType.Equals("voice"))
            {
                itemView.Type = (int)AttachmentsType.AUDIO;
                //itemView.Duration = GetDuration(Path.Combine(saveDir, saveFullName));
            }
            else
            {
                itemView.Description = fileName;
                itemView.Type = (int)AttachmentsType.FILE;
            }
            if (isUpdate)
            {
                this.Repository.Update(itemView);
            }
            else
            {
                this.Repository.Insert(itemView);
            }
            return itemView;
        }

        /// <summary>        
        /// 生成大概想要的图片 !+_+      
        /// </summary>        
        /// <param name="originalImagePath">源图路径（物理路径）</param>        
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>        
        /// <param name="width">缩略图宽度</param>        
        /// <param name="height">缩略图高度</param>        
        /// <param name="mode">生成缩略图的方式</param>   
        /// <param name="saveFullName">文件的名字</param>           
        private void MakeThumbnailForUploadImage(string serverPath, string targetFilePath, int width, int height, string mode, string saveFullName, SysAttachmentsItem itemView)
        {
            bool isGetThrumbImage = saveFullName.Contains("_t");
            string originalImagePath = Path.Combine(serverPath, targetFilePath.Trim('/'));
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(isGetThrumbImage ? Path.Combine(originalImagePath, saveFullName).Replace("_t", "") : Path.Combine(originalImagePath, saveFullName));
            if (!isGetThrumbImage)
            {
                itemView.Width = originalImage.Width;
                itemView.Height = originalImage.Height;
            }
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            if (ow > width || isGetThrumbImage)
            {
                switch (mode)
                {
                    case "HW":
                        //指定高宽缩放（可能变形）                                    
                        break;
                    case "W":
                        //指定宽，高按比例                                        
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H":
                        //指定高，宽按比例                    
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut":
                        //指定高宽裁减（不变形）                                    
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }
                //新建一个bmp图片            
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
                //新建一个画板            
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法            
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度            
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充            
                g.Clear(System.Drawing.Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分            
                g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                    new System.Drawing.Rectangle(x, y, ow, oh),
                    System.Drawing.GraphicsUnit.Pixel);
                try
                {
                    //以jpg格式保存缩略图  
                    originalImage.Dispose();//释放原图，保存的时候覆盖原图
                    bitmap.Save(Path.Combine(originalImagePath, saveFullName), System.Drawing.Imaging.ImageFormat.Png);

                    if (isGetThrumbImage)
                    {
                        itemView.ThumbUrl = Path.Combine(targetFilePath, saveFullName).Replace("\\", "/");
                    }
                    else
                    {
                        itemView.Width = towidth;
                        itemView.Height = toheight;
                        itemView.FileSize = new FileInfo(Path.Combine(originalImagePath, saveFullName)).Length;
                    }
                }
                catch (System.Exception e)
                {
                    throw;
                }
                finally
                {
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
            if (!isGetThrumbImage)
            {
                //获得宽200的缩略图
                //以_t标记
                string[] temp = saveFullName.Split('.');
                temp[0] = temp[0] + "_t";
                saveFullName = string.Join(".", temp);
                MakeThumbnailForUploadImage(serverPath, targetFilePath, 200, 200, "W", saveFullName, itemView);
            }
        }

        private string FileSizeFormat(long contentLength)
        {
            string fileSize = string.Empty;
            decimal fileContent = new decimal((float)contentLength / 1024);
            if (fileContent / 1024 > 1)
            {
                fileSize = ((float)(fileContent / 1024)).ToString("#0.00") + "MB";
            }
            else
            {
                fileSize = fileContent.ToString("#0.0") + "KB";
            }
            return fileSize;
        }

        /// <summary>
        /// 由于发布到服务器上暂时获取不到时长, 该方法暂时弃用
        /// 具体时长验证留给微信服务器, 我们只需要在页面提示或者弹出错误提示.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        //private string GetDuration(string filePath)
        //{
        //    return string.Empty;
        //    log.Debug("filePath :{0}", filePath);
        //    string dirName = Path.GetDirectoryName(filePath);
        //    log.Debug("dirName :{0}", dirName);
        //    string SongName = Path.GetFileName(filePath);
        //    log.Debug("SongName :{0}", SongName);
        //    FileInfo fInfo = new FileInfo(filePath);
        //    ShellClass sh = new ShellClass();
        //    Folder dir = sh.NameSpace(dirName);
        //    FolderItem item = dir.ParseName(SongName);
        //    string duration = Regex.Match(dir.GetDetailsOf(item, 27), "\\d:\\d{2}:\\d{2}").Value;//获取歌曲时间 
        //    if (string.IsNullOrEmpty(duration))
        //    {
        //        throw new Exception("上传的文件没有时长, 请检查");
        //    }
        //    log.Debug("duration :{0}", duration);
        //    string[] temp = duration.Split(':');
        //    int hour = 0;
        //    int min = 0;
        //    int second = 0;
        //    if (int.TryParse(temp[0], out hour))
        //    {
        //        hour = hour * 60 * 60;
        //    }
        //    if (int.TryParse(temp[1], out min))
        //    {
        //        min = min * 60;
        //    }
        //    int.TryParse(temp[2], out second);
        //    log.Debug("hour :{0}, min :{1}, second :{2}", hour, min, second);
        //    int senconds = hour + min + second;
        //    return senconds.ToString();
        //}

        public int UpdateMediaId(int fileId, string mediaId, DateTime MediaCreateTime)
        {
            var result = 0;
            var entity = new SysAttachmentsItem() { Id = fileId, MediaId = mediaId, MediaExpireTime = MediaCreateTime.AddDays(3) };

            // entity.MediaId = mediaId;
            result = Repository.Update(entity, new List<string>() { "MediaId", "MediaExpireTime" });


            return result;
        }
    }
}
