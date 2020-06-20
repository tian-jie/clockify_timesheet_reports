using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class AppUserView : IViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string WeChatUserID { get; set; }

        public string EmailName { get; set; }

        public string Position { get; set; }

        public string MobileNumber { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new System.NotImplementedException();
        }
    }
}
