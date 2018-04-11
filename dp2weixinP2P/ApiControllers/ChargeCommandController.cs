﻿using dp2weixin.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace dp2weixinWeb.ApiControllers
{
    public class ChargeCommandController : ApiController
    {
        [HttpGet]
        public ChargeCommandResult GetCommands(string libId)
        {
            ChargeCommandResult result = new ChargeCommandResult();

            SessionInfo sessionInfo = (SessionInfo)HttpContext.Current.Session[WeiXinConst.C_Session_sessioninfo];
            ChargeCommandContainer cmdContainer = sessionInfo.cmdContainer;
            result.cmds = cmdContainer;

            return result;
        }

        [HttpPost]
        public ChargeCommand CreateCmd(string weixinId, 
            string libId,
            string libraryCode,
            int needTransfrom,
            ChargeCommand cmd)
        {

            SessionInfo sessionInfo = (SessionInfo)HttpContext.Current.Session[WeiXinConst.C_Session_sessioninfo];
            ChargeCommandContainer cmdContainer = sessionInfo.cmdContainer;
            if (sessionInfo.Active == null)
            {
                dp2WeiXinService.Instance.WriteLog1("提交流通API时，发现session失效了。");
            }
            

            try
            {
                // 执行命令
                return cmdContainer.AddCmd(//sessionInfo.Active,
                    weixinId,
                    libId,
                    libraryCode,
                    needTransfrom,
                    cmd);
            }
            catch (Exception ex)
            {
                cmd.errorInfo = ex.Message;
                cmd.state = -1;
                dp2WeiXinService.Instance.WriteLog1("借还时同错："+ex.Message);


                return cmd;
            }
        }

        public ApiResult VerifyBarcode(string libId,
            string libraryCode,
            string userId,
            string barcode,
            int needTransform)
        {
            ApiResult result = new ApiResult();

            string error = "";
            string resultBarcode="";
            int nRet = dp2WeiXinService.Instance.VerifyBarcode(libId,
                libraryCode,
                userId,
                barcode,
                needTransform,
                out resultBarcode,
                out error);
            if (nRet == -1)
            {
                dp2WeiXinService.Instance.WriteLog1("校验条码error:" + error);
            }
            result.errorCode = nRet;
            result.errorInfo = error;
            result.info = resultBarcode;

            return result;
        }
    }
}
