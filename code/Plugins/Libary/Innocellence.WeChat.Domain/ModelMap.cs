using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Web.Domain.Entity;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts.ViewModel;
using Innocellence.WeChat.Domain.ModelsView;
using Innocellence.WeChat.Domain.ViewModel;
//using Innocellence.WeChatMeeting.Domain.ViewModel;
//using Innocellence.WeChatMeeting.Domain.Entity;



namespace Innocellence.WeChat.Domain.Entity
{

    public class ModelMappers
    {
        public static void MapperRegister()
        {
            //Identity
            Mapper.CreateMap<ArticleInfoView, ArticleInfo>();

            Mapper.CreateMap<QuestionManageView, QuestionManage>();
            //   Mapper.CreateMap<SurveyView, Survey>();
            //   Mapper.CreateMap<SysUserView, SysUser>();
            //   Mapper.CreateMap<SysRoleView, SysRole>();
            Mapper.CreateMap<ArticleThumbsUpView, ArticleThumbsUp>();
            Mapper.CreateMap<MessageView, Message>();
            Mapper.CreateMap<WechatUserView, WechatUser>();
            Mapper.CreateMap<ThumbsUpCountView, ThumbsUp>();
            Mapper.CreateMap<SearchKeywordView, SearchKeyword>();
            Mapper.CreateMap<FaqInfoView, FaqInfo>();
            Mapper.CreateMap<ArticleImagesView, ArticleImages>();

            //vote
            Mapper.CreateMap<VoteView, VoteEntity>();
            Mapper.CreateMap<VoteQuestionView, VoteQuestionEntity>();
            Mapper.CreateMap<VoteAnswerView, VoteAnswerEntity>();
            Mapper.CreateMap<VoteReslutView, VoteResultEntity>();

            Mapper.CreateMap<AppMenuView, Category>();
            Mapper.CreateMap<FeedBackView, FeedBackEntity>();
            Mapper.CreateMap<PageReportGroupView, PageReportGroup>();
            Mapper.CreateMap<AccessDashboardView, AccessDashboard>();
            Mapper.CreateMap<FileManageView, FileManage>();

            Mapper.CreateMap<QrCodeView, QrCodeMPItem>();
            Mapper.CreateMap<ArticleCommentView, ArticleComment>();
            Mapper.CreateMap<MessageLogView, MessageLog>();
            Mapper.CreateMap<ArticleInfoReadHistoryView, ArticleInfoReadHistory>();
            //会议
            //Mapper.CreateMap<MeetingView,Meeting>();
            Mapper.CreateMap<SysWechatConfigView, SysWechatConfig>();
            Mapper.CreateMap<SysMenuView, SysMenu>();
        }

    }
}
