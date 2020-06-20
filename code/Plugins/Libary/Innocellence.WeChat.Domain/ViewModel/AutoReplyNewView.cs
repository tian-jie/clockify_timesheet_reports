using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class AutoReplyNewView
    {
        public AutoReplyView Main { get; set; }

        /// <summary>
        /// //保存多条内容。只有新闻存在多条情况
        /// </summary>
        public List<NewsInfoView> Send { get; set; }

        public List<int> UserTags { get; set; }

        public int? MessageTags { get; set; }

        public int? UserGroups { get; set; }

        public string InterfaceLink { get; set; }

        public void SetExtraContent(List<AutoReplyContentView> contentList)
        {
            if (MessageTags == null || MessageTags == -1)
            {
                contentList[0].MessageTags = new List<int>();
            }
            else
            {
                contentList[0].MessageTags = new List<int> { MessageTags.Value };
            }
            if (UserGroups == null || UserGroups == -1)
            {
                contentList[0].UserGroups = new List<int>();
            }
            else
            {
                contentList[0].UserGroups = new List<int> { UserGroups.Value };
            }
            if (UserTags != null && UserTags.Count == 1 && UserTags[0] == -1)
            {
                contentList[0].UserTags = new List<int>();
            }
            else
            {
                contentList[0].UserTags = UserTags;
            }
            contentList[0].InterfaceLink = InterfaceLink;
        }

        public void GetExtraContent(List<AutoReplyContentView> contentList)
        {
            UserTags = contentList[0].UserTags;
            if (contentList[0].MessageTags == null || contentList[0].MessageTags.Count == 0)
            {
                MessageTags = -1;
            }
            else
            {
                MessageTags = contentList[0].MessageTags[0];
            }
            if (contentList[0].UserGroups == null || contentList[0].UserGroups.Count == 0)
            {
                UserGroups = -1;
            }
            else
            {
                UserGroups = contentList[0].UserGroups[0];
            }
            
            InterfaceLink = contentList[0].InterfaceLink;
            Main.TextKeyword = new List<string>();
            Main.MatchType = new List<int>();
            if (Main.Keywords != null)
            {
                foreach (var item in Main.Keywords)
                {
                    Main.TextKeyword.Add(item.Keyword);
                    Main.MatchType.Add(item.SecondaryType);
                }
            }
        }
    }
}
