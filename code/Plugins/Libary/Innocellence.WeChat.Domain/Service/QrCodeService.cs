using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class QrCodeService : BaseService<QrCodeMPItem>, IQrCodeService
    {
        public QrCodeService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {

        }

        public void EditDescription(QrCodeView model, string userName)
        {
            var old = this.Repository.Entities.Where(a => a.SceneId == model.SceneId && a.AppId == model.AppId && a.Deleted == false).FirstOrDefault();
            if (old != null)
            {
                old.Description = model.Description;
                old.UpdatedDate = DateTime.Now;
                old.UpdatedUserID = userName;
                this.Repository.Update(old);
            }
        }

        public void AddOrCode(QrCodeView model, string userName)
        {
            var old = this.Repository.Entities.Where(a => a.Id == model.SceneId && a.Deleted == true).FirstOrDefault();
            if (old != null)
            {
                old.SceneId = model.SceneId;
                old.AppId = model.AppId;
                old.Description = model.Description;
                old.CreatedDate = DateTime.Now;
                old.Deleted = false;
                old.CreatedUserID = userName;
                old.UpdatedDate = DateTime.Now;
                old.UpdatedUserID = userName;
                old.Url = model.Url;
                this.Repository.Update(old);
            }
        }

        public int GenerateSceneId()
        {
            QrCodeMPItem entity = new QrCodeMPItem()
            {
                Deleted = true,
                AppId = 0
            };
            this.Repository.Insert(entity);
            return entity.Id;
        }

        /// <summary>
        /// 获取分页，因为查询
        /// </summary>
        /// <returns></returns>
        public List<QrCodeView> GetListAll(int pageIndex, int pageSize, ref int total, string strDescription, int appId)
        {
            string strSQL = string.Format(@"select a.id,a.SceneId,a.Description,a.url, sum(case Status when 1 then 1 else 0 end) PeopleCount, sum(case  when Status=1 then 1 when Status is null then 0 else -1 end) PurePeopleCount 
                                            from QrCodeMPItem a 
                                            left outer join ( SELECT distinct userid,QrCodeSceneId,Status,PeopleCount,PurePeopleCount
                                                            FROM [FocusHistory] 
                                                            group by userid,QrCodeSceneId,Status,[PeopleCount],[PurePeopleCount]) b 
                                            on a.SceneId=b.QrCodeSceneId where a.AppId={0} and ## group by a.SceneId,a.Description,a.url,a.Id "
                                            , appId);

            if (!string.IsNullOrEmpty(strDescription))
            {
                strSQL = strSQL.Replace("##", " Description like '%'+@Description+'%'");
                SqlParameter para = new SqlParameter() { ParameterName = "Description", Value = strDescription };

                return base.GetListSQL<QrCodeView>(pageIndex, pageSize, ref total, strSQL, "id desc", para);
            }
            else
            {
                strSQL = strSQL.Replace("##", "1=1");
                return base.GetListSQL<QrCodeView>(pageIndex, pageSize, ref total, strSQL, "id desc");
            }

        }
    }
}
