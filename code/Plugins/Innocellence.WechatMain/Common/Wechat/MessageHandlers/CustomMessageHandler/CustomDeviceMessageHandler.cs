/*----------------------------------------------------------------
    Copyright (C) 2015 Innocellence
    
    文件名：CustomMessageHandler.cs
    文件功能描述：自定义MessageHandler
    
    
    创建标识：Innocellence - 20150312
----------------------------------------------------------------*/

using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Innocellence.Weixin.MP.Agent;
using Innocellence.Weixin.Context;
using Innocellence.Weixin.MP.Entities;
using Innocellence.Weixin.MP.Entities.Request;
using Innocellence.Weixin.MP.MessageHandlers;
using Innocellence.Weixin.MP.Helpers;
using Innocellence.Weixin.CommonService.Utilities;
using Innocellence.Weixin.MP;
using Innocellence.Weixin.CommonService.CustomMessageHandler;
using Infrastructure.Core.Logging;
//using Innocellence.LED.Common;
using Innocellence.Weixin.MP.AdvancedAPIs;
using Innocellence.Weixin.Entities;
using System.Collections.Generic;

namespace Innocellence.Weixin.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {


        public override List<IResponseMessageBase> OnEvent_DeviceBind(RequestMessageEvent_Device_Bind requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessagedevice_text>(requestMessage);

            responseMessage.DeviceID = requestMessage.DeviceID;
            responseMessage.DeviceType = requestMessage.DeviceType;
            responseMessage.OpenID = requestMessage.OpenID;
            responseMessage.SessionID = requestMessage.SessionID;
            responseMessage.Content = "";
            responseMessage.MsgId = requestMessage.MsgId;
            return DefaultResponseMessage(requestMessage);
        }




        public override List<IResponseMessageBase> OnDeviceTextRequest(RequestMessageDeviceMsg requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessagedevice_text>();
            responseMessage.DeviceID = requestMessage.DeviceID;
            responseMessage.SessionID = requestMessage.SessionID;
            responseMessage.DeviceType = requestMessage.DeviceType;
            responseMessage.OpenID = requestMessage.OpenID;
            responseMessage.MsgId = (requestMessage.MsgID == 0 ? 112 : requestMessage.MsgID);
            responseMessage.Content = "";
            return new List<IResponseMessageBase>() { responseMessage };
        }



        //public override List<IResponseMessageBase> OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        //{
        //    var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
        //    responseMessage.Content = "您刚才发送了ENTER事件请求。";
        //    return new List<IResponseMessageBase>() { responseMessage };
        //}
    }
}
