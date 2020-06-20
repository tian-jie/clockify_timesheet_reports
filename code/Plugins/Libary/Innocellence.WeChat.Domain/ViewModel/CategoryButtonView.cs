using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    /// <summary>
    /// just use for json
    /// </summary>
    public class CategoryButtonView : IViewModel
    {
        public CategoryButtonView()
        {
            children = new List<CategoryButtonView>();
        }

        public int Id { get; set; }

        public string type { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public int autoReplyId { get; set; }
        public List<CategoryButtonView> children { get; set; }


        public IViewModel ConvertAPIModel(object model)
        {
            return new CategoryButtonView();
        }
    }
}
