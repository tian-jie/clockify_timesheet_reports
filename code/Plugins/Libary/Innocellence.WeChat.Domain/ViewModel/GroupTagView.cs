using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class GroupTagView: IViewModel
    {
        public int count { get; set; }
        public int Id { get; set; }
        public string name { get; set; }
        public bool IsSelected { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (GroupTagView)obj;
            Id = entity.Id;
            name = entity.name;
            IsSelected = entity.IsSelected;
            count = entity.count;

            return this;
        }
    }
}
