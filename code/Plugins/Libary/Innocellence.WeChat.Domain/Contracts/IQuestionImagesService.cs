using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using System.Collections.Generic;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface IQuestionImagesService : IDependency, IBaseService<QuestionImages>
    {
        List<T> GetListByQuestionID<T>(int QuestionId) where T : IViewModel, new();
    }
}
