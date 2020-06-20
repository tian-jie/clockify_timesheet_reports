using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Contracts;
using Innocellence.WeChat.Domain.Entity;
using System.Linq;
using Infrastructure.Core;
using System;
using System.Linq.Expressions;

namespace Innocellence.WeChat.Domain.Services
{
    public partial class FileManageService : BaseService<FileManage>, IFileManageService
    {
        public FileManageService()
            : base("CAAdmin")
        {

        }

        public T GetById<T>(int id) where T : IViewModel, new()
        {
            Expression<Func<FileManage, bool>> predicate = a => a.Id == id && a.IsDeleted == false;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

        /// <summary>
        /// 更新文件的MediaID
        /// 当文件被上传到微信服务器时，此字段需要更新；或者需要从微信服务器移除时，更新为空
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public int UpdateMediaId(int fileId, string mediaId)
        {
            var result = 0;
            var entity = Repository.Entities.FirstOrDefault(x => x.Id == fileId);
            if (entity != null)
            {
                entity.MediaID = mediaId;
                result = Repository.Update(entity);
            }

            return result;
        }


        public T GetByMediaId<T>(string mediaId) where T : IViewModel, new()
        {
            Expression<Func<FileManage, bool>> predicate = a => a.MediaID.Equals(mediaId, StringComparison.CurrentCultureIgnoreCase) && a.IsDeleted == false;

            var t = Repository.Entities.Where(predicate).ToList().Select(n => (T)(new T().ConvertAPIModel(n))).FirstOrDefault();

            return t;
        }

    }
}