using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IQrCodeService : IDependency, IBaseService<QrCodeMPItem>
    {
        int GenerateSceneId();
        void EditDescription(QrCodeView model, string userName);
        void AddOrCode(QrCodeView model, string userName);

        List<QrCodeView> GetListAll(int pageIndex, int pageSize, ref int total, string strDescription, int appId);
    }
}
