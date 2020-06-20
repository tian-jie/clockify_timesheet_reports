using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AccountManageView : IViewModel, IEntity
    {
        public AccountManageView() { }
        //{
        //    AccountManageViews = new List<AccountManageView>();
        //    List = new List<AccountManageView>();
        //}
        public Int32 Id { get; set; }
        [Required(ErrorMessage = "请上传Qr Code")]
        public HttpPostedFileBase QrCode { get; set; }
        [Required(ErrorMessage = "请上传Account Logo")]
        public HttpPostedFileBase AccountLogo { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "账户名称不能为空")]
        public string AccountName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "账户描述不能为空")]
        public string AccountDescription { get; set; }
        public string AccountLogoPath { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "CorpId不能为空")]
        public string CorpId { get; set; }
        public string QrCodePath { get; set; }
        public Int32? AccountType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedUserID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedUserID { get; set; }
        public bool IsDeleted { get; set; }
        //public List<AccountManageView> AccountManageViews { get; set; }
        //public IList<AccountManageView> List { get; set; }

        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (AccountManage)obj;
            Id = entity.Id;
            AccountName = entity.AccountName;
            AccountDescription = entity.AccountDescription;
            AccountLogoPath = entity.AccountLogo;
            CorpId= entity.CorpId;
            QrCodePath= entity.QrCode;
            AccountType= entity.AccountType;
            CreatedDate = entity.CreatedDate;
            CreatedUserID= entity.CreatedUserID;
            UpdatedDate = entity.UpdatedDate;
            UpdatedUserID = entity.UpdatedUserID;
            IsDeleted = (bool)entity.IsDeleted;

            return this;
        }
        public AccountManage ConvertToEntity()
        {
            var entity = new AccountManage();
            entity.Id = Id;
            entity.AccountName = AccountName;
            entity.AccountDescription = AccountDescription;
            entity.AccountLogo = AccountLogoPath;
            entity.CorpId = CorpId;
            entity.QrCode = QrCodePath;
            entity.AccountType = AccountType;
            entity.CreatedDate = CreatedDate;
            entity.CreatedUserID =CreatedUserID;
            entity.UpdatedDate = UpdatedDate;
            entity.UpdatedUserID = UpdatedUserID;
            entity.IsDeleted = IsDeleted;
            return entity;
        }
    }
}
