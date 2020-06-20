using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Infrastructure.Web.Domain.ModelsView;
using Infrastructure.Web.Domain.Service;
using Innocellence.WeChat.Domain.ModelsView;
using Infrastructure.Utility.Data;
using Infrastructure.Web.UI;
using Innocellence.WeChat.Domain.Service;
using Innocellence.WeChat.Domain.Common;
using Innocellence.Weixin.QY.Entities;
using Infrastructure.Web.ImageTools;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Infrastructure.Web.Domain.Service.Common;
using Innocellence.WeChat.Domain.Contracts.ViewModel;

namespace Innocellence.WeChat.Controllers
{
    public partial class HRESQuestionController : QuestionManageController
    {

        public HRESQuestionController(IQuestionManageService objService)
            : base(objService)
        {
            _objService = objService;
            AppId = (int)CategoryType.HREServiceCate;
        }
    }
}