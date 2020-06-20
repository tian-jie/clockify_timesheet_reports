using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;

namespace Innocellence.Activity.Contracts.ViewModel
{
    public class PollingScreenView : IViewModel
    {
        public int Id { get; set; }
        public int PollingTotal { get; set; }

        public IList<SereisData> SereisData { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            throw new System.NotImplementedException();
        }

        IViewModel IViewModel.ConvertAPIModel(object model)
        {
            throw new System.NotImplementedException();
        }
    }
}
