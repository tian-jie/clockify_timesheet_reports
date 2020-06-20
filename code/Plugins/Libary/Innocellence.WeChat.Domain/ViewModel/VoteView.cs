using System;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.Contracts.ViewModel
{
    public class VoteView : IViewModel
    {
        public int Id { get; set; }

        public string VoteName { get; set; }

        public string CreatUserId { get; set; }

        public DateTime CreatDateTime { get; set; }

        public string VoteType { get; set; }

        public string VoteStatus { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public string UpdateUserId { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var obj = (VoteEntity)model;

            return new VoteView { Id = obj.Id, VoteName = obj.VoteName };
        }
    }

    public class VoteQuestionView : IViewModel
    {
        public int Id { get; set; }

        public string QuestionName { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var voteQuestion = (VoteQuestionEntity)model;

            return new VoteQuestionView { Id = voteQuestion.Id, QuestionName = voteQuestion.QuestionName };
        }
    }

    public class VoteAnswerView : IViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (VoteAnswerEntity)model;

            return new VoteAnswerView
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }

    public class VoteReslutView : IViewModel
    {
        public int Id { get; set; }

        public int AnswerId { get; set; }

        public IViewModel ConvertAPIModel(object model)
        {
            var entity = (VoteResultEntity)model;

            return new VoteReslutView
            {
                Id = entity.Id,
                AnswerId = entity.AnwserId
            };
        }
    }

}
