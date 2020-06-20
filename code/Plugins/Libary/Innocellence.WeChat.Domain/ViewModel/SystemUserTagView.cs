using System;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ViewModel
{
    public class SystemUserTagView : IViewModel, IEntity
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsSelected { get; set; }
        public IViewModel ConvertAPIModel(object model)
        {
            var obj = (SystemUserTag)model;

            Id = obj.Id;
            ParentId = obj.ParentId;
            IsDeleted = obj.IsDeleted;
            Name = obj.Name;
            return this;
        }
    }


}
