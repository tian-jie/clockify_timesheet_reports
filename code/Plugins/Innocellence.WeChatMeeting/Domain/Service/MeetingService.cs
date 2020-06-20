using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Filter;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChatMeeting.Domain.Entity;
using Innocellence.WeChatMeeting.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Innocellence.WeChatMeeting.Domain.Service
{
    public partial class MeetingService : BaseService<Meeting>, IMeetingService
    {

    }
}