using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Contracts;
using System.IO;
using Innocellence.WeChat.Domain;

namespace Innocellence.WeChatMain.Controllers
{
    public class AccountDetailController : BaseController<AccountManage, AccountManageView>
    {

        private readonly IAccountManageService _accountManageService;

        public AccountDetailController(IAccountManageService objService)
            : base(objService)
        {
            _accountManageService = objService;
        }

        // GET: AccountDetail
        public override ActionResult Index()
        {
            ViewBag.AccountManageid =  int.Parse(Request.Cookies["AccountManageId"].Value); ;
            return base.Index();
        }
        public override ActionResult Get(string id)
        {
            int accountId = 0;
            int.TryParse(id,out accountId);
            var account=_accountManageService.GetById(accountId);
            AccountManageView view=ConvertToAccountManageView(account);
            return Json(view,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public  ActionResult UploadImg(AccountManageView viewModel)
        {
            var account = _accountManageService.GetById(viewModel.Id);
            if (viewModel.QrCode!= null)
            {
                account.QrCode = SaveImage("QrCode", viewModel.Id, viewModel.QrCode, account.QrCode);
            }
            if (viewModel.AccountLogo!= null)
            {
                account.AccountLogo = SaveImage("AccountLogo", viewModel.Id, viewModel.AccountLogo, account.AccountLogo);
            }
            _accountManageService.Update(account);
            AccountManageView view = ConvertToAccountManageView(account);
            return Redirect("~/AccountDetail");
        }
        private string SaveImage(string namePrexi,int id, HttpPostedFileBase file,string oldFile)
        {
            string newName = namePrexi + id + GetTimeStamp()+ Path.GetExtension(file.FileName);
            string filePath = Path.Combine(HttpContext.Server.MapPath("~/"), "Content/picture",
                            Path.GetFileName(newName));
            var oldFilePath = Path.Combine(HttpContext.Server.MapPath("~/"), 
                            Path.GetFileName(oldFile));
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            file.SaveAs(filePath);
            string resoucePath = Path.Combine(("/Content/picture"), Path.GetFileName(newName));
            return resoucePath.Replace("\\", "/");
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        private AccountManageView ConvertToAccountManageView(AccountManage entity)
        {
            return new AccountManageView
            {
                Id = entity.Id,
                AccountName = entity.AccountName,
                AccountDescription = entity.AccountDescription,
                AccountLogoPath = entity.AccountLogo,
                CorpId = entity.CorpId,
                QrCodePath = entity.QrCode,
                AccountType = entity.AccountType,
                CreatedDate = entity.CreatedDate,
                CreatedUserID = entity.CreatedUserID,
                UpdatedDate = entity.UpdatedDate,
                UpdatedUserID = entity.UpdatedUserID,
                IsDeleted = (bool)entity.IsDeleted,
           };
        }
    }
}