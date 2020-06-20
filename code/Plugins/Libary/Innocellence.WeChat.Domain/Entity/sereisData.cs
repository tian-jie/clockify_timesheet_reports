using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innocellence.WeChat.Domain.Entity
{
    /// <summary>

    /// 定义一个SereisData类 设置其每一组sereisData的一些基本属性

    /// </summary>

    public class SereisData
    {

        public int Id { get; set; }
        /// <summary>
        /// SereisData序列组value
        /// </summary>
        public int Sereisvalue { get; set; }


        /// <summary>
        /// SereisData序列组名称
        /// </summary>
        public string Sereisname { get; set; }

    }
}
