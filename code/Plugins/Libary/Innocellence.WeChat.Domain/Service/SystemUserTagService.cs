using Innocellence.WeChat.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core;
using Infrastructure.Utility.Data;
using Innocellence.WeChat.Domain.Entity;
using System.Linq.Expressions;
using Infrastructure.Core.Data;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Infrastructure;
using Autofac;

namespace Innocellence.WeChat.Domain.Service
{
    public class SystemUserTagService : BaseService<SystemUserTag>, ISystemUserTagService
    {

        public List<SystemUserTag> GetFirstLevelTag()
        {
            var list = Repository.Entities;
            if (list!=null&& list.Count()>0)
            {
                list = from a in list
                       where a.ParentId==null && !a.IsDeleted
                       select a ;
            }
            return list.ToList();
        }

        public List<SystemUserTag> GetTagByParentId(int parentId)
        {
            var list = Repository.Entities;
            if (list != null && list.Count() > 0)
            {
                list = from a in list
                       where a.ParentId == parentId
                       select a;
                if (list!=null)
                {
                    return list.ToList();
                }
            }
            return new List<SystemUserTag> { };
        }

        public int Create(SystemUserTag tag)
        {

          ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SystemUserTagService)));
          
           tag.IsDeleted = false;
            var result= Repository.Insert(tag);
            if (result==1)
            {
                cacheManager.Remove("UserTagList");
                return tag.Id;
            }
            return result;
        }
        public int Delete(int tagId)
        {
            var deleteItem = this.Repository.Entities.Where(a => a.Id == tagId).FirstOrDefault();
            if (deleteItem != null)
            {
                deleteItem.IsDeleted = true;
                this.Repository.Update(deleteItem);
                ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SystemUserTagService)));
                cacheManager.Remove("UserTagList");
            }
            return deleteItem.Id;
        }
        public int Update(SystemUserTag tag)
        {
            var updateItem = this.Repository.Entities.Where(a => a.Id == tag.Id).FirstOrDefault();
            if (updateItem != null)
            {
                 updateItem.IsDeleted = tag.IsDeleted;
                updateItem.Name = tag.Name;
                updateItem.ParentId = tag.ParentId;
                this.Repository.Update(updateItem);
                ICacheManager cacheManager = EngineContext.Current.Resolve<ICacheManager>(new TypedParameter(typeof(Type), typeof(SystemUserTagService)));
                cacheManager.Remove("UserTagList");
            }
            return updateItem.Id;
        }

        public List<SystemUserTag> GetAllTags()
        {
            return this.Repository.Entities.Where(a=>!a.IsDeleted).ToList();
        }
        public SystemUserTag GetTagById(int tagId)
        {
            return this.Repository.Entities.Where(a => a.Id == tagId).FirstOrDefault();
        }
        public int EditTagName(string tagName, int tagId)
        {
            var tag = GetTagById(tagId);
            if (tag!=null)
            {
                tag.Name = tagName;
                this.Repository.Update(tag);
                return tag.Id;
            }
            return -1;
        }
    }
}
