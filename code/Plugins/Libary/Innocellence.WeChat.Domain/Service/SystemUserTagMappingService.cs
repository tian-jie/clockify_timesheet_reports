using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innocellence.WeChat.Domain.Service
{
    public class SystemUserTagMappingService : BaseService<SystemUserTagMapping>, ISystemUserTagMappingService
    {

        public int Create(SystemUserTagMapping tag)
        {
            tag.IsDeleted = false;
            var result = Repository.Insert(tag);
            return result;
        }

        public void Create(List<SystemUserTagMapping> tags)
        {
            tags.ForEach(a => a.IsDeleted = false);
            Repository.Insert((IEnumerable<SystemUserTagMapping>)tags);
        }

        public int DeleteByTag(int tagId)
        {
            var deleteItem = this.Repository.Entities.Where(a => a.TagId == tagId).ToList();
            var sql = "update SystemUserTagMapping set [IsDeleted] = 1 where TagId= {0}";
            this.Repository.SqlExcute(sql, tagId);
            return deleteItem == null ? 0 : deleteItem.Count;
        }
        public int Update(SystemUserTagMapping tag)
        {
            var updateItem = this.Repository.Entities.Where(a => a.Id == tag.Id).FirstOrDefault();
            if (updateItem != null)
            {
                updateItem.IsDeleted = tag.IsDeleted;
                updateItem.TagId = tag.TagId;
                updateItem.UserOpenid = tag.UserOpenid;
                this.Repository.Update(updateItem);
            }
            return updateItem.Id;
        }

        public List<SystemUserTagMapping> GetMappingByTagIds(List<int> tagIds)
        {
            if (null != tagIds && tagIds.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("select * from SystemUserTagMapping where TagId in ({0})", string.Join(",", tagIds.ToArray()))
                  .Append(" and UserOpenid in ")
                  .Append(" (")
                  .Append(" select distinct temp.UserOpenid")
                  .Append(" from ( ")
                  .Append(" select a.UserOpenid, count(a.UserOpenid) as c ")
                  .Append(" from SystemUserTagMapping as a")
                  .AppendFormat(" where a.TagId in ({0}) and a.IsDeleted = 0 ", string.Join(",", tagIds.ToArray()))
                  .Append(" group by UserOpenid")
                  .Append(" ) as temp")
                  .AppendFormat(" where temp.c = {0}", tagIds.Count)
                  .Append(" )");
                var userOpenIdList = Repository.SqlQuery(sb.ToString());
                if (userOpenIdList != null)
                {
                    return userOpenIdList.ToList();
                }
            }
            return new List<SystemUserTagMapping> { };
        }


        public List<SystemUserTagMapping> GetUserTagByOpenId(string openId)
        {
            var list = Repository.Entities.Where(a => a.UserOpenid == openId && a.IsDeleted == false);
            if (list != null)
            {
                return list.ToList();
            }
            return new List<SystemUserTagMapping> { };
        }

        public int Delete(int id)
        {
            var updateItem = this.Repository.Entities.Where(a => a.Id == id).FirstOrDefault();
            if (updateItem != null)
            {
                this.Repository.Delete(updateItem);
                return id;
            }
            return -1;
        }

        public void DeleteByOpenId(string openId)
        {
            Repository.Delete(a => a.UserOpenid == openId);
        }
    }
}
