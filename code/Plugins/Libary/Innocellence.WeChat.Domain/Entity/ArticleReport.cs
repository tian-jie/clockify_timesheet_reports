using Infrastructure.Core;
using System;

namespace Innocellence.WeChat.Domain.Entity
{
    public class ArticleReport : EntityBase<int>
    {
        //[Id("Id",IsDbGenerated=true)]
        public override int Id { get; set; }

        //[AppId("AppId",IsDbGenerated=true)]
        public Int32 AppId { get; set; }

        //[ArticleId("ArticleId",IsDbGenerated=true)]
        public Int32 ArticleId { get; set; }

        //[Column("MenuKey",DbType=DBType.Varchar,Length=50,Precision=23,IsNullable=true)]
        public string MenuKey { get; set; }

        //[Column("MenuName",DbType=DBType.NVarchar,Length=50,Precision=23,IsNullable=true)]
        public string MenuName { get; set; }

        //[Column("MenuName",DbType=DBType.NVarchar,Length=50,Precision=23,IsNullable=true)]
        public string AppName { get; set; }

        //[Column("ArticleTitle",DbType=DBType.NVarchar,Length=500,Precision=23,IsNullable=true)]
        public string ArticleTitle { get; set; }

        //[Column("CreatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? CreatedDate { get; set; }

        //[Column("UpdatedDate",DbType=DBType.DateTime,Length=8,Precision=23,IsNullable=true)]
        public DateTime? AccessDate { get; set; }

        //[VisitorCount("VisitorCount",IsDbGenerated=true)]
        public Int32 VisitorCount { get; set; }

        //[VisitTimes("VisitTimes",IsDbGenerated=true)]
        public Int32 VisitTimes { get; set; }
    }
}
