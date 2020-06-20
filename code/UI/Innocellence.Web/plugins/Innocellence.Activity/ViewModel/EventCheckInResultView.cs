using Infrastructure.Core;

namespace Innocellence.Activity.ModelsView
{
    public class EventCheckInResultView : IViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 用户LillyID
        /// </summary>
        public string LillyId { get; set; }
        /// <summary>
        /// 用户头像(圆形)
        /// </summary>
        public string UserLogoUrl { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public string EventStartTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public string EventEndTime { get; set; }
        /// <summary>
        /// 活动举办地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 活动描述
        /// </summary>
        public string EventDescription { get; set; }        

        public IViewModel ConvertAPIModel(object model)
        {
            throw new System.NotImplementedException();
        }
    }
}
