using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ThumbsUpCountView : IViewModel
    {

        public Int32 Id { get; set; }

        /// <summary>
        /// 用于记录点赞的模块，用表名标识
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 对该表中的这条记录进行点赞
        /// </summary>
        public Int32 RecordID { get; set; }
        
        /// <summary>
        /// 点赞数量
        /// </summary>
        public int ThumbsUpCount { get; set; }

        /// <summary>
        /// 我有没有点过赞
        /// </summary>
        public bool AmIThumbsUp { get; set; }

        /// <summary>
        /// 这个case里的这个转换不好用了。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ThumbsUp)obj;
            Id = entity.Id;
            TableName = entity.TableName;
            RecordID = entity.RecordID;

            return this;
        }

    }
}
