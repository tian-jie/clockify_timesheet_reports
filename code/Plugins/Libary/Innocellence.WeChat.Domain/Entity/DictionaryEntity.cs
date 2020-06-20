using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
    public class DictionaryEntity : EntityBase<int>
    {
        public override int Id { get; set; }

        public string CategoryCode { get; set; }

        public string TypeCode { get; set; }

        public string CategoryName { get; set; }

        public string TypeName { get; set; }
    }
}
