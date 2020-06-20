using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    public class ThumbsUp : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }


        /// <summary>
        /// 用于记录点赞的模块，用表名标识
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 对该表中的这条记录进行点赞
        /// </summary>
        public Int32 RecordID { get; set; }

        /// <summary>
        /// 点赞的人
        /// </summary>
        public string WeChatUserID { get; set; }

        /// <summary>
        /// 点赞的时间
        /// </summary>
        public string ThumbUpTime { get; set; }

    }
}
