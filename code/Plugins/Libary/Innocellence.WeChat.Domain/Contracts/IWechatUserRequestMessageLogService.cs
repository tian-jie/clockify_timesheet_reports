using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IWechatUserRequestMessageLogService : IDependency, IBaseService<WechatUserRequestMessageLog>
    {
        List<T> GetList<T>(Expression<Func<WechatUserRequestMessageLog, bool>> predicate) where T : IViewModel, new();

        int Add(WechatUserRequestMessageLogView view);

        /// <summary>
        /// 根据指定id, 获取指定user, 指定位置的前/后n条数据.
        /// 当pageNumber = 0 时, 显示指定位置上下
        /// 当pageNumber < 0 时, 显示指定位置之前的 pageSize * |pageNumber| 条数据
        /// 当pageNumber > 0 时, 显示指定位置之后的 pageSize * |pageNumber| 条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="appId"></param>
        /// <param name="pageSize">每页显示多少条记录</param>
        /// <param name="pageNumber">第几页, 负数表示上翻, 正数表示下翻</param>
        /// <returns></returns>
        List<T> GetRecords<T>(int id, int appId, int pageSize, int pageNumber) where T : IViewModel, new();
    }
}
