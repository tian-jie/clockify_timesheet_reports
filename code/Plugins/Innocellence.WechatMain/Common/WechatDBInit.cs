//using Innocellence.Weixin.Entity;

namespace Innocellence.WeChatMain.Common
{
    public class WechatDBInit
    {

        public static void InitWechatDB()
        {

           // DatabaseInitializer.AddMapper(new SysWechatConfigConfiguration());


            //var d = new BaseService<SysWechatConfig>();
            //Innocellence.Weixin.CommonAPIs.AccessTokenDBContainer.FindTokenFromDB =
            //    (a) =>
            //    {
            //        return d.Repository.Entities.Where(e => e.WeixinCorpId == a && e.IsDeleted == false).FirstOrDefault();
            //    };

            //Innocellence.Weixin.CommonAPIs.AccessTokenDBContainer.UpdateTokenToDB =
            //   (a) =>
            //   {
            //       return d.Repository.Update((SysWechatConfig)a);
            //   };



        }

    }

    //public partial class SysWechatConfigConfiguration : EntityConfigurationBase<SysWechatConfig, Int32>
    //{
    //    /// <summary>
    //    /// 初始化一个<see cref="RoleConfiguration"/>类型的新实例
    //    /// </summary>
    //    public SysWechatConfigConfiguration()
    //    {
    //        SysWechatConfigConfigurationAppend();
    //    }

    //    /// <summary>
    //    /// 额外的数据映射
    //    /// </summary>
    //    partial void SysWechatConfigConfigurationAppend();

    //    partial void SysWechatConfigConfigurationAppend()
    //    {
    //        // HasRequired(m => m.Organization).WithMany(n => n.Roles);
    //    }
    //}
}