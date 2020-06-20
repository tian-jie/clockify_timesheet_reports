using System.IO;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Extensions;
using Infrastructure.Web.Net.WebPull.Images;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Service
{
    public class ReportService : BaseService<UserBehavior>, IReportService
    {
        public ReportService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}