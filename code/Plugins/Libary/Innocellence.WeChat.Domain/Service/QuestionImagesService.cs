using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——在线问答
    /// </summary>
    public partial class QuestionImagesService : BaseService<QuestionImages>, IQuestionImagesService
    {
        public QuestionImagesService()
            : base("CAAdmin")
        {
            
        }
        //搜出图片列表
        public List<T> GetListByQuestionID<T>(int QuestionID) where T : IViewModel, new()
        {
            
            var ens = Repository.Entities.Where(a => a.QuestionID == QuestionID).ToList();

            var lst = ens.Select(n => (T)(new T().ConvertAPIModel(n))).ToList();

            return lst;
           
        }

    }
}