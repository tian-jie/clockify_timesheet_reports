using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core;

namespace Innocellence.WeChat.Domain.Entity
{
	//[Table("Category")]
    public  class UploadMessageSuccess
	{

        public UploadMessageSuccess(object objResult, string strID)
        {
            result = objResult;
            id = strID;
        }

        public string jsonrpc { get { return "2.0"; } }

        public object result { get; set; }
        public String id { get; set; }
 
	}

    public class UploadMessageSuccessMsg
    {
        public UploadMessageSuccessMsg(string strName, string strSrc)
        {
            Name = strName;
            Src = strSrc;
        }
        public String Name { get; set; }
        public String Src { get; set; }

        public String FileID { get; set; }
    }

    public  class UploadMessageError
    {
        public UploadMessageError(string strErrCode,string strErrMsg,string strID)
        {
            error = new UploadMessageErrorMsg(strErrCode, strErrMsg);
            id = strID;
        }
        public string jsonrpc { get { return "2.0"; } }

        public UploadMessageErrorMsg error { get; set; }
        public String id { get; set; }

    }
    public  class UploadMessageErrorMsg
    {
         public UploadMessageErrorMsg(string strErrCode, string strErrMsg)
         {
             code = strErrCode;
             message = strErrMsg;
         }
        public String code { get; set; }
        public String message { get; set; }
    }



}
