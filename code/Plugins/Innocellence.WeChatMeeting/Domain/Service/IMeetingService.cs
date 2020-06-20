using Infrastructure.Core;
using Innocellence.WeChatMeeting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChatMeeting.Domain.Service
{
    public interface IMeetingService : IDependency, IBaseService<Meeting>
    {
        
    }
}
