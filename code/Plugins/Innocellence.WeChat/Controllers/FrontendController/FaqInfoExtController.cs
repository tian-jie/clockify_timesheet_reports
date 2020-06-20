using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Innocellence.WeChat.Controllers
{
    public partial class HRESFAQController : FaqInfoController
    {
 
        public HRESFAQController(IFaqInfoService objService,ISearchKeywordService searchKeywordService)
            : base(objService, searchKeywordService)
        {
            _objService = objService;
            _searchKeywordService = searchKeywordService;
            AppId = (int)CategoryType.HREServiceCate;

        }
    }

    public partial class SPPFAQController : FaqInfoController
    {

        public SPPFAQController(IFaqInfoService objService, ISearchKeywordService searchKeywordService)
            : base(objService, searchKeywordService)
        {
            _objService = objService;
            _searchKeywordService = searchKeywordService;
            AppId = (int)CategoryType.SPPCate;
        }
    }
}
