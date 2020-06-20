using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using Innocellence.CA.Contracts.Entity;
using Innocellence.CA.Contracts.ViewModel;
using Innocellence.CA.Service;
using NUnit.Framework;

namespace UnitTest.ServiceTest
{
    public class VoteUT : DbSetUp
    {
        [Test]
        public void QueryDatas()
        {
            var vote = new DictionaryService();

            var list = vote.QueryList(x => x.Id > 0);

            Assert.AreEqual(list.Count, 9);
        }

        [Test]
        public void Vote()
        {
            var vote = new VoteService();
            vote.InsertView(new VoteView
            {
                VoteName = "test",
                CreatDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
                CreatUserId = "john",
                UpdateUserId = "john",
                VoteType = "QuestionSurvey",
                VoteStatus = "Waited",
            });

            Assert.AreEqual(vote.QueryList(x => x.Id > 0).Count, 1);
        }

        [Test]
        public void VoteRel()
        {
            var vote = new VoteService();

            var collection = new Collection<VoteQuestionEntity>
            {
                new VoteQuestionEntity {
                    IsRequired = false, 
                    QuestionName = "关系模型",
                    QuestionType = "TextBox"}
            };

            vote.InsertView(new VoteEntity
             {
                 VoteName = "test",
                 CreatDateTime = DateTime.Now,
                 UpdateDateTime = DateTime.Now,
                 CreatUserId = "john",
                 UpdateUserId = "john",
                 VoteType = "QuestionSurvey",
                 VoteStatus = "Waited",
                 VoteQuestions = collection
             });

            var list = vote.QueryList(x => x.Id > 0);

            Assert.IsTrue(list.Count == 2 && list.SelectMany(x => x.VoteQuestions).Count() == 1);

        }

        [Test]
        public void TransRel()
        {
            var vote = new VoteService();

            var collection = new Collection<VoteQuestionEntity>
            {
                new VoteQuestionEntity {
                    IsRequired = false, 
                    QuestionName = "关系模型",
                    QuestionType = "TextBox",
                    Id = 1
                }
            };

            vote.InsertView(new VoteEntity
           {
               VoteName = "test1",
               CreatDateTime = DateTime.Now,
               UpdateDateTime = DateTime.Now,
               CreatUserId = "john",
               UpdateUserId = "john",
               VoteType = "QuestionSurvey",
               VoteStatus = "Waited",
               VoteQuestions = collection,
           });

            var list = vote.QueryList(x => x.Id > 0);

            Assert.IsTrue(list.Count == 2 && list.SelectMany(x => x.VoteQuestions).Count() == 1);
        }

        [Test]
        public void Trans()
        {
            var vote = new VoteService();

            var collection = new Collection<VoteQuestionEntity>
            {
                new VoteQuestionEntity {
                    IsRequired = false, 
                    QuestionName = "关系模型",
                    QuestionType = "TextBox",
                    Id = 1
                }
            };

            vote.InsertView(new VoteEntity
            {
                VoteName = "test1",
                CreatDateTime = DateTime.Now,
                UpdateDateTime = DateTime.Now,
                CreatUserId = "john",
                UpdateUserId = "john",
                VoteType = "QuestionSurvey",
                VoteStatus = "Waited",
                VoteQuestions = collection,
            });



            using (var transactionscope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {

                transactionscope.Complete();
            }
        }
    }
}
