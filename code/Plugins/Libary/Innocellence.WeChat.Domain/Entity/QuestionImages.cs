using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.WeChat.Domain.Entity
{
    //[Table("Category")]
    /// <summary>

    /// </summary>
    public partial class QuestionImages : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public override Int32 Id { get; set; }


        public String ImageType { get; set; }


        public String ImageName { get; set; }

       
        public Int32? QuestionID { get; set; }



        public byte[] ImageContent { get; set; }

        //
        //[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? CreatedDate { get; set; }

        //
        //[Column("CreatedUserID",DbType=DBType.VarChar,Length=50,Precision=50,IsNullable=true)]
        public String CreatedUserID { get; set; }

        //
        //[Column("AppId",DbType=DBType.Int32,Length=4,Precision=10,IsNullable=true)]
        public Int32? AppId { get; set; }
    }
}
