using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core;
using Infrastructure.Core.Logging;
using System.IO;
using Innocellence.WeChat.Domain.Service;
using System.Linq;
using System.Collections.Generic;
using Innocellence.WeChat.Domain.Common;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using Infrastructure.Web.ImageTools;
//using Shell32;

namespace Innocellence.WeChatMain.Controllers
{
    public class UploadController : Controller
    {
        ILogger _Logger = LogManager.GetLogger("UploadController");

        const string StrJsonTypeTextHtml = "text/html";
        private string UploadVideoFilePath = "Content/UploadFiles/Videos/";
        private string UploadImageFilePath = "Content/UploadFiles/Images/";
        private string UploadAudioFilePath = "Content/UploadFiles/Audio/";
        private string UploadFilePath = "Content/UploadFiles/File/";
        private string UploadFileCombineCharacter = "_";
        private string UploadFileDateTimeFormat = "yyyyMMddHHmm";


        private AttachmentsItemService _attachmentsItemService = new AttachmentsItemService();

        public ActionResult Index()
        {
            return View();
        }

        //[BackofficeAuthorize]
        public ActionResult Uploadfile()
        {
            try
            {
                string thumbNailPath = string.Empty;
                string targetFilePath = string.Empty;
                string serverFileName = string.Empty;
                string duration = string.Empty;
                string realFileName = string.Empty;
                _Logger.Debug("File Count:{0}", Request.Files.Count);
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] != null)
                    {
                        _Logger.Debug("File Name:{0}", Request.Files[i].FileName);
                        FileInfo fi = new FileInfo(Request.Files[i].FileName);
                        realFileName = Request.Files[i].FileName;
                        //serverFileName = Request.Files[i].FileName.Substring(0, Request.Files[i].FileName.IndexOf('.')) + UploadFileCombineCharacter + DateTime.Now.ToString(UploadFileDateTimeFormat) + fi.Extension;
                        serverFileName = Guid.NewGuid().ToString().Replace("-", "") + fi.Extension;
                        _Logger.Debug("serverFileName : {0}", serverFileName);
                        string uploadFileType = Request.QueryString["type"].ToLower();
                        targetFilePath = InitTargetFilePathDir(uploadFileType);
                        _Logger.Debug("targetFilePath : {0}", targetFilePath);
                        //上传原文件并压缩
                        ProcessPostedFile(serverFileName, Request.Files[i], targetFilePath.Trim('/'), uploadFileType);
                        if (uploadFileType.Contains("audio"))
                        {
                            //duration = GetDuration(Path.Combine(Server.MapPath("~/"), targetFilePath.Trim('/'), serverFileName));
                        }
                    }
                    else
                    {
                        return Json(new { success = false, timeout = false },
                            StrJsonTypeTextHtml);
                    }
                }
                return Json(new { success = true, timeout = false, serverFileName = serverFileName, targetFilePath = targetFilePath, duration = duration, realFileName = realFileName },
                    StrJsonTypeTextHtml);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "Upload file failed");
            }
            return Json(new { success = false, timeout = false },
                StrJsonTypeTextHtml);
        }

        private void GenerateNewsCoverThumbImage(string serverFileName, string realFileName, string targetFilePath)
        {
            AttachmentsItemPostProperty p = new AttachmentsItemPostProperty()
            {
                SaveFullName = serverFileName,
                ServerPath = Server.MapPath("~/"),
                TargetFilePath = targetFilePath.Trim('/'),
                UploadFileType = "image",
                AppId = int.Parse(Request["AppId"]),
                IsNewsCover = true
            };
            _attachmentsItemService.ThumbImageAndInsertIntoDB(p);
        }

        private string InitTargetFilePathDir(string uploadFileType)
        {
            CheckUploadFolderPath();
            string targetFilePath = string.Empty;
            switch (uploadFileType)
            {
                case "image":
                    targetFilePath = UploadImageFilePath;
                    break;
                case "video":
                    targetFilePath = UploadVideoFilePath;
                    break;
                case "audio":
                    targetFilePath = UploadAudioFilePath;
                    break;
                case "file":
                default:
                    targetFilePath = UploadFilePath;
                    break;
            }
            string saveDir = Server.MapPath("~/");
            DateTime now = DateTime.Now;
            List<string> fix = new List<string>()
            {
                targetFilePath,
                now.Year.ToString(),
                now.Month.ToString(),
                now.Day.ToString()
            };
            for (int i = 0; i < fix.Count; i++)
            {
                if (i != 0)
                {
                    targetFilePath = Path.Combine(targetFilePath, fix[i]);
                }
                string path = Path.Combine(saveDir, targetFilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            targetFilePath = targetFilePath.Replace("\\", "/");
            return targetFilePath + "/";
        }

        private void CheckUploadFolderPath()
        {
            try
            {
                string uploadFolderName = WebConfigurationManager.AppSettings["UploadFolderName"];
                List<string> fix = new List<string>()
                {
                    "/Content",
                    uploadFolderName,
                    "Videos",
                    "Images",
                    "Audio",
                    "File",
                };
                string path = Server.MapPath("~/");
                try
                {
                    fix.ForEach(f =>
                    {
                        path = Path.Combine(path, f);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                    });
                }
                catch (Exception)
                {

                }
                UploadVideoFilePath = Path.Combine(fix[0], fix[1], fix[2]);
                UploadImageFilePath = Path.Combine(fix[0], fix[1], fix[3]);
                UploadAudioFilePath = Path.Combine(fix[0], fix[1], fix[4]);
                UploadFilePath = Path.Combine(fix[0], fix[1], fix[5]);
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
        }

        private void ProcessPostedFile(string saveFullName, HttpPostedFileBase postedFile, string targetFilePath, string uploadFileType)
        {
            try
            {
                _Logger.Debug("begin to update file to :{0}", targetFilePath);
                string saveDir = Path.Combine(Server.MapPath("~/"), targetFilePath);
                if (Directory.Exists(saveDir) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(saveDir);
                }
                if (postedFile != null)
                {
                    string strPath = Path.Combine(saveDir, saveFullName);
                    if ("image".Equals(uploadFileType, StringComparison.OrdinalIgnoreCase))
                    {
                        //1.压缩原图, 最大尺寸, 宽1024, 不限制高
                        ImageUtility.MakeThumbnail(null, postedFile.InputStream, strPath, 1024, 768, "W");
                        //2.生成200*200的缩略图, _T 结尾
                        ImageUtility.MakeThumbnail(null, postedFile.InputStream, strPath.Insert(strPath.LastIndexOf('.'), "_T"), 200, 200, "Cut");
                        //3.图文封面生成宽900*500的等比图, 由于900*500的容易超过64kg, 等比压缩为360*220
                        bool isNewsCover = false;
                        if (bool.TryParse(Request["isNewsCover"], out isNewsCover) && isNewsCover)
                        {
                            int appId = -1;
                            int width = 360;
                            int height = 220;
                            try
                            {
                                if (int.TryParse(Request["AppId"], out appId) && appId > -1)
                                {
                                    var accoutManagement = WeChatCommonService.GetAccountManageByWeChatID(appId);
                                    if (null != accoutManagement && null != accoutManagement.AccountType && accoutManagement.AccountType.Value == 0)
                                    {
                                        width = 900;
                                        height = 500;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _Logger.Error(ex);
                            }
                            _Logger.Debug("_T width:{0} height:{1}", width, height);
                            ImageUtility.MakeThumbnail(null, postedFile.InputStream, strPath.Insert(strPath.LastIndexOf('.'), "_B"), width, height, "W");
                        }
                    }
                    else
                    {
                        postedFile.SaveAs(strPath);
                    }
                }
                _Logger.Debug("success to upload file.");
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "Upload file failed");
                throw ex;
            }
        }

        public ActionResult Breakpoint_Uploadfile(string path)
        {
            try
            {
                string thumbNailPath = string.Empty;
                string targetFilePath = string.Empty;
                string serverFileName = string.Empty;
                if (Request.Files[0] != null)
                {
                    FileInfo fi = new FileInfo(Request.Files[0].FileName);
                    serverFileName = Request.Files[0].FileName;
                    string uploadFileType = Request.QueryString["type"].ToLower();
                    targetFilePath = InitTargetFilePathDir(uploadFileType);
                    //上传原文件
                    Breakpoint_ProcessPostedFile(serverFileName, Request.Files[0], targetFilePath.Trim('/'), uploadFileType);
                }
                return Json(new { serverfileName = serverFileName, targetFilePath = targetFilePath },
                    StrJsonTypeTextHtml);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "Upload file failed");
            }
            return Json(new { success = false, timeout = false },
                StrJsonTypeTextHtml);
        }

        private void Breakpoint_ProcessPostedFile(string saveFullName, HttpPostedFileBase postedFile, string targetFilePath, string uploadFileType)
        {
            try
            {
                string saveDir = Path.Combine(Server.MapPath("~/"), targetFilePath);
                if (Directory.Exists(saveDir) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(saveDir);
                }
                if (postedFile != null)
                {
                    long lStartPos = 0;
                    int startPosition = 0;
                    int endPosition = 0;
                    var contentRange = Request.Headers["Content-Range"];
                    //bytes 10000-19999/1157632
                    if (!string.IsNullOrEmpty(contentRange))
                    {
                        contentRange = contentRange.Replace("bytes", "").Trim();
                        contentRange = contentRange.Substring(0, contentRange.IndexOf("/"));
                        string[] ranges = contentRange.Split('-');
                        startPosition = int.Parse(ranges[0]);
                        endPosition = int.Parse(ranges[1]);
                    }
                    System.IO.FileStream fs;
                    if (System.IO.File.Exists(Path.Combine(saveDir, saveFullName)))
                    {
                        fs = System.IO.File.OpenWrite(Path.Combine(saveDir, saveFullName));
                        lStartPos = fs.Length;
                    }
                    else
                    {
                        fs = new System.IO.FileStream(Path.Combine(saveDir, saveFullName), System.IO.FileMode.Create);
                        lStartPos = 0;
                    }
                    if (lStartPos > endPosition)
                    {
                        fs.Close();
                        return;
                    }
                    else if (lStartPos < startPosition)
                    {
                        lStartPos = startPosition;
                    }
                    else if (lStartPos > startPosition && lStartPos < endPosition)
                    {
                        lStartPos = startPosition;
                    }
                    fs.Seek(lStartPos, System.IO.SeekOrigin.Current);
                    byte[] nbytes = new byte[5120];
                    int nReadSize = 0;
                    nReadSize = postedFile.InputStream.Read(nbytes, 0, 5120);
                    while (nReadSize > 0)
                    {
                        fs.Write(nbytes, 0, nReadSize);
                        nReadSize = postedFile.InputStream.Read(nbytes, 0, 5120);
                    }
                    fs.Close();
                    //if ((endPosition - lStartPos) < 99999)
                    //{
                    //    FileInfo fi = new FileInfo(saveDir + saveFullName);
                    //    newFileName = Guid.NewGuid().ToString().Replace("-", "") + fi.Extension;
                    //    fi.MoveTo((saveDir + saveFullName).Replace(saveFullName, newFileName));
                    //}  
                    //ThumbImageAndInsertIntoDB(saveFullName, postedFile, targetFilePath, uploadFileType);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex, "Upload file failed");
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Breakpoint_FileReName(string serverfileName, string targetFilePath)
        {
            targetFilePath = targetFilePath.Trim('/');
            string saveDir = Path.Combine(Server.MapPath("~/"), targetFilePath, serverfileName);
            FileInfo fi = new FileInfo(saveDir);
            string newFileName = Guid.NewGuid().ToString().Replace("-", "") + fi.Extension;
            fi.MoveTo((saveDir).Replace(serverfileName, newFileName));
            return Json(new { isOk = true, serverfileName = newFileName, targetFilePath = targetFilePath },
                    StrJsonTypeTextHtml);
        }

        [HttpPost]
        public JsonResult ThumbImageAndInsertIntoDB()
        {
            string saveFullName = Request["saveFullName"];
            string fileName = Request["title"];
            string targetFilePath = Request["targetFilePath"];
            string uploadFileType = Request["uploadFileType"];
            string pid = Request["id"];
            AttachmentsItemPostProperty p = new AttachmentsItemPostProperty()
            {
                SaveFullName = saveFullName,
                ServerPath = Server.MapPath("~/"),
                FileName = fileName,
                TargetFilePath = targetFilePath.Trim('/'),
                UploadFileType = uploadFileType,
                AppId = int.Parse(Request["appid"]),
                Description = Request["description"] == null ? string.Empty : Request["description"],
                VideoCoverSrc = Request["videoCoverSrc"] == null ? string.Empty : Request["videoCoverSrc"].Trim('/'),
                UserName = User.Identity.Name,
                ViewId = pid,
            };
            SysAttachmentsItem itemView = _attachmentsItemService.ThumbImageAndInsertIntoDB(p);
            return Json(new { Id = itemView.Id, AttachmentUrl = itemView.AttachmentUrl, AttachmentTitle = itemView.AttachmentTitle, Description = itemView.Description, Duration = itemView.Duration }
                        , StrJsonTypeTextHtml);
        }

        ///// <summary>        
        ///// 生成大概想要的图片 !+_+      
        ///// </summary>        
        ///// <param name="originalImagePath">源图路径（物理路径）</param>        
        ///// <param name="thumbnailPath">缩略图路径（物理路径）</param>        
        ///// <param name="width">缩略图宽度</param>        
        ///// <param name="height">缩略图高度</param>        
        ///// <param name="mode">生成缩略图的方式</param>   
        ///// <param name="saveFullName">文件的名字</param>           
        //private void MakeThumbnailForUploadImage(string targetFilePath, int width, int height, string mode, string saveFullName, SysAttachmentsItem itemView)
        //{
        //    bool isGetThrumbImage = saveFullName.Contains("_t");
        //    string originalImagePath = Path.Combine(Server.MapPath("~/"), targetFilePath);
        //    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(isGetThrumbImage ? Path.Combine(originalImagePath, saveFullName).Replace("_t", "") : Path.Combine(originalImagePath, saveFullName));
        //    if (!isGetThrumbImage)
        //    {
        //        itemView.Width = originalImage.Width;
        //        itemView.Height = originalImage.Height;
        //    }
        //    int towidth = width;
        //    int toheight = height;
        //    int x = 0;
        //    int y = 0;
        //    int ow = originalImage.Width;
        //    int oh = originalImage.Height;
        //    if (ow > width || isGetThrumbImage)
        //    {
        //        switch (mode)
        //        {
        //            case "HW":
        //                //指定高宽缩放（可能变形）                                    
        //                break;
        //            case "W":
        //                //指定宽，高按比例                                        
        //                toheight = originalImage.Height * width / originalImage.Width;
        //                break;
        //            case "H":
        //                //指定高，宽按比例                    
        //                towidth = originalImage.Width * height / originalImage.Height;
        //                break;
        //            case "Cut":
        //                //指定高宽裁减（不变形）                                    
        //                if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
        //                {
        //                    oh = originalImage.Height;
        //                    ow = originalImage.Height * towidth / toheight;
        //                    y = 0;
        //                    x = (originalImage.Width - ow) / 2;
        //                }
        //                else
        //                {
        //                    ow = originalImage.Width;
        //                    oh = originalImage.Width * height / towidth;
        //                    x = 0;
        //                    y = (originalImage.Height - oh) / 2;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        //新建一个bmp图片            
        //        System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
        //        //新建一个画板            
        //        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
        //        //设置高质量插值法            
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //        //设置高质量,低速度呈现平滑程度            
        //        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //        //清空画布并以透明背景色填充            
        //        g.Clear(System.Drawing.Color.Transparent);
        //        //在指定位置并且按指定大小绘制原图片的指定部分            
        //        g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
        //            new System.Drawing.Rectangle(x, y, ow, oh),
        //            System.Drawing.GraphicsUnit.Pixel);
        //        try
        //        {
        //            //以jpg格式保存缩略图  
        //            originalImage.Dispose();//释放原图，保存的时候覆盖原图
        //            bitmap.Save(Path.Combine(originalImagePath, saveFullName), System.Drawing.Imaging.ImageFormat.Png);

        //            if (isGetThrumbImage)
        //            {
        //                itemView.ThumbUrl = Path.Combine(targetFilePath, saveFullName);
        //            }
        //            else
        //            {
        //                itemView.Width = towidth;
        //                itemView.Height = toheight;
        //                itemView.FileSize = new FileInfo(Path.Combine(originalImagePath, saveFullName)).Length;
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            bitmap.Dispose();
        //            g.Dispose();
        //        }
        //    }
        //    if (!isGetThrumbImage)
        //    {
        //        //获得宽200的缩略图
        //        //以_t标记
        //        string[] temp = saveFullName.Split('.');
        //        temp[0] = temp[0] + "_t";
        //        saveFullName = string.Join(".", temp);
        //        MakeThumbnailForUploadImage(targetFilePath, 200, 200, "W", saveFullName, itemView);
        //    }
        //}

        //private void MakeThumbnail(string targetFilePath, int width, int height, string mode, string saveFullName)
        //{
        //    string originalImagePath = Path.Combine(Server.MapPath("~/"), targetFilePath);
        //    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(Path.Combine(originalImagePath, saveFullName));
        //    int towidth = width;
        //    int toheight = height;
        //    int x = 0;
        //    int y = 0;
        //    int ow = originalImage.Width;
        //    int oh = originalImage.Height;
        //    switch (mode)
        //    {
        //        case "HW":
        //            //指定高宽缩放（可能变形）                                    
        //            break;
        //        case "W":
        //            //指定宽，高按比例                                        
        //            toheight = originalImage.Height * width / originalImage.Width;
        //            break;
        //        case "H":
        //            //指定高，宽按比例                    
        //            towidth = originalImage.Width * height / originalImage.Height;
        //            break;
        //        case "Cut":
        //            //指定高宽裁减（不变形）                                    
        //            if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
        //            {
        //                oh = originalImage.Height;
        //                ow = originalImage.Height * towidth / toheight;
        //                y = 0;
        //                x = (originalImage.Width - ow) / 2;
        //            }
        //            else
        //            {
        //                ow = originalImage.Width;
        //                oh = originalImage.Width * height / towidth;
        //                x = 0;
        //                y = (originalImage.Height - oh) / 2;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    //新建一个bmp图片            
        //    System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
        //    //新建一个画板            
        //    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
        //    //设置高质量插值法            
        //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //    //设置高质量,低速度呈现平滑程度            
        //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    //清空画布并以透明背景色填充            
        //    g.Clear(System.Drawing.Color.Transparent);
        //    //在指定位置并且按指定大小绘制原图片的指定部分            
        //    g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
        //        new System.Drawing.Rectangle(x, y, ow, oh),
        //        System.Drawing.GraphicsUnit.Pixel);
        //    try
        //    {
        //        //以jpg格式保存缩略图  
        //        originalImage.Dispose();//释放原图，保存的时候覆盖原图
        //        bitmap.Save(Path.Combine(originalImagePath, saveFullName), System.Drawing.Imaging.ImageFormat.Png);
        //    }
        //    catch (System.Exception e)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        bitmap.Dispose();
        //        g.Dispose();
        //    }
        //}

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
        //    _Logger.Debug("filePath :{0}", filePath);
        //    string dirName = Path.GetDirectoryName(filePath);
        //    _Logger.Debug("dirName :{0}", dirName);
        //    string SongName = Path.GetFileName(filePath);
        //    _Logger.Debug("SongName :{0}", SongName);
        //    FileInfo fInfo = new FileInfo(filePath);
        //    ShellClass sh = new ShellClass();
        //    Folder dir = sh.NameSpace(dirName);
        //    FolderItem item = dir.ParseName(SongName);
        //    string duration = Regex.Match(dir.GetDetailsOf(item, 27), "\\d:\\d{2}:\\d{2}").Value;//获取歌曲时间 
        //    if (string.IsNullOrEmpty(duration))
        //    {
        //        throw new Exception("上传的文件没有时长, 请检查");
        //    }
        //    _Logger.Debug("duration :{0}", duration);
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
        //    _Logger.Debug("hour :{0}, min :{1}, second :{2}", hour, min, second);
        //    int senconds = hour + min + second;
        //    return senconds.ToString();
        //}

    }
}
