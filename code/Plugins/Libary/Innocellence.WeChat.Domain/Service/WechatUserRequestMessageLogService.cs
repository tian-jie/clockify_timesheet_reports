using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.ViewModel;
using System.Diagnostics;
using Infrastructure.Utility.Data;
using Infrastructure.Core.Infrastructure;

namespace Innocellence.WeChat.Domain.Service
{
    public class WechatUserRequestMessageLogService : BaseService<WechatUserRequestMessageLog>, IWechatUserRequestMessageLogService
    {
        private IAddressBookService _addressBookService = EngineContext.Current.Resolve<IAddressBookService>();

        public WechatUserRequestMessageLogService()
            : base("CAADMIN")
        {
            
        }

        public List<T> GetList<T>(Expression<Func<WechatUserRequestMessageLog, bool>> predicate) where T : IViewModel, new()
        {
            var list = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return list;
        }

        public int Add(WechatUserRequestMessageLogView view)
        {
            var entity = view.ConvertToEntity();
            var result = Repository.Insert(entity);
            return result;
        }

        private string GetUserNameByUserId(string userId)
        {
            var user = _addressBookService.GetMemberByUserId(userId);
            return user == null ? string.Empty : user.UserName;
        }

        public List<T> GetRecords<T>(int id, int appId, int pageSize, int pageNumber) where T : IViewModel, new()
        {
            Expression<Func<WechatUserRequestMessageLog, bool>> predicate = h => h.Id == id && h.AppID == appId;
            var entity = GetList<WechatUserRequestMessageLogView>(predicate, new PageCondition()).FirstOrDefault();
            if (null != entity)
            {
                int forwardCount = 0;
                int backwardCount = 0;
                if (pageNumber == 0)
                {
                    forwardCount = pageSize;
                    backwardCount = pageSize;
                }
                else if (pageNumber < 0)
                {
                    forwardCount = (-pageNumber + 1) * pageSize;
                    backwardCount = pageNumber * pageSize - 1;
                }
                else if (pageNumber > 0)
                {
                    forwardCount = -(pageNumber * pageSize + 1);
                    backwardCount = (pageNumber + 1) * pageSize;
                }
                //生成row_number
                string rowNumberTable = string.Format("select *,  row_number() over(order by id) as RowNumber from WechatUserRequestMessageLog where UserId = '{0}'", entity.UserID);
                //获取当前点击的记录对应的row_number
                string getForwardRowNumber = string.Format("select ForwardRowNumberTable.RowNumber - {0} from ({1}) as ForwardRowNumberTable where ForwardRowNumberTable.Id = {2}", forwardCount, rowNumberTable, entity.Id);
                string getBackwardRowNumber = string.Format("select BackwardRowNumberTable.RowNumber + {0} from ({1}) as BackwardRowNumberTable where BackwardRowNumberTable.Id = {2}", backwardCount, rowNumberTable, entity.Id);
                //获取前后记录
                string getResult = string.Format("select * from ({0}) as result where result.RowNumber between ({1}) and ({2})", rowNumberTable, getForwardRowNumber, getBackwardRowNumber);
                var result = Repository.SqlQuery(getResult).ToList();
                return result.Select(n => (T)(new T().ConvertAPIModel(n))).ToList(); ;
            }
            return null;
        }
    }
}
