using Infrastructure.Utility.Data;
using Infrastructure.Web.Domain.Model;
using Infrastructure.Web.Domain.Service;
using Infrastructure.Web.MVC.Attribute;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain;
using Innocellence.WeChat.Domain.Common;
//using Innocellence.WeChatMain.Admin.Controllers;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;


namespace Innocellence.WeChatMain.Controllers
{
    /// <summary>
    /// 这个类属于一个公用的类，很多地方会用，暂时弄成登录用户就可以
    /// </summary>
    [AllowLoginUserAttribute]
    public class FileManageController : BaseController<SysAttachmentsItem, AttachmentsItemView>
    {
        private static string fileManageUrl = CommonService.GetSysConfig("FileManageUrl", string.Empty);
        private static string fileManagePicture = CommonService.GetSysConfig("FileManagePicture", "filemanage/image/");
        private static string fileManageOther = CommonService.GetSysConfig("FileManageOther", "filemanage/other/");

        private IAttachmentsItemService _attachmentsItemService;

        public FileManageController(IAttachmentsItemService attachmentsItemService)
            : base(attachmentsItemService)
        {
            _attachmentsItemService = attachmentsItemService;
        }

        public override ActionResult Index()
        {
            ViewBag.AppId = Request.QueryString["AppId"];
            return View();
        }

        /// <summary>
        /// 上传图片处理
        /// </summary>
        /// <returns></returns>
        public ActionResult PostFile()
        {
            string strPath = string.Empty;

            string fileType = string.Empty;
            string fileSize = string.Empty;
            string filePath = string.Empty;
            string fileExtension = string.Empty;
            string miniFileName = string.Empty;
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase objFile = Request.Files[0];
                filePath = objFile.ContentType.ToLower().Contains("image") ? fileManagePicture : fileManageOther;
                if (!System.IO.Directory.Exists(Request.PhysicalApplicationPath + filePath))
                {
                    System.IO.Directory.CreateDirectory(Request.PhysicalApplicationPath + filePath);
                }

                fileExtension = System.IO.Path.GetExtension(objFile.FileName);

                if (string.IsNullOrEmpty(fileExtension))
                {
                    return Json(new UploadMessageError("不识别的非法文件,无法进行上传!", string.Empty, string.Empty),
                            JsonRequestBehavior.AllowGet);
                }

                miniFileName = objFile.FileName.Substring(0, objFile.FileName.IndexOf('.'));
                if (miniFileName.Length > 50)
                {
                    return Json(new UploadMessageError("文件名过长,请控制在50字符以内!", string.Empty, string.Empty),
                        JsonRequestBehavior.AllowGet);
                }

                strPath = string.Format("{0}{1}{2}**{3}", Request.PhysicalApplicationPath, filePath,
                DateTime.Now.ToString("yyyyMMddHHmmss"), fileExtension);

                if (filePath.Equals(fileManageOther) && fileExtension.ToLower().Contains(".xlsx"))
                {
                    fileType = "application/vnd.ms-excel";
                }
                else if (filePath.Equals(fileManageOther) && fileExtension.ToLower().Contains(".pptx"))
                {
                    fileType = "application/vnd.ms-powerpoint";
                }
                else if (filePath.Equals(fileManageOther) && fileExtension.ToLower().Contains(".docx"))
                {
                    fileType = "application/msword";
                }
                else
                {
                    fileType = objFile.ContentType;
                }

                objFile.SaveAs(strPath.Replace("**", ""));

                fileSize = FileSizeFormat(objFile.ContentLength);

                string strName = System.IO.Path.GetFileName(strPath.Replace("**", ""));

                FileManageView imageFile = new FileManageView()
                {
                    FileName = strName,
                    FileType = fileType,
                    FileSize = fileSize,
                    Url = fileManageUrl,
                    FilePath = filePath,
                    OriginalName = miniFileName
                };

                _BaseService.InsertView(imageFile);

                return Json(new UploadMessageSuccess(new UploadMessageSuccessMsg(strName, string.Empty), imageFile.Id.ToString()),
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new UploadMessageError("没有正确上传文件，或上传已终止!", string.Empty, string.Empty),
                JsonRequestBehavior.AllowGet);
        }

        private string FileSizeFormat(int contentLength)
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

