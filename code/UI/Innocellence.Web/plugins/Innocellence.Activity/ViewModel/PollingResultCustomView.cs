using Infrastructure.Core;
using Innocellence.Activity.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Innocellence.Activity.ModelsView
{
    public class PollingCustomView
    {
        public PollingCustomView()
        {
            Results = new List<PollingResultCustomView>();
        }
        public int PollingId { get; set; }
        public string PollingName { get; set; }
        public List<PollingResultCustomView> Results { get; set; }
    }

    public class PollingResultCustomView : IViewModel
    {
        public PollingResultCustomView()
        {
            Answers = new PollingResultOptionCustomView[10];
            for (int i = 0; i < 10; i++)
            {
                Answers[i] = new PollingResultOptionCustomView();
            }
        }
        public int Id { get; set; }

        public String UserId { get; set; }

        //[DescriptionAttribute("用户名")]
        public String UserName { get; set; }

        [DescriptionAttribute("一级部门")]
        public String UserDeptLv1 { get; set; }

        [DescriptionAttribute("二级部门")]
        public String UserDeptLv2 { get; set; }

        [DescriptionAttribute("三级部门")]
        public String UserDeptLv3 { get; set; }
        public DateTime AnswerTime { get; set; }
        public int QuestionId { get; set; }

        [DescriptionAttribute("题目")]
        public string QuestionTitle { get; set; }

        [DescriptionAttribute("答卷时间")]
        public DateTime? CreatedDate;

        [DescriptionAttribute("正确答案")]
        public string RightAnswers { get; set; }

        [DescriptionAttribute("用户选择")]
        public string CustomAnswer { get; set; }

        [DescriptionAttribute("是否正确")]
        public string CustomStatus { get; set; }
       

        public bool IsRight { get; set; }
        public int RightPersons { get; set; }
        public int answerPersons { get; set; }
        [DescriptionAttribute("得分")]
        public decimal? CustomScore { get; set; }
        public PollingResultOptionCustomView[] Answers { get; set; }
        public IViewModel ConvertAPIModel(object obj)
        {
            if (obj == null) { return this; }
            var entity = (PollingAnswerEntity)obj;
            UserId = entity.LillyId;
            UserName = entity.Name;
            UserDeptLv1 = entity.Dept1;
            UserDeptLv2 = entity.Dept2;
            UserDeptLv3 = entity.Dept3;
            QuestionId = entity.QuestionId;
            QuestionTitle = entity.QuestionTitle;
            RightAnswers = entity.RightAnswer;
            CustomAnswer = entity.SelectAnswer;
            IsRight = entity.Status;
            CustomScore = entity.Score;
            CustomStatus = entity.Status == false ? "错误" : "正确";
            CreatedDate = entity.CreatedDate;
            return this;
        }
    }

    public class PollingResultOptionCustomView
    {
        public int OptionId { get; set; }
        public string OptionTitle { get; set; }
        public string ABCDId { get; set; }
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
    }
    public class PollingResultScoreView
    {
        public String UserId { get; set; }

        [DescriptionAttribute("用户名")]
        public String UserName { get; set; }

        [DescriptionAttribute("一级部门")]
        public String UserDeptLv1 { get; set; }

        [DescriptionAttribute("二级部门")]
        public String UserDeptLv2 { get; set; }

        [DescriptionAttribute("三级部门")]
        public String UserDeptLv3 { get; set; }

       
    }
}