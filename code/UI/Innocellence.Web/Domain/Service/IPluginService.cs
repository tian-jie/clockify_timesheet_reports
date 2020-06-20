using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.Web.Entity;
using Innocellence.Web.Models.Plugins;
namespace Innocellence.Web.Service
{
    public interface IPluginService : IDependency, IBaseService<PluginModel>
    {

    }
}
