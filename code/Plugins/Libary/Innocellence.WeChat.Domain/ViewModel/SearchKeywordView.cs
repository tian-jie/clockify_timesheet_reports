using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("Message")]
    public partial class SearchKeywordView : IViewModel
    {

        public Int32 Id { get; set; }
        public Int32? AppId { get; set; }
        public String Keyword { get; set; }
        public Int32? SearchCount { get; set; }


        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (SearchKeyword)obj;
            Id = entity.Id;
            Keyword = entity.Keyword;
            SearchCount = entity.SearchCount;

            return this;
        }
    }
}
