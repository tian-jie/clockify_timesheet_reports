using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class FaqInfoService : BaseService<FaqInfo>, IFaqInfoService
    {
        public FaqInfoService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {
        }

        public List<FaqInfoView> GetListBySearchKey(int appId, string key)
        {
            var entities = Repository.Entities;

            foreach (var k in key.Split(' '))
            {
                entities = entities.Where(a => a.AppId == appId && (a.Question.Contains(k) || a.Answer.Contains(k)));
            }

            return entities.ToList()
                     .Select(n => (FaqInfoView)new FaqInfoView().ConvertAPIModel(n))
                     .OrderByDescending(x => x.ReadCount).ThenBy(x => x.Id)
                     .ToList();
        }
        //测试【Key Resource Attachment】链接
        public List<FaqInfoView> GetFAQList()
        {
            var entities = Repository.Entities;

            
                entities = entities.Where(a => a.ResourceEnternalLink !=null || (a.KeyResourceAttachment!=null));
         
            return entities.ToList()
                     .Select(n => (FaqInfoView)new FaqInfoView().ConvertAPIModel(n)).OrderBy(m=>m.Id)
                     .ToList();
        }
    }
}
