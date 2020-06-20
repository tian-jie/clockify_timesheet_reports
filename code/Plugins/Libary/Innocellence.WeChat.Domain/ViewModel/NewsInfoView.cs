using Infrastructure.Core;
using Innocellence.WeChat.Domain.Common;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class NewsInfoView : IViewModel
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string NewsCate { get; set; }
        public string NewsTitle { get; set; }
        public string NewsCode { get; set; }
        [JsonIgnore]
        public string[] SendToPerson { get; set; }
        [JsonIgnore]
        public string[] SendToGroup { get; set; }
        [JsonIgnore]
        public string[] SendToTag { get; set; }
        [JsonIgnore]
        public string[] SendToPersonName { get; set; }
        [JsonIgnore]
        public string[] SendToGroupName { get; set; }
        [JsonIgnore]
        public string[] SendToTagName { get; set; }
        public string ImageContent { get; set; }
        public string VideoContent { get; set; }
        public string SoundSrc { get; set; }
        public string ImageSrc { get; set; }
        public string FileSrc { get; set; }
        public string Size { get; set; }
        public string Duration { get; set; }
        public string RealFileName { get; set; }
        public string NewsComment { get; set; }
        public string NewsContent { get; set; }
        public string MediaId { get; set; }

        public long MediaCreateTime { get; set; }

        public string FeedBackBtnContent { get; set; }
        public int IsUseComment { get; set; }

        public int? Group { get; set; }
        public int? Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

        //是否保密
        public bool? isSecurityPost { get; set; }

        public int? materialId { get; set; }

        public int SecurityLevel { get; set; }
        public int PostType { get; set; }
        public ScheduleSendTimeView ScheduleSendTime { get; set; }

        /// <summary>
        /// 口令回复用，多条新闻ID
        /// </summary>
        public string  NewsID { get; set; }

        public int SecondaryType { get; set; }

        public bool? IsLike { get; set; }

        public bool? ShowComment { get; set; }

        public bool? ShowReadCount { get; set; }
        public bool? IsWatermark { get; set; }
        public bool? NoShare { get; set; }
        public string ArticleURL { get; set; }

        public WechatMessageLog ConvertToEntity()
        {
            return new WechatMessageLog() { AppID = this.AppId, CreatedTime = DateTime.Now, ContentType = (int)Enum.Parse(typeof(WechatMessageLogType), this.NewsCate) };
        }

        public ArticleInfoView ConvertToEntityArticle()
        {
            return new ArticleInfoView()
            {
                AppId = this.AppId,
                ContentType = (int)Enum.Parse(typeof(WechatMessageLogType), this.NewsCate),
                ArticleComment = this.NewsComment,
                ArticleContent = this.NewsContent,
                ArticleStatus = "Published",
                ArticleTitle = this.NewsTitle,
                ArticleType = 1,
                ImageCoverUrl = this.ImageContent,
                City = this.City,
                Sex = this.Sex,
                Province = this.Province,
                Group = this.Group,
                ToDepartment =(SendToGroup==null?"": string.Join(",", this.SendToGroup)),
                ToTag = (SendToTag == null ? "" : string.Join(",", this.SendToTag)),
                ToUser = (SendToPerson == null ? "" : string.Join(",", this.SendToPerson))

            };
        }

        public IViewModel ConvertAPIModel(object model)
        {
            return JsonConvert.DeserializeObject<NewsInfoView>(((WechatMessageLog)model).Content);
        }
    }

    public static class NewsInfoViewExtension
    {
        public static string Filter(this NewsInfoView model, string source, string targetType)
        {
            var currentType = model.NewsCate;
            if (currentType.Equals(targetType, StringComparison.CurrentCultureIgnoreCase))
            {
                return source;
            }
            else
            {
                return string.Empty;
            }
        }
    }


}
