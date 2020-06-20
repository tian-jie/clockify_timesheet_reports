
using Infrastructure.Web.Domain.Service;

using Innocellence.WeChat.Domain.Contracts;

namespace Innocellence.WeChatMain.Controllers
{    
    public partial class SalesTrainingArticleImagesController : ArticleImagesController
    {
        public SalesTrainingArticleImagesController(IArticleImagesService objService)
            : base(objService, (int)CategoryType.SalesTrainingCate)
        {
        }

    }

    public partial class NSCCateArticleImagesController : ArticleImagesController
    {
        public NSCCateArticleImagesController(IArticleImagesService objService)
            : base(objService, (int)CategoryType.NSCCate)
        {
        }

    }
    
}
