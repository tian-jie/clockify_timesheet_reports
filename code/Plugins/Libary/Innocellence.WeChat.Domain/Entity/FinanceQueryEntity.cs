using System;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class FinanceQueryEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string TEANO { get; set; }

        public string GEDNO { get; set; }

        public string WeChatUserID { get; set; }

        public Decimal MoneySum { get; set; }

        public DateTime ReceiveDate { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string CreatedUserID { get; set; }

        public string UpdatedUserID { get; set; }

        public string Comment { get; set; }
    }
}
