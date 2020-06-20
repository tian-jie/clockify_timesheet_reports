using EntityFramework.Extensions;
using Infrastructure.Core;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Data;
using Infrastructure.Utility.Data;
using Innocellence.Activity.Contracts.Entity;
using Innocellence.Activity.Contracts.ViewModel;
using Innocellence.Activity.Model;
using Innocellence.Activity.ModelsView;
using Innocellence.Activity.Services;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebGrease.Css.Extensions;

namespace Innocellence.CA.Service
{
    public class PollingService : BaseService<PollingEntity>, IPollingService
    {
        private readonly IPollingQuestionService _pollingQuestionService;
        private readonly IPollingOptionService _pollingOptionService;
        private readonly IPollingResultService _pollingResultService;
        private readonly IPollingResultTempService _pollingResultTempService;
        private readonly IPollingAnswerService _pollingAnswerService;
        private PollingView _pollingView = null;
        private IList<PollingResultEntity> _pollingResultView;
        private IList<PollingResultTempView> _pollingResultTempView;
        private ICacheManager _cacheManager;
        private const string CachePrefix = "polling_view_{0}";

        //TODO: linq does not support cross db content instance query
        private const string SqlQueryTemplate = @"SELECT * FROM
(SELECT  s.pollingId ,
        s.OptionName,
        SUM(s.c) AS sort,
  s.orderIndex
FROM    ( SELECT    Polling.Id AS pollingId ,
                    PollingQuestion.Id AS qustionId ,
                    PollingOption.Id AS optionId ,
                    OptionName ,
     PollingOption.OrderIndex AS orderIndex,
                    ( SELECT    COUNT(1)
                      FROM      dbo.PollingResult
                      WHERE     PollingId IN ( {0})
                                AND Answer = dbo.PollingOption.Id
                    ) AS c
          FROM      dbo.Polling
                    LEFT JOIN dbo.PollingQuestion ON PollingQuestion.PollingId = Polling.Id
                    LEFT JOIN dbo.PollingOption ON PollingOption.QuestionId = PollingQuestion.Id
          WHERE     Polling.Id IN ( {0} )
        ) AS s
GROUP BY s.pollingId ,
        s.OptionName,s.orderIndex) AS s2 ORDER BY s2.orderIndex;";

        public PollingService(IPollingQuestionService pollingQuestionService,
            IPollingOptionService pollingOptionService,
             IPollingResultService pollingResultService,
            IPollingAnswerService pollingAnswerService,
            IPollingResultTempService pollingResultTempService,
            ICacheManager cacheManager)
            : base("CAAdmin")
        {
            _pollingQuestionService = pollingQuestionService;
            _pollingOptionService = pollingOptionService;
            _pollingResultService = pollingResultService;
            _pollingAnswerService = pollingAnswerService;
            _pollingResultTempService = pollingResultTempService;
            _cacheManager = cacheManager;
        }

        public List<PollingView> QueryList(Expression<Func<PollingEntity, bool>> predicate)
        {
            var lst = Repository.Entities.Where(predicate).Where(a => a.IsDeleted != true).OrderByDescending(x => x.Id).ToList()
                .Select(n => (PollingView)(new PollingView().ConvertAPIModel(n)))
                .ToList();
            return lst;
        }

        public override int InsertView<T>(T objModalSrc)
        {
            int iRet;
            var objModal = (PollingView)(IViewModel)objModalSrc;

            objModal.GuiId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            iRet = base.InsertView(objModal);
            return iRet;
        }

        public int InsertOrUpdateView<T>(T objModalSrc)
        {
            var polling = objModalSrc as PollingView;
            if (polling == null)
            {
                return 0;
            }

            if (polling.Id == 0)
            {
                return InsertView(polling);
            }
            else
            {
                return UpdateView(polling);
            }
        }

