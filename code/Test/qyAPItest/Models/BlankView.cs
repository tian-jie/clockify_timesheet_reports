using System;
using Infrastructure.Core;


namespace qyAPITest.Controllers
{
    public class BlankView : IViewModel
    {
        public int Id { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (BlankEntity)model;
            Id = entity.Id;
          

            return this;
        }
    }
}
