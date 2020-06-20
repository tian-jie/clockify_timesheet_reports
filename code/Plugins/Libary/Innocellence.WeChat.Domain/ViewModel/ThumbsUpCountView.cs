using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;
using Innocellence.WeChat.Domain.Entity;

namespace Innocellence.WeChat.Domain.ModelsView
{
    //[Table("ArticleInfoView")]

    public partial class ThumbsUpCountView : IViewModel
    {

        public Int32 Id { get; set; }

        /// <summary>
        /// ���ڼ�¼���޵�ģ�飬�ñ�����ʶ
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// �Ըñ��е�������¼���е���
        /// </summary>
        public Int32 RecordID { get; set; }
        
        /// <summary>
        /// ��������
        /// </summary>
        public int ThumbsUpCount { get; set; }

        /// <summary>
        /// ����û�е����
        /// </summary>
        public bool AmIThumbsUp { get; set; }

        /// <summary>
        /// ���case������ת���������ˡ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IViewModel ConvertAPIModel(object obj)
        {
            var entity = (ThumbsUp)obj;
            Id = entity.Id;
            TableName = entity.TableName;
            RecordID = entity.RecordID;

            return this;
        }

    }
}