        public override int UpdateView<T>(T objModalSrc)
        {
            var pollingView = (PollingView)(IViewModel)objModalSrc;
            // 在后台根据传过来的值区分哪些option or question是插入哪些是更新

            // 先从后台拉一下当前的数据，组一个oldPollingView
            var oldPollingView = new PollingView();
            oldPollingView.ConvertAPIModel(Repository.Entities.AsNoTracking().FirstOrDefault(a => a.Id == pollingView.Id && a.IsDeleted != true));
            oldPollingView.PollingQuestions = _pollingQuestionService.GetPollingQuestion(pollingView.Id);
            foreach (var oldQuestion in oldPollingView.PollingQuestions)
            {
                oldQuestion.PollingOptionEntities = _pollingOptionService.GetPollingOptions(oldQuestion.Id);
            }

            // 因为直接更新会级联更新，所以这里把孩子都删了更新，然后再加上孩子们
            var newQuestions = pollingView.PollingQuestions;
            pollingView.PollingQuestions = null;
            base.UpdateView(pollingView, new List<string> { "Name", "Type", "StartDateTime", "EndDateTime", "StandardScore", "ReplyMessage" });
            pollingView.PollingQuestions = newQuestions;

            var allUpdates = newQuestions.AsParallel().Where(x => oldPollingView.PollingQuestions.AsParallel().Any(y => x.Id == y.Id && (x.Score != y.Score || string.Compare(x.RightAnswers, y.RightAnswers, StringComparison.OrdinalIgnoreCase) != 0))).Select(x => new { x.Id, x.Score, x.RightAnswers }).ToList();

            if (allUpdates.Any())
            {
                var ids = allUpdates.Select(x => x.Id).ToList();
                var oldAnswers = _pollingAnswerService.Repository.Entities.Where(x => ids.Contains(x.QuestionId) && x.IsDeleted != true).ToList();
                var updateScoreQuestionList = allUpdates.AsParallel().Where(x => oldPollingView.PollingQuestions.AsParallel().Any(y => x.Id == y.Id && x.Score != y.Score)).Select(x => new { x.Id, x.Score }).ToList();
                var updateRightAnswerQuestionList = allUpdates.AsParallel().Where(x => oldPollingView.PollingQuestions.AsParallel().Any(y => x.Id == y.Id && string.Compare(x.RightAnswers, y.RightAnswers, StringComparison.OrdinalIgnoreCase) != 0)).Select(x => new { x.Id, x.RightAnswers }).ToList();

                if (updateScoreQuestionList.Any() && oldAnswers.Any())
                {
                    Parallel.ForEach(oldAnswers, x =>
                       {
                           var newQuestion = updateScoreQuestionList.AsParallel().First(y => y.Id == x.QuestionId);
                           x.Score = newQuestion.Score.GetValueOrDefault();
                       });
                    oldAnswers.ForEach(x => _pollingAnswerService.Repository.Update(x, new List<string> { "Score" }));
                }

                if (updateRightAnswerQuestionList.Any() && oldAnswers.Any())
                {
                    Parallel.ForEach(oldAnswers, x =>
                    {
                        var newQuestion = updateRightAnswerQuestionList.AsParallel().First(y => y.Id == x.QuestionId);

                        x.Status = string.Compare(x.SelectAnswer, newQuestion.RightAnswers, StringComparison.OrdinalIgnoreCase) == 0;
                        x.RightAnswer = newQuestion.RightAnswers;
                    });
                    oldAnswers.ForEach(x => _pollingAnswerService.Repository.Update(x, new List<string> { "RightAnswer", "Status" }));
                }
            }


            // 检查OldPollingView相对于NewPollingView删除了哪些
            foreach (var oldQuestion in oldPollingView.PollingQuestions)
            {
                // 如果id不在新polling里，就是删掉了
                var sameQuestionInNew = pollingView.PollingQuestions.FirstOrDefault(a => a.Id == oldQuestion.Id && a.IsDeleted != true);
                if (sameQuestionInNew == null)
                {
                    _pollingQuestionService.Repository.Delete(oldQuestion.Id);
                    // 如果删除了question的话，会自动删除option
                    // 我们再次手动删除answer表里相关的option
                    _pollingResultService.DeleteByQuestionId(oldQuestion.Id);
                    PollingQuestionView question = oldQuestion;
                    _pollingAnswerService.Repository.Entities.Where(x => x.QuestionId == question.Id).Delete();

                    continue;
                }
                // 如果没删，检查option。
                // 如果option被删了，同时去删除用户填写过答案的answer
                foreach (var oldOption in oldQuestion.PollingOptionEntities)
                {
                    if (sameQuestionInNew.PollingOptionEntities == null || sameQuestionInNew.PollingOptionEntities.FirstOrDefault(a => a.Id == oldOption.Id && a.IsDeleted != true) == null)
                    {
                        _pollingOptionService.Repository.Delete(oldOption.Id);
                        _pollingResultService.DeleteByOptionId(oldOption.Id);
                        PollingOptionView option = oldOption;
                        _pollingAnswerService.Repository.Entities.Where(x => x.QuestionId == option.Id).Delete();
                    }
                }
            }

            // 除了删除的，就是添加和更新的
            // 检查NewPollingView相对于OldPollingView添加了哪些

            foreach (var newQuestion in pollingView.PollingQuestions)
            {
                // 如果id是0，肯定新加的，执行级联添加
                if (newQuestion.Id == 0)
                {
                    newQuestion.PollingId = pollingView.Id;
                    _pollingQuestionService.InsertView(newQuestion);
                    continue;
                }
                // 如果不是0，更新自己，并且检查option
                var options = newQuestion.PollingOptionEntities;
                newQuestion.PollingOptionEntities = null;
                _pollingQuestionService.UpdateView(newQuestion);
                newQuestion.PollingOptionEntities = options;

                if (newQuestion.PollingOptionEntities != null && newQuestion.PollingOptionEntities.Any())
                {
                    foreach (var newOption in newQuestion.PollingOptionEntities)
                    {
                        if (newOption.Id == 0)
                        {
                            newOption.QuestionId = newQuestion.Id;
                            _pollingOptionService.InsertView(newOption);
                        }
                        else
                        {
                            _pollingOptionService.UpdateView(newOption, new List<string> { "OptionName", "Picture", "Type" });
                        }
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// 后台问题列表界面
        /// </summary>
        /// <param name="query"></param>
        /// <param name="lillyid"></param>
        /// <returns></returns>
        public PollingView GetPersonCount(int pollingId, string lillyid = null)
        {

            var pol = GetPollingView(pollingId);

            if (pol.PollingQuestions != null && pol.PollingQuestions.Count > 0)
            {
                var resultGroup = GetResultGroups(pollingId);

                if (pol.PollingQuestions != null && pol.PollingQuestions.Count > 0)
                {
                    pol.PollingQuestions.ToList().ForEach(x => GetOptionPercent(x.PollingOptionEntities, resultGroup));
                }

                // 后台有奖问答正确人数/答题人数计算
                // 非有奖问答不走这个逻辑
                if (pol.Type == 1)
                {
                    //var lst = PollingResultPerson(pol);

                    //pol.PollingQuestions.ToList().ForEach(x =>
                    //{
                    //    lst.Where(a => a.QuestionId == x.Id).ToList().ForEach(y =>
                    //    {
                    //        x.rightPersons = y.RightPersons;
                    //        x.answerPersons = y.answerPersons;
                    //    });

                    //});

                    var query = _pollingAnswerService.Repository.Entities.Where(x => x.PollingId == pollingId && x.IsDeleted != true);
                    var totalQuery = query.GroupBy(x => x.QuestionId).Select(x => new { x.Key, count = x.Count(), rightCount = x.Count(y => y.Status) }).Future();
                    //var rightCountList = query.Where(x => x.Status).GroupBy(x => x.QuestionId).Select(x => new { x.Key, count = x.Count() }).Future().ToList().AsParallel();
                    //var totalCountList = totalQuery.ToList().AsParallel();
                    var list = totalQuery.ToList().AsParallel();

                    Parallel.ForEach(pol.PollingQuestions.AsEnumerable(), x =>
                    {
                        var item = list.FirstOrDefault(y => y.Key == x.Id);
                        x.rightPersons = item == null ? 0 : item.rightCount;
                        x.answerPersons = item == null ? 0 : item.count;
                    });

                }

                pol.PollingTotal = GetPollingTotal(pol.Id);
                return pol;
            }
            return pol;
        }

        /// <summary>
        /// 根据Guid获取PollingView
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PollingView GetPollingView(string guid)
        {
            var polling = Repository.Entities.Where(a => a.IsDeleted != true && a.GuiId.ToString() == guid).Include(x => x.PollingQuestions.Select(y => y.PollingOptionEntities)).FirstOrDefault();

            if (polling != null)
            {
                return GetPollingViewPrivate(polling);
            }

            return default(PollingView);
        }

        /// <summary>
        /// 适用于后台编辑页面（纯polling信息）
        /// </summary>
        /// <param name="pollingId"></param>
        /// <returns></returns>
        public PollingView GetPollingView(int pollingId)
        {
            var polling = Repository.Entities.Where(a => a.IsDeleted != true && a.Id == pollingId).Include(x => x.PollingQuestions.Select(y => y.PollingOptionEntities)).FirstOrDefault();

            if (polling != null)
            {
                return GetPollingViewPrivate(polling);
            }

            return default(PollingView);
        }

        private PollingView GetPollingViewPrivate(PollingEntity polling)
        {
            var pollingview = (PollingView)new PollingView().ConvertAPIModel(polling);
            _pollingView = pollingview;
            var pqv = new List<PollingQuestionView>();

            if (polling.PollingQuestions != null)
            {
                var pollingQuestion = polling.PollingQuestions.Where(a => a.IsDeleted != true).OrderBy(a => a.OrderIndex).ThenBy(a => a.Id).ToList();

                foreach (PollingQuestionEntity poll in pollingQuestion)
                {
                    var pollingQuestionView = (PollingQuestionView)new PollingQuestionView().ConvertAPIModel(poll);
                    var pov = poll.PollingOptionEntities.Where(a => a.IsDeleted != true).Select(
                            polloption => (PollingOptionView)new PollingOptionView().ConvertAPIModel(polloption))
                            .OrderBy(a => a.OrderIndex).ThenBy(a => a.Id).ToList();
                    pollingQuestionView.PollingOptionEntities = pov;

                    pqv.Add(pollingQuestionView);
                }
            }

            pollingview.PollingQuestions = pqv;

            return pollingview;
        }

        /// <summary>
        /// 适用于前台投票提交后页面（投票）
        /// </summary>
        /// <param name="query"></param>
        /// <param name="LillyId"></param>
        /// <returns></returns>
        public PollingView GetPollingVote(int pollingId, string LillyId)
        {
            PollingView pollingview = GetPollingDetailView(pollingId);

            //var vote = _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingview.Id && x.UserId == LillyId)
            // .Select(x => new { UserId = x.UserId.ToUpper(), Answer = x.Answer, AnswerText = x.AnswerText, QuestionId = x.QuestionId }).ToList();

            //refactor by john xie
            var query = _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingview.Id && x.IsDeleted != true);
            var voteQuery = query.Where(x => x.UserId == LillyId).Select(
                        x =>
                            new
                            {
                                UserId = x.UserId.ToUpper(),
                                x.Answer,
                                x.AnswerText,
                                x.QuestionId
                            }).Future();
            var groupQuery = query.GroupBy(x => new { x.QuestionId, x.Answer }).Select(a => new { a.Key.QuestionId, a.Key.Answer, Count = a.Count() }).Future();

            var vote = voteQuery.ToList();
            var group = groupQuery.ToList().Select(x => new ResultGroup
            {
                AnswerId = x.Answer,
                CountNumber = x.Count,
                QuestionId = x.QuestionId
            }).ToList();

            if (pollingview.PollingQuestions != null && pollingview.PollingQuestions.Count > 0)
            {
                pollingview.PollingQuestions.ToList().ForEach(x =>
                {
                    x.PollingOptionEntities.ForEach(y =>
                    {
                        if (vote.Where(v => v.UserId == LillyId).Any(z => z.Answer == y.Id))
                        {
                            y.SelectName = "(已选)";
                        }
                    });

                    x.PollingQuestionResult = vote.Where(m => m.Answer == 0 && m.QuestionId == x.Id).Select(n => n.AnswerText).FirstOrDefault();//文本题答案

                    //百分比
                    GetOptionPercent(x.PollingOptionEntities, group); //给每个选项都加上百分数和票数

                });

            }

            return pollingview;
        }

        /// <summary>
        /// 前台投票前页面
        /// </summary>
        /// <param name="query"></param>
        /// <param name="lillyid"></param>
        /// <param name="pollingId"></param>
        /// <returns></returns>
        public PollingView GetPollingDetailView(int pollingId)
        {
            return _cacheManager.Get(string.Format(CachePrefix, pollingId), 1, () =>
                {
                    PollingView pollingview = GetPollingView(pollingId);
                    if (pollingview.PollingQuestions != null && pollingview.PollingQuestions.Count > 0)
                    {
                        pollingview.PollingQuestions.ToList().ForEach(x =>
                        {
                            if (x.Type == 1)
                            {
                                x.optionName = "(单选)";

                            }
                            else if (x.Type == 0 || x.Type == x.PollingOptionEntities.Count)
                            {
                                x.optionName = "(多选)";
                            }
                            else if (x.Type < x.PollingOptionEntities.Count)
                            {
                                x.optionName = "(最多选" + x.Type + "项)";
                            }
                            else if (x.Type == 999)
                            {
                                x.optionName = "文本";
                            }
                        });
                    }
                    if (pollingview.Type == 1)
                    {
                        pollingview.Title = "有奖问答";
                    }
                    else if (pollingview.Type == 2)
                    {
                        pollingview.Title = "投票";
                    }
                    if (pollingview.StartDateTime > DateTime.Now)
                    {
                        pollingview.StatusName = "未开始";
                    }
                    else if (pollingview.EndDateTime > DateTime.Now && pollingview.StartDateTime < DateTime.Now)
                    {
                        pollingview.StatusName = "提交";
                    }
                    else if (pollingview.EndDateTime < DateTime.Now)
                    {
                        pollingview.StatusName = "已结束";
                    }
                    return pollingview;
                });


        }

        /// <summary>
        /// 后台大屏幕显示界面
        /// </summary>
        /// <param name="pollingid"></param>
        /// <param name="questionid"></param>
        /// <returns></returns>
        public PollingScreenView GetPollingScreenData(int pollingid, int questionid)
        {
            // TODO: check performance
            var pollingScreenView = new PollingScreenView();

            var query = _pollingResultService.Repository.Entities.Where(
                x => x.PollingId == pollingid && x.IsDeleted != true);

            var allAnswersQuery = query.Where(x => x.QuestionId == questionid).GroupBy(x => x.Answer)
                    .Select(x => new
                            {
                                x.Key,
                                count = x.Count()
                            }).Future();

            var totalQuery = query.Select(x => x.UserId.ToUpper()).Distinct().FutureCount();

            var allAnswers = allAnswersQuery.ToList();

            pollingScreenView.PollingTotal = totalQuery.Value;//最新的投票人数

            var sereisLst = new List<SereisData>();

            var allOptions = _pollingOptionService.Repository.Entities.Where(x => x.QuestionId == questionid && x.IsDeleted != true)
                .Select(x => new { Id = x.Id, OptionName = x.OptionName }).ToList();

            var options = allOptions.AsParallel().Where(x => allAnswers.AsParallel().All(y => x.Id != y.Key)).Select(x => new SereisData { Sereisname = x.OptionName, Sereisvalue = 0, Id = x.Id }).ToList();

            sereisLst.AddRange(options);

            allAnswers.ForEach(y =>
            {
                int answer = y.Key;
                int optionCount = y.count;
                int perent = (optionCount * 100 / allAnswers.Sum(x => x.count));
                var obj = new SereisData()
                {
                    Sereisname = allOptions.Find(x => x.Id == answer).OptionName,
                    Sereisvalue = perent,
                    Id = answer
                };
                sereisLst.Add(obj);
            });

            pollingScreenView.SereisData = sereisLst.OrderBy(x => x.Id).ToList();

            return pollingScreenView;
        }

        /// <summary>
        /// 获取某个polling下的所有result
        /// </summary>
        /// <param name="pollingID"></param>
        /// <returns></returns>
        public IList<PollingResultEntity> GetPollingResultByPollingId(int pollingID, string lillyId = null)
        {
            if (!string.IsNullOrEmpty(lillyId))
            {
                if (_pollingResultView == null)
                {
                    _pollingResultView = _pollingResultService.Repository.Entities.Where(a => a.PollingId == pollingID && a.UserId == lillyId && a.IsDeleted != true).ToList();
                }

                return _pollingResultView;

            }
            else
            {
                return _pollingResultService.Repository.Entities.Where(a => a.PollingId == pollingID && a.IsDeleted != true).ToList();
            }
        }
        /// <summary>
        /// 获取某人某个polling下的所有result
        /// </summary>
        /// <param name="pollingID"></param>
        /// <returns></returns>
        public IList<PollingResultTempView> GetPollingResultTempByLillyId(int pollingID, string lillyId)
        {
            if (_pollingResultTempView == null)
            {
                _pollingResultTempView = _pollingResultTempService.Repository.Entities.Where(a => a.PollingId == pollingID && a.UserId == lillyId && a.IsDeleted != true)
                    .ToList().Select(pollTemp => (PollingResultTempView)new PollingResultTempView().ConvertAPIModel(pollTemp)).ToList();
            }

            return _pollingResultTempView;
        }
        /// <summary>
        /// 某个人得分情况
        /// </summary>
        /// <param name="pollingId"></param>
        /// <param name="polling"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        //private int GetScore(int pollingId,PollingView polling,IList<PollingResultEntity> results)
        //{
        //    //string answered;

        //    //string answer1;
        //    //获取polling、pollingresult
        //    //var polling = GetPollingView(pollingId);
        //    //var pollingResultByUser = _pollingResultService.GetList(pollingId, lillyid);

        //    var result = PollingResult(pollingId, results, polling);

        //    //stage data
        //    //var results = result.Select(x =>
        //    //   {
        //    //       var question = polling.PollingQuestions.FirstOrDefault(a => a.Id == x.QuestionId);
        //    //       return new PollingAnswerEntity
        //    //       {
        //    //           Dept1 = x.UserDeptLv1,
        //    //           Dept2 = x.UserDeptLv2,
        //    //           Dept3 = x.UserDeptLv3,
        //    //           LillyId = lillyid,
        //    //           Name = x.UserName,
        //    //           QuestionId = x.QuestionId,
        //    //           QuestionTitle = x.QuestionTitle,
        //    //           RightAnswer = x.RightAnswers,
        //    //           SelectAnswer = x.CustomAnswer,
        //    //           Status = x.IsRight,
        //    //           Score = question == null ? 0 : question.Score.GetValueOrDefault()
        //    //       };
        //    //   });

        //    // _pollingAnswerService.Repository.Insert(results);

        //    var rights = result.AsParallel().Where(x => x.IsRight).ToList();

        //    var score = polling.PollingQuestions.AsParallel().Where(x => rights.Any(y => x.Id == y.QuestionId)).Sum(x => x.Score).GetValueOrDefault();

        //    //foreach (var questionresult in result)
        //    //{
        //    //    answered = questionresult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

        //    //    if (polling != null)
        //    //    {
        //    //        var questionstr = polling.PollingQuestions.FirstOrDefault(a => a.Id == questionresult.QuestionId);

        //    //        if (questionstr != null)
        //    //        {
        //    //            answer1 = ConvertAbcToYn(questionstr.RightAnswers, 10);
        //    //            if (answer1 == answered && questionresult.UserId.ToUpper() == lillyid.ToUpper())
        //    //            {
        //    //                score += questionstr.Score.GetValueOrDefault();
        //    //            }
        //    //            else
        //    //            {
        //    //                score += 0;
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return score;

        //}


        //答题总人数

        private int GetPollingTotal(int pollingId)
        {
            return _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingId && x.IsDeleted != true)
                        .Select(x => x.UserId.ToUpper())
                        .Distinct()
                        .Count();
        }


        /// <summary>
        /// 某个polling员工答题后的得分列表
        /// </summary>
        /// <param name="pollingId"></param>
        /// <returns>员工列表，每个员工的得分，以及员工的部门(不再是员工啦)</returns>
        public List<PollingResultScoreView> PollingResultScore(int pollingId, PageCondition pageCondition)
        {
            //var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId);
            var polling = GetPollingView(pollingId);
            var pollingResultScores = new List<PollingResultScoreView>();

            // TODO: 用到了PollingResult
            // var pollingAnswers = _pollingResultService.GetList(pollingId);

            var pollingAnswers = pageCondition != null ? _pollingResultService.GetGroupPagingByUserId(x => x.PollingId == pollingId && x.IsDeleted == false, pageCondition) : _pollingResultService.GetList(pollingId);

            var pollingResult = PollingResult(pollingId, pollingAnswers, polling);

            pollingResult.GroupBy(x => x.UserId).ToList().ForEach(y =>
            {
                // var userid = y.Key.ToUpper();

                var answersForYUser = y.ToList();


                var pollingResultScore = new PollingResultScoreView();
                decimal score = 0;
                foreach (var questionresult in answersForYUser)
                {
                    var answered = questionresult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

                    if (polling != null)
                    {
                        var questionstr = polling.PollingQuestions.FirstOrDefault(a => a.Id == questionresult.QuestionId);

                        if (questionstr != null)
                        {
                            //NY转换成ABC
                            var answer1 = ConvertAbcToYn(questionstr.RightAnswers, 10);

                            if (answer1 == answered && questionresult.UserId.ToUpper() == y.Key.ToUpper())
                            {
                                score += questionstr.Score.GetValueOrDefault();
                            }
                            else
                            {
                                score += 0;
                            }
                        }
                    }
                }
                //List<EmployeeInfoWithDept> empDetails = WeChatCommonService.lstUserWithDeptTag;
                //var emp = empDetails.SingleOrDefault(a => a.userid.ToUpper().Equals(y.Key.ToUpper()));
                pollingResultScore.UserId = y.Key;
                pollingResultScore.UserName = y.Key;
                //pollingResultScore.CustomScore = score;
                //if (emp != null)
                //{
                //    pollingResultScore.UserName = emp.name;
                //    pollingResultScore.UserDeptLv1 = emp.deptLvs[2];
                //    pollingResultScore.UserDeptLv2 = emp.deptLvs[3];
                //    pollingResultScore.UserDeptLv3 = emp.deptLvs[4];
                //}
                pollingResultScores.Add(pollingResultScore);
            });

            return pollingResultScores;
        }

        /// <summary>
        /// 将PollingResultList转换成每个question一条答案的方式，比如多选的多条Result转成1条（ABC）
        /// </summary>
        /// <param name="pollingID"></param>
        /// <param name="_pollingEntity"></param>
        /// <returns></returns>
        public List<PollingResultCustomView> PollingResult(int pollingId, IList<PollingResultEntity> pollingResultEntity, PollingView pollingView)
        {
            var pollingCustomView = new PollingCustomView { PollingId = pollingId };
            //var pollingView = GetPollingView(pollingId);

            foreach (var question in pollingView.PollingQuestions)
            {
                var pollingOptions = question.PollingOptionEntities.Where(a => (a.IsDeleted != true)).ToList();
                int i = 0;
                foreach (var option in pollingOptions)
                {
                    option.OptionIndex = i.ToString();//toABCD(i);
                    i++;
                }

            }

            // 开始处理答案
            var pollingAnswers = pollingResultEntity;

            // 同一个人对同一个question的答案（多选），需要汇总到一个里。

            //var empDetails = WeChatCommonService.lstUserWithDeptTag;

            var grouped = pollingAnswers.GroupBy(a => new { a.UserId, a.QuestionId });
            foreach (var g in grouped)
            {
                var question = pollingView.PollingQuestions.FirstOrDefault(a => a.Id == g.Key.QuestionId);
                if (question == null || question.Type == 99)
                {
                    continue;
                }

                //var emp = empDetails.SingleOrDefault(a => a.userid.Equals(g.Key.UserId, StringComparison.InvariantCultureIgnoreCase));
                var newAnswerResult = new PollingResultCustomView()
                {
                    QuestionId = question.Id,
                    QuestionTitle = question.Title,
                    UserId = g.Key.UserId,

                    //UserName = (emp != null ? emp.name : ""),
                    //UserDeptLv1 = (emp != null ? emp.deptLvs[2] : ""),
                    //UserDeptLv2 = (emp != null ? emp.deptLvs[3] : ""),
                    //UserDeptLv3 = (emp != null ? emp.deptLvs[4] : ""),
                };
                pollingCustomView.Results.Add(newAnswerResult);
                foreach (var v in g.ToList())
                {
                    var option = question.PollingOptionEntities.FirstOrDefault(a => a.Id == v.Answer && (a.IsDeleted != true));
                    if (option == null)
                    {
                        continue;
                    }
                    int optionIndex = int.Parse(option.OptionIndex);
                    //var answer = newAnswerResult.Answers[optionIndex];

                    newAnswerResult.AnswerTime = v.CreatedDate.GetValueOrDefault();

                    newAnswerResult.Answers[optionIndex].AnswerId = v.Answer;
                    newAnswerResult.Answers[optionIndex].AnswerText = v.AnswerText;
                    var answered = newAnswerResult.Answers.Aggregate("", (current, answer) => current + (answer.AnswerId == 0 ? "N" : "Y"));

                    newAnswerResult.CustomAnswer = ConvertYNToABCStatic(answered, 10);
                    var questionstr = pollingView.PollingQuestions.FirstOrDefault(a => a.Id == newAnswerResult.QuestionId);
                    if (questionstr != null) newAnswerResult.RightAnswers = questionstr.RightAnswers;
                    if (newAnswerResult.CustomAnswer == newAnswerResult.RightAnswers)
                    {
                        newAnswerResult.CustomStatus = "正确"; // : "错误";
                        newAnswerResult.IsRight = true;
                    }
                    else
                    {
                        newAnswerResult.CustomStatus = "错误";
                        newAnswerResult.IsRight = false;
                    }
                    //newAnswerResult.CustomStatus = newAnswerResult.CustomAnswer == newAnswerResult.RightAnswers ? "正确" : "错误";
                }
            }

            return pollingCustomView.Results;
        }


        /// <summary>
        /// 某个polling所有question正确人数和参与人数
        /// </summary>
        /// <param name="polling"></param>
        /// <returns>question列表，每个question一行</returns>
        private List<PollingResultCustomView> PollingResultPerson(PollingView polling)
        {
            //var polling = Repository.Entities.FirstOrDefault(a => a.Id == pollingId);
            var pollingResultPersons = new List<PollingResultCustomView>();

            //var pollingAnswers = _pollingResultService.GetList(polling.Id);
            var pollingAnswers = _pollingResultService.GetBatchResultsByPollingId(polling.Id, 0, null, null);
            var pollingResult = PollingResult(polling.Id, pollingAnswers, polling);

            // 循环得到每个question的答题情况
            foreach (var question in polling.PollingQuestions)
            {
                var pollingResultPerson = new PollingResultCustomView();
                int person = 0;
                int answerPersons = 0;
                foreach (var questionresult in pollingResult)
                {
                    if (questionresult.CustomStatus.Equals("正确") && questionresult.QuestionId == question.Id)
                    {
                        person += 1;
                        answerPersons += 1;
                    }
                    if (questionresult.CustomStatus.Equals("错误") && questionresult.QuestionId == question.Id)
                    {
                        answerPersons += 1;
                    }
                }
                pollingResultPerson.QuestionId = question.Id;
                pollingResultPerson.RightPersons = person;
                pollingResultPerson.answerPersons = answerPersons;
                pollingResultPersons.Add(pollingResultPerson);
            }

            return pollingResultPersons;
        }


        /// <summary>
        /// 获取百分比
        /// </summary>
        /// <param name="pov"></param>
        /// <param name="pollingid"></param>
        /// <param name="lillyid"></param>
        /// <returns></returns>
        private IList<PollingOptionView> GetOptionPercent(IList<PollingOptionView> pov, int pollingid, string lillyid)
        {
            var option = _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.IsDeleted != true).GroupBy(x => new { QuestionId = x.QuestionId, Answer = x.Answer }).
                Select(a => new { QuestionId = a.Key.QuestionId, Answer = a.Key.Answer, Count = a.Count() }).ToList();

            var question = option.GroupBy(a => a.QuestionId).Select(a => a.Key).ToList();
            question.ForEach(x =>
            {
                int questionTotal = 0;
                var os = option.Where(a => a.QuestionId == x);

                foreach (var o in os)
                {
                    questionTotal += o.Count;
                    int optionCount = o.Count;
                    int perent = (optionCount * 100 / questionTotal);
                    pov.ForEach(z =>
                    {
                        if (z.Id != o.Answer) return;
                        z.VoteNum = optionCount;
                        z.Percent = perent;
                    });
                }

            });

            return pov;
        }

        private IList<PollingOptionView> GetOptionPercent(IList<PollingOptionView> pov, IList<ResultGroup> groups)
        {
            var question = groups.GroupBy(a => a.QuestionId).Select(a => a.Key).ToList();
            question.ForEach(x =>
            {
                var os = groups.Where(a => a.QuestionId == x).ToList();
                var questionTotal = os.Sum(o => o.CountNumber);

                foreach (var o in os)
                {
                    int optionCount = o.CountNumber;
                    int perent = (optionCount * 100 / questionTotal);
                    pov.ForEach(z =>
                    {
                        if (z.Id != o.AnswerId) return;
                        z.VoteNum = optionCount;
                        z.Percent = perent;
                    });
                }

            });

            return pov;
        }

        private IList<ResultGroup> GetResultGroups(int pollingid)
        {
            return _pollingResultService.Repository.Entities.Where(x => x.PollingId == pollingid && x.IsDeleted != true).GroupBy(x => new { x.QuestionId, x.Answer }).
    Select(a => new { a.Key.QuestionId, a.Key.Answer, Count = a.Count() }).ToList().Select(x => new ResultGroup { QuestionId = x.QuestionId, AnswerId = x.Answer, CountNumber = x.Count }).ToList();
        }

        public string ConvertAbcToYn(string abc, int len = 10)
        {
            return ConvertAbcToYnStatic(abc, len);
        }

        public static string ConvertAbcToYnStatic(string abc, int len = 10)
        {
            var ync = new char[10] { 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N', 'N' };
            if (string.IsNullOrEmpty(abc))
            {
                return new string(ync);
            }

            if (abc.Length > 10)
            {
                abc = abc.Substring(0, 10);
            }
            foreach (var c in abc.ToUpper())
            {
                if (!Regex.IsMatch(c.ToString(), @"^[a-jA-J]+$"))
                {
                    continue;
                }
                int i = c - 'A';
                if (i >= 10)
                {
                    continue;
                }
                ync[i] = 'Y';
            }

            return new string(ync);
        }

        public static string ConvertYNToABCStatic(string YN, int len = 10)
        {
            var ync = new char[10] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
            var result = "";
            if (string.IsNullOrEmpty(YN))
            {
                return result;
            }
            if (YN.Length > 10)
            {
                YN = YN.Substring(0, 10);
            }

            for (int i = 0; i < YN.Length - 1; i++)
            {
                if (YN[i] == 'Y')
                {
                    result += ync[i].ToString();
                }
            }

            return result;
        }

        public decimal InsertResult(PollingResultView view)
        {
            //generate results
            var results = view.AnswerResults.AsParallel().Select(x => new PollingResultEntity
            {
                PollingId = view.PollingId,
                QuestionId = x.QuestionId,
                QuestionName = x.QuestionName,
                Answer = x.Answer,
                AnswerText = x.AnswerText,
                UserId = view.UserId
            });

            _pollingResultService.Repository.Insert(results.ToList().AsEnumerable());

            var polling = GetPollingView(view.PollingId);

            var result = PollingResult(view.PollingId, results.ToList(), polling);

            //stage data
            var resultsAnswers = result.Select(x =>
               {
                   var question = polling.PollingQuestions.FirstOrDefault(a => a.Id == x.QuestionId);
                   return new PollingAnswerEntity
                   {
                       Dept1 = x.UserDeptLv1,
                       Dept2 = x.UserDeptLv2,
                       Dept3 = x.UserDeptLv3,
                       LillyId = view.UserId,
                       Name = x.UserName,
                       QuestionId = x.QuestionId,
                       QuestionTitle = x.QuestionTitle,
                       RightAnswer = x.RightAnswers,
                       SelectAnswer = x.CustomAnswer,
                       Status = x.IsRight,
                       Score = x.IsRight ? (question == null ? 0 : question.Score.GetValueOrDefault()) : 0,
                       PollingId = view.PollingId
                   };
               }).ToList();

            _pollingAnswerService.Repository.Insert(resultsAnswers.AsEnumerable());

            var rights = result.AsParallel().Where(x => x.IsRight).ToList();

            var score = polling.PollingQuestions.AsParallel().Where(x => rights.Any(y => x.Id == y.QuestionId)).Sum(x => x.Score).GetValueOrDefault();
            return score;
        }

        public IList<SortViewModel> GetSorting(IList<int> pollingIds)
        {
            //var valuearray = pollingIds.Split(',').ToList().ConvertAll(int.Parse).OrderBy(x => x).ToList();

            var currentDate = DateTime.Now;
            var invalidPollings = Repository.Entities.Where(x => pollingIds.Contains(x.Id) && (x.IsDeleted == true || (x.StartDateTime > currentDate))).Select(x => x.Id).ToList();

            var validIds = pollingIds.AsParallel().Where(x => invalidPollings.All(y => y != x)).ToList();

            if (!validIds.Any())
            {
                return new List<SortViewModel>();
            }

            var parms = validIds.Select((s, i) => "@p" + i.ToString(CultureInfo.InvariantCulture)).ToArray();
            var sql = string.Format(SqlQueryTemplate, string.Join(",", parms));

            var values = validIds.Select((x, i) => new SqlParameter(parms[i], x)).ToArray();

            var list = ((DbContext)Repository.UnitOfWork).Database.SqlQuery<PollingSortingEntity>(sql, values).ToList();

            //convert row to column
            var sorts = list.GroupBy(x => x.OptionName).Select(x =>
            {
                var tempSorts = x.Select(y => (int?)y.Sort).ToList();

                if (invalidPollings.Any())
                {
                    tempSorts.AddRange(invalidPollings.Select(y => (int?)null));
                }
                return new SortViewModel
                {
                    Name = x.Key,
                    Sorts = tempSorts,
                    SortSum = x.Sum(y => y.Sort)
                };
            }).ToList();

            return sorts;
        }

        public int ClonePolling(int pollingId)
        {
            var pollingView = GetPollingView(pollingId);

            //var polling = Repository.Entities.Where(a => a.IsDeleted != true && a.Id == pollingId).Include(x => x.PollingQuestions.Select(y => y.PollingOptionEntities)).FirstOrDefault();

            if (pollingView == null)
            {
                return 0;
            }

            Parallel.ForEach(pollingView.PollingQuestions, x =>
             {
                 x.PollingOptionEntities.AsParallel().ForEach(y =>
                 {
                     y.Id = 0;
                     y.QuestionId = 0;
                 });

                 x.PollingId = 0;
                 x.Id = 0;
             }
                 );

            pollingView.Id = 0;
            pollingView.GuiId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            var t = pollingView.MapTo<PollingEntity>();
            Repository.Insert(t);

            return t.Id;
        }
    }

    public class PollingComapre : IEqualityComparer<PollingResultEntity>
    {

        public bool Equals(PollingResultEntity x, PollingResultEntity y)
        {
            return x.UserId == y.UserId;
        }

        public int GetHashCode(PollingResultEntity obj)
        {
            return obj.UserId.GetHashCode();
        }
    }

    public class ResultGroup
    {
        public int QuestionId { get; set; }

        public int AnswerId { get; set; }

        public int CountNumber { get; set; }
    }


}
