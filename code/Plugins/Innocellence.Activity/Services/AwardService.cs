using Infrastructure.Core.Data;
using Innocellence.Activity.Entity;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using System;

namespace Innocellence.Activity.Service
{
    public class AwardService : BaseService<AwardEntity>, IAwardService
    {
        public AwardService()
            : base("CAAdmin")
        {
        }
        //public new int InsertView(int num,AwardView objModalSrc)
        //{
        //    return AddOrUpdate(num, objModalSrc, true);
        //}

        //public new int UpdateView(int num,AwardView objModalSrc)
        //{
        //    return AddOrUpdate(num, objModalSrc, false);
        //}
        private int AddOrUpdate(int num, AwardView objModalSrc, bool bolAdd)
        {
            AwardView objView = objModalSrc;
            if (objView == null)
            {
                return -1;
            }
            int iRet;
            AwardEntity award = new AwardEntity();
            for (int i = 0; i < num; i++)
            {
                award = new AwardEntity
                {
                    PollingId = objModalSrc.PollingId,
                    Type = "polling",
                    SecurityCode = NumberCheck(7,true),
                    Status = "已中奖",
                    AccessDate = DateTime.Now
                };

                if (bolAdd)
                {

                    iRet = Repository.Insert(award);

                }
                else
                {
                    iRet = Repository.Update(award);
                }
            }

            return 1;

        }
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public string NumberCheck(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

    }
}
