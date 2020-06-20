using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class ArticleInfoReadHistoryView : IViewModel
    {
        public int Id { get; set; }

        public Int32? ArticleInfoId { get; set; }

        public string UserId { get; set; }


        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (ArticleInfoReadHistory)model;
            this.Id = entity.Id;
            this.UserId = entity.UserId;
            this.ArticleInfoId = entity.ArticleInfoId;
            return this;
        }
    }
}
