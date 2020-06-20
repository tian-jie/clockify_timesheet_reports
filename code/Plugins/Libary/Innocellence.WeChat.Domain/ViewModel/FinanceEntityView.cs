using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ModelsView
{
    public class FinanceEntityView : IViewModel
    {
        public  int Id { get; set; }
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

        public List<FinanceEntityView> FinanceList { get;set; }

        public IViewModel ConvertAPIModel(object obj) {
            var entity = (FinanceQueryEntity)obj;
            Id = entity.Id;
            GEDNO = entity.GEDNO;
            TEANO = entity.TEANO;
            WeChatUserID = entity.WeChatUserID;
            MoneySum = entity.MoneySum;
            ReceiveDate = entity.ReceiveDate;
            Status = entity.Status;
            CreatedDate = entity.CreatedDate;
            UpdateDate = entity.UpdateDate;
            CreatedUserID = entity.CreatedUserID;
            UpdatedUserID = entity.UpdatedUserID;
            Comment = entity.Comment;
            return this;
        }
    }
}
