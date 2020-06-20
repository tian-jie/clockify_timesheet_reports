using EntityFramework.Extensions;
using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Innocellence.Activity.Service
{
    public class PollingResultService : BaseService<PollingResultEntity>, IPollingResultService
    {
        public PollingResultService()
            : base("CAAdmin")
        {
        }

        public new int InsertView(PollingResultView objModalSrc)
        {
            return AddOrUpdate(objModalSrc, true);

        }

        public int UpdateView(PollingResultView objModalSrc)
        {

            return AddOrUpdate(objModalSrc, false);

        }
        private int AddOrUpdate(PollingResultView objModalSrc, bool bolAdd)
        {
            PollingResultView objView = objModalSrc;
            if (objView == null)
            {
                return -1;
            }
            int iRet;
            var pollings = new List<PollingResultEntity>();
            PollingResultEntity polling = new PollingResultEntity();
            objView.AnswerResults.ToList().ForEach(x =>
            {
                polling = new PollingResultEntity
                {
                    PollingId = objModalSrc.PollingId,
                    QuestionId = x.QuestionId,
                    QuestionName = x.QuestionName,
                    Answer = x.Answer,
                    AnswerText = x.AnswerText,
                    UserId = objModalSrc.UserId
                };
                pollings.Add(polling);

                if (!bolAdd)
                {
                    // 应为没有批量更新
                    iRet = Repository.Update(polling);
                }

            });

            if (bolAdd)
            {
                iRet = Repository.Insert(pollings.AsEnumerable());
            }

            return 1;

        }

        public int GetUserCount(int id, string lillyid)
        {
            return Repository.Entities.Where(x => x.PollingId == id && x.UserId == lillyid && x.IsDeleted == false).Select(x => x.UserId).Count();
        }


        public List<PollingResultEntity> GetPollingReslutViews(int id, string lillyid)
        {
            return Repository.Entities.Where(x => x.PollingId == id && x.UserId == lillyid).ToList();
        }

        public List<T> GetListExport<T>(Expression<Func<PollingResultEntity, bool>> predicate) where T : IViewModel, new()
        {
            var lst = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).ToList();
            return lst;
        }

        public int DeleteByQuestionId(int id)
        {
            return Repository.Entities.Where(a => a.QuestionId == id).Delete();

            //return lst.Sum(l => Repository.Delete(l.Id));
        }

        public int DeleteByOptionId(int id)
        {

            return Repository.Entities.Where(a => a.Answer == id).Delete();
            //if (l == null)
            //{
            //    return i;
            //}
            //return Repository.Delete(l.Id);
        }
        public void GetUserList(List<PollingResultExportView> lists)
        {
            // 获取部门列表
            //List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
            //lists.ForEach(item =>
            //{
            //    var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(item.UserId.ToUpper()));
            //    if (emp != null)
            //    {
                  
            //        item.UserName = emp.name;
                   
            //    }
            //});
        }

        /// <summary>
        /// 根据pollingid和lillyid获取某个用户的某个polling的所有答案
        /// </summary>
        /// <param name="polling"></param>
        /// <param name="lillyid"></param>
        /// <returns></returns>
        public List<PollingResultEntity> GetList(int polling, string lillyid = null)
        {
            if (!string.IsNullOrEmpty(lillyid))
            {
                var r = Repository.Entities.Where(a => a.PollingId == polling && a.UserId == lillyid && a.IsDeleted == false).Select(a =>
                  new
                  {
                      Answer = a.Answer,
                      PollingId = a.PollingId,
                      QuestionId = a.QuestionId,
                      UserId = a.UserId
                  })
                .ToList();

                return r.Select(a => new PollingResultEntity()
                {
                    Answer = a.Answer,
                    PollingId = a.PollingId,
                    QuestionId = a.QuestionId,
                    UserId = a.UserId
                }).ToList();
            }
            else
            {
                var r = Repository.Entities.Where(a => a.PollingId == polling && a.IsDeleted == false).Select(a =>
                new
                {
                    Answer = a.Answer,
                    PollingId = a.PollingId,
                    QuestionId = a.QuestionId,
                    UserId = a.UserId
                })
                .ToList();

                return r.Select(a => new PollingResultEntity()
                {
                    Answer = a.Answer,
                    PollingId = a.PollingId,
                    QuestionId = a.QuestionId,
                    UserId = a.UserId
                }).ToList();
            }
        }

        /// <summary>
        /// group by questionid and userid
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public IList<PollingResultEntity> GetGroupPagingList(Expression<Func<PollingResultEntity, bool>> predicate, PageCondition con)
        {
            int total = con.RowCount;

            if (total <= 0)
            {

            }

            var source = Repository.Entities.Where(predicate)
                                            .GroupBy(x => new { x.QuestionId, x.UserId }).Select(x => new { x.Key.QuestionId, x.Key.UserId });

            //TODO: 
            //var source = Repository.Entities.Where(predicate)
            //    .GroupBy(x => new { x.UserId, x.QuestionId });

            source = source.OrderByDescending(m => m.QuestionId);

            var allCountQuery = source.FutureCount();

            var group = source.Skip((con.PageIndex - 1) * con.PageSize).Take(con.PageSize).Select(x => new { x.QuestionId, x.UserId }).Future().ToList();
            con.RowCount = allCountQuery.Value;

            var questions = group.Select(x => x.QuestionId).ToList();
            var users = group.Select(x => x.UserId).ToList();
            var list =
                Repository.Entities.Where(predicate)
                    .Where(x => questions.Contains(x.QuestionId) && users.Contains(x.UserId)).Select(x => new { x.QuestionId, x.UserId, x.Answer })
                    .ToList();

            return list.Select(x => new PollingResultEntity { QuestionId = x.QuestionId, Answer = x.Answer, UserId = x.UserId }).ToList();
        }

        public IList<PollingResultEntity> GetGroupPagingByUserId(Expression<Func<PollingResultEntity, bool>> predicate, PageCondition con)
        {
            //var query = Repository.Entities.Where(predicate)
            //                  .GroupBy(x => new { x.UserId }).Select(x => new { x.Key.UserId });
            //var q = query.FutureCount();

            //TODO: 
            var source = Repository.Entities.Where(predicate)
                .GroupBy(x => new { x.UserId }).Select(x => x.Key.UserId);

            source = source.OrderByDescending(m => m);
            var allCountQuery = source.FutureCount();
            var query = source.Skip((con.PageIndex - 1) * con.PageSize).Take(con.PageSize).Future();

            var total = allCountQuery.Value;
            var group = query.ToList();

            var users = group.Select(x => x).ToList();
            var list =
                Repository.Entities.Where(predicate)
                    .Where(x => users.Contains(x.UserId)).Select(x => new { x.QuestionId, x.UserId, x.Answer })
                    .ToList();

            con.RowCount = total;

            return list.Select(y => new PollingResultEntity { QuestionId = y.QuestionId, Answer = y.Answer, UserId = y.UserId }).ToList();
        }

        //paging
        public IList<PollingResultEntity> GetBatchResultsByPollingId(int id, int batchSize, QueryableGroup queryableGroup, Expression<Func<PollingResultEntity, dynamic>> selectExpression)
        {
            batchSize = 50000;
            var page = new PageCondition { PageSize = batchSize };
            var query = Repository.Entities.Where(x => x.PollingId == id && x.IsDeleted == false);
            var list = new List<PollingResultEntity>();

            while (true)
            {
                var fetchs =
                    query.OrderBy(x => x.Id)
                        .Skip(page.PageIndex * page.PageSize)
                        .Take(batchSize)
                        .Select(x => new { x.UserId, x.QuestionId, x.Answer, x.PollingId })
                        .ToList();

                list.AddRange(fetchs.Select(x => new PollingResultEntity { UserId = x.UserId, QuestionId = x.QuestionId, Answer = x.Answer, PollingId = x.PollingId }).ToList());

                if (fetchs.Count < batchSize)
                {
                    break;
                }
                page.PageIndex++;
            }

            return list;
        }
    }

    public class QueryableGroup
    {
        public Expression<Func<PollingResultEntity, dynamic>> GroupExpression { get; set; }

        public Expression<Func<IGrouping<dynamic, PollingResultEntity>, dynamic>> GroupSelectExpression { get; set; }
    }
}

