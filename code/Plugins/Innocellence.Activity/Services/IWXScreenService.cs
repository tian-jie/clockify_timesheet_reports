using Infrastructure.Core;
using Innocellence.Activity.Model;
using Innocellence.Activity.ViewModel;
using System.Collections.Generic;


namespace Innocellence.Activity.Services
{

    public interface IWXScreenService : IDependency, IBaseService<WXScreen>
    {
         void getEmplist(List<WXScreenView> lists);
    }
}