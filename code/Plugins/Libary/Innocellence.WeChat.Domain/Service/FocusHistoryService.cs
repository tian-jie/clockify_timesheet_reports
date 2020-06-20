using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class FocusHistoryService : BaseService<FocusHistory>, IFocusHistoryService
    {
        public FocusHistoryService(IUnitOfWork unitOfWork)
            : base("CAAdmin")
        {

        }

        public FocusHistoryService()
            : base("CAAdmin")
        {

        }
    }
}
