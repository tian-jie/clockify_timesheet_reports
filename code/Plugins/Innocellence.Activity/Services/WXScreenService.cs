using Infrastructure.Core.Data;
using Innocellence.Activity.Model;
using Innocellence.Activity.ViewModel;
using System.Collections.Generic;
using System.Linq;
namespace Innocellence.Activity.Services
{
    public partial class WXScreenService : BaseService<WXScreen>, IWXScreenService
    {
        public WXScreenService()
            : base("CAAdmin")
        {


        }

        public void getEmplist(List<WXScreenView> lists)
        {
         //   // 获取部门列表
         //   List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
         //   lists.ForEach(item =>
         //   {
         //       var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(item.LillyId.ToUpper()));
         //       if (emp != null)
         //       {
         //           item.deptLvs = emp.deptLvs;
         //           item.UserName = emp.name;
         //       }
         //   }
         //);
        }
    }
}