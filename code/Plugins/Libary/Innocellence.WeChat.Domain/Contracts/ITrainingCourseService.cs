using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.ModelsView;

namespace Innocellence.WeChat.Domain.Contracts
{
    public interface ITrainingCourseService : IDependency, IBaseService<TrainingCourse>
    {
        //List<CourseDateRange> GetDateListByCode<T>(string coursecode);
        //List<T> GetListByCode<T>(string coursecode) where T : IViewModel, new();
        //TrainingCourse MapTrainingCourse(TrainingCourseView objModal, TrainingCourse obj, bool IsEnglish);

        //List<CourseDateCourseView> GetListDate(DateTime dtStart, DateTime dtEnd);

        //List<Innocellence.WeChat.Domain.ViewModelFront.TrainingCourseView> GetListByCondition(
        //    DateTime dtStart, List<int> lstIDs, int? role, int? skill,
        //    string strLanguage, string searchStr, string strUserID, bool myCourse);
    }
}