        public override List<AttachmentsItemView> GetListEx(Expression<Func<SysAttachmentsItem, bool>> predicate, PageCondition ConPage)
        {
            if (Request["type"] != null)
            {
                string type = Request["type"].ToString().ToUpper();
                AttachmentsType fileType = (AttachmentsType)Enum.Parse(typeof(AttachmentsType), type);
                predicate = predicate.AndAlso(x => x.Type == (int)fileType);
            }

            if (Request["appid"] != null && !string.IsNullOrEmpty(Request["appid"]))
            {
                int appId = int.Parse(Request["appid"]);
                predicate = predicate.AndAlso(x => x.AppId == appId);
            }

            if (!string.IsNullOrEmpty(Request["searchKeyword"]))
            {
                string searchKeyword = Request["searchKeyword"].ToString().ToUpper();
                predicate = predicate.AndAlso(x => x.AttachmentTitle.ToUpper().Contains(searchKeyword));
            }

            predicate = predicate.AndAlso(x => x.IsDeleted == false);//&& (x.UserId.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase) || x.UserName.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)));

            //ConPage.SortConditions.Add(new SortCondition("CreateTime", System.ComponentModel.ListSortDirection.Descending));

            var q = _BaseService.GetList<AttachmentsItemView>(predicate, ConPage);
            q.ForEach(p =>
            {
                if (p.Type == (int)AttachmentsType.IMAGE)
                {
                    p.AttachmentUrl = "/" + p.AttachmentUrl;
                }
                if (!string.IsNullOrEmpty(p.ThumbUrl))
                {
                    p.ThumbUrl = "/" + p.ThumbUrl;
                }
            });
            return q.ToList();
        }

        public JsonResult Update(AttachmentsItemView newView)
        {
            try
            {
                _attachmentsItemService.UpdateView<AttachmentsItemView>(newView);
                return Json(doJson(null), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return ErrorNotification(e, "update failed");
            }
        }

        public override JsonResult Delete(string sIds)
        {
            try
            {
                string deleteId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(deleteId))
                {
                    var deleteRecord = _attachmentsItemService.GetById<AttachmentsItemView>(int.Parse(deleteId));
                    if (deleteRecord != null)
                    {
                        try
                        {
                            string filePath = Path.Combine(Server.MapPath("~/"), deleteRecord.AttachmentUrl);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                            if (deleteRecord.Type == (int)AttachmentsType.IMAGE)
                            {
                                filePath = Path.Combine(Server.MapPath("~/"), deleteRecord.ThumbUrl);
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            _attachmentsItemService.Repository.Delete(deleteRecord.Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return Json(doJson(null), JsonRequestBehavior.AllowGet);
        }

        public override ActionResult Edit(string id)
        {
            try
            {
                string type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(type))
                {
                    var record = _attachmentsItemService.Repository.GetByKey(int.Parse(id));
                    if (record != null)
                    {
                        record.AttachmentTitle = Request.QueryString["AttachmentTitle"];
                        _attachmentsItemService.Repository.Update(record, new List<string>() { "AttachmentTitle" });
                    }
                }
                return Json(doJson(null), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return ErrorNotification(e, "update failed");
            }
        }

        public void Download(string filePath, int fileId, string extension)
        {
            var file = _attachmentsItemService.GetById<AttachmentsItemView>(fileId);
            if (null != file)
            {
                var path = Path.Combine(Server.MapPath("~/"), filePath.Trim('/'));
                Response.ClearHeaders();
                Response.Clear();
                Response.Expires = 0;
                Response.Buffer = true;
                Response.AddHeader("Accept-Language", "zh-tw");
                string fileName = file.AttachmentTitle;
                string name = fileName.EndsWith(extension) ? fileName : string.Format("{0}{1}", fileName, extension);
                System.IO.FileStream files = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] byteFile = null;
                if (files.Length == 0)
                {
                    byteFile = new byte[1];
                }
                else
                {
                    byteFile = new byte[files.Length];
                }
                files.Read(byteFile, 0, (int)byteFile.Length);
                files.Close();
                string fileDownloadName = HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8);
                fileDownloadName = fileDownloadName.Replace("+", "%20");
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fileDownloadName);
                Response.ContentType = "application/octet-stream;charset=gbk";
                Response.BinaryWrite(byteFile);
                Response.End();
            }
        }

        public ActionResult ConvertToBase64(string filePath)
        {
            string base64Str = string.Empty;
            long duration = 0;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var path = Path.Combine(Server.MapPath("~/"), filePath.Trim('/'));
                _Logger.Debug(path);
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        duration = GetAMRFileDuration(path);
                        using (FileStream fs = System.IO.File.OpenRead(path))
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                byte[] bt = br.ReadBytes(Convert.ToInt32(fs.Length));
                                base64Str = Convert.ToBase64String(bt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Logger.Error(ex);
                }
            }
            return Json(new { path = base64Str, duration = duration }, JsonRequestBehavior.AllowGet);
        }

        private long GetAMRFileDuration(string fileName)
        {
            long duration = 0;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] packed_size = new byte[16] { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };
                    int pos = 0;
                    pos += 6;
                    long lenth = fs.Length;
                    byte[] toc = new byte[1];
                    int framecount = 0;
                    byte ft;
                    while (pos < lenth)
                    {
                        fs.Seek(pos, SeekOrigin.Begin);
                        if (1 != fs.Read(toc, 0, 1))
                        {
                            duration = lenth > 0 ? ((lenth - 6) / 650) : 0;
                            fs.Close();
                            break;
                        }
                        ft = (byte)((toc[0] / 8) & 0x0F);
                        pos += packed_size[ft] + 1;
                        framecount++;
                    }
                    duration = framecount * 20 / 1000;
                }
            }
            catch (Exception e)
            {
                _Logger.Error(e);
            }
            return duration;
        }
    }
}
