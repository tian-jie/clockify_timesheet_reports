// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="Innocellence">
//      Copyright (c) 2014-2015 Innocellence. All rights reserved.
//  </copyright>
//  <last-editor>@Innocellence</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Innocellence.WeChat.Domain.Entity;
using Innocellence.WeChat.Domain.Contracts;


namespace Innocellence.WeChat.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class WeChatAppUserService : BaseService<WeChatAppUser>, IWeChatAppUserService
    {
        public WeChatAppUserService()
            : base("CAAdmin")
        {

        }


    }
}