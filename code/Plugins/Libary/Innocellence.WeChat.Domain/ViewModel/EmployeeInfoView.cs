using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class EmployeeInfoView
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public EmployeePageData Data { get; set; }
    }

    public  class EmployeePageData
    {
        public int TotalQty { get; set; }

        public List<EmployeeData> EmployeeInfo { get; set; }
    }

    public class EmployeeData 
    {
        public int ROWNUMBER { get; set; }
        public string EmployeeNo { get; set; }
        public string ADAccount { get; set; }
        public string CName { get; set; }
        public string EName { get; set; }
        public string Gender { get; set; }
        public string GradeCode { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string DirectManagerID { get; set; }
        public string EmployeeStatus { get; set; }
        public string GroupName { get; set; }
        public int? GroupID { get; set; }
        public string BUName { get; set; }
        public int? BUID { get; set; }
        public string SBUName { get; set; }
        public int? SBUID { get; set; }
        public int? CompanyID { get; set; }
    }
}
