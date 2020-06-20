using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Infrastructure.Core;
using System.ComponentModel.DataAnnotations;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Category")]
    /// <summary>
    /// ʾ����
    ///     1. ����APP�����ţ�(http://chinaca-zgkx.XXX.com:25001/News/ArticleInfo/wxdetail/15)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: News/ArticleInfo/wxdetail
    ///         Content : 15
    ///         
    ///     2. ��˵���(����ҳView��������ҳ����ͬ1)
    ///     
    ///     3. ��˵���(click event������CORP_NEWS�˵�)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: CORP_NEWS
    ///         Content : ��
    ///     
    ///     4. ����Ϣ��(click event��������Ϣ������Hello)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: TEXT_EVENT
    ///         Content : Hello
    ///     
    ///     5. ������ҳ��(����1��https://www.chinacmp.XXX.com:25001/News/ArticleInfo/Edit/15)
    ///         UserID  : WeChatUserID
    ///         AppID   : 6
    ///         FunctionID: News/ArticleInfo/Edit
    ///         Content : 15
    /// </summary>
    public partial class UserBehavior : EntityBase<int>
	{
        public override Int32 Id { get; set; }
        /// <summary>
        /// WeChatUserID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ΢����ҵ�ŵ�APPID����������6��
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// ����
        ///     �����ţ�����url��һ����News/ArticleInfo/wxdetail
        ///     ��˵������ò˵���ID��CORP_NEWS��
        ///     ����Ϣ����������Ϣ��TEXT����ͼƬ��Ϣ��NEWS����������Ϣ��Voice����ÿ����Ϣ���͸�һ���̶�ID
        ///     ������ҳ������ҳ�бȽ��д����Ե��ַ���
        /// </summary>
        public string FunctionId { get; set; }
        /// <summary>
        /// �������Ǹ������У��������Ϣ
        ///     �翴���ţ���url��News/ArticleInfo/wxdetail/15������15
        ///     ��˵���������Ϣ�������������Ϣ������
        /// </summary>
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Device { get; set; }
        public string ClientIp { get; set; }
        //��������
        ///     1. URL�д�News ��ArticleInfo���и���id����Article��Ϣ

        ///     2. URL�д�Message ��Message���и���id����Message��Ϣ
        /// 
        ///     3. URL�д� QuestionDetail  ��QuestionManage���и���id����question��Ϣ
        ///     
        ///     4. ��˵���(click event������CORP_NEWS�˵�)

        ///     5. URL�д�wxlink
        /// 
        ///     6. faq ��index��
        /// 
        ///     7. ���߷��� ��raisequestion��/feedback���û�������
        /// 
        ///     8. ����Ϣ��(click event��������Ϣ������Hello)

        ///     9. URL�д�caadmin
        
        ///     10. ����app
        
        ///     11.URL�д� FaqInfo ��FaqInfo���и���id����Faq��Ϣ
       
        ///     12.URL�д� EventManage   ��Event���и���id����Event��Ϣ��ע����ǩ����
        public int? ContentType { get; set; }
	}
}
