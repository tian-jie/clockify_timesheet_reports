using Infrastructure.Core;
using Innocellence.WeChat.Domain.ViewModel;
using Innocellence.Weixin.Entities;
using Innocellence.Weixin.QY.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MP = Innocellence.Weixin.MP;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IWeChatUserRequestMessageLogHandler : IDependency
    {

        void WriteRequestLog(IRequestMessageBase requestMessage, string appId);

        void WriteRequestLogMP(IRequestMessageBase requestMessage, string appId);

        void WriteResponseLog(List<IResponseMessageBase> responseList, string appId);

        Task<WechatUserRequestMessageLogView> WriteResponseLogMP(List<IResponseMessageBase> responseList, string appId, bool isAutoReply);
    }
}
