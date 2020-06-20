using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Innocellence.Weixin.QY.AdvancedAPIs.MailList;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IAddressBookService : IDependency, IBaseService<SysAddressBookMember>
    {
        StringBuilder InsertOrUpdateAddressBookMember(List<AddressBookMemberTemplateModel> items, HttpRequestBase request, List<string> updateColumns, bool canInsertNew);

        void InsertAddressBookMember(SysAddressBookMember item);

        List<SysAddressBookMember> GetAllAddressBookMember(int accountManageId);

        SysAddressBookMember GetMemberByUserId(string userId, bool ignoreDeleted = false);

        SysAddressBookMember GetMemberById(int id);

        void UpdateMember(SysAddressBookMember member);

        void DeleteMember(string userId);

        string GenerateUserId(string employeeNo);

        void SyncMember(int accountManageId);

        Stream GetUploadCsv(List<AddressBookMemberTemplateModel> modelList, List<string> needUploadColumns);

        void addMemberTag(string userId, int tagId);

        void delMemberTag(string userId, int tagId);

        void CutPropertyBefaultSync2TX(GetMemberResult objModal);

        void InitTagListTemp();

        void UpdateTagListTemp(List<string> userList, int tagId);

        void UpdateTagListByTagListTemp();
    }
}
