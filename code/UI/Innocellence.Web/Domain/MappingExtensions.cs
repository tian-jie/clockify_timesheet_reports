using System;
using AutoMapper;
using Innocellence.Web.Models.Plugins;
using Infrastructure.Core.Plugins;


namespace Innocellence.Web.Extensions
{
    public static class MappingExtensions
    {

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return entity.MapTo<PluginDescriptor, PluginModel>();
        }

        public static void MapperRegister()
        {
            //Identity
            Mapper.CreateMap<PluginDescriptor, PluginModel>();


            //// Mapper.CreateMap<SurveyView, Survey>();
            //Mapper.CreateMap<SysUserView, SysUser>();
            //Mapper.CreateMap<SysRoleView, SysRole>();
            ////  Mapper.CreateMap<ArticleThumbsUpView, ArticleThumbsUp>();
            ////  Mapper.CreateMap<MessageView, Message>();
            //Mapper.CreateMap<LogsView, Logs>();
            ////   Mapper.CreateMap<FeedbackView, Feedback>();
            //Mapper.CreateMap<CategoryView, Category>();
            //   Mapper.CreateMap<WechatUserView, WechatUser>();
            //   Mapper.CreateMap<ToolsView, Tools>();
        }

        #endregion
    }


}