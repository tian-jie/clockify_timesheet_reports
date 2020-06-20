using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ISurveyService : IDependency, IBaseService<Survey>
    {
        List<T> GetListByCode<T>(string Surveycode) where T : IViewModel, new();
    }
}
