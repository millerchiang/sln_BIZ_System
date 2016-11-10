using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiContrib.ModelBinders;
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class MessageController : ApiController
    {
        private MessageService messageService = new MessageService();
        
        [HttpGet]
        public object GetMessagePrivateList(string user_id, string date)
        {
           if (user_id.IsNullOrEmpty() || date.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "data is null");

            DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.CurrentCulture);
            IList<MsgPrivate> msgPrivates = messageService.SelectMsgPrivateForMobile(user_id, dt).Select(
                msgModel =>
                new MsgPrivate
                {
                    msg_no = msgModel.msg_no,
                    msg_title = msgModel.msg_title,
                    company = msgModel.company,
                    company_en = msgModel.company_en,
                    create_time = msgModel.create_time.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    is_read = msgModel.is_read
                }
            ).ToList();
            
            return Request.CreateResponse(HttpStatusCode.OK, msgPrivates);
        }

        [HttpGet]
        public object GetMessageContent(int msg_no)
        {
            if (msg_no == 0) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "msg_no is null");
            MessageContent messageContent = new MessageContent();
            MsgModel msgModel = messageService.SelectMsgPrivateOne(msg_no);
            messageContent.msgPrivate = new MsgPrivate {
                msg_no = msgModel.msg_no,
                msg_title = msgModel.msg_title,
                msg_content = msgModel.msg_content,
                company = msgModel.company,
                create_time = msgModel.create_time.ToString("yyyy-MM-dd HH:mm")
            };
            messageContent.msgPrivate.msg_member = messageService.transferMsg_member2Msg_company(msgModel.msg_member, prj_BIZ_System.Controllers.MessageCatalog.Private);
            string[] fileNames = messageService.SelectMsgPrivateFileByMsg_no(msg_no).Select(
                msgFileModel =>
                msgFileModel.msg_file_site   
            ).ToArray();
            messageContent.msgPrivate.msg_file = string.Join(",", fileNames);
            messageContent.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no).Select(
                msgPrivateReplyModel => 
                new MsgPrivateReply
                {
                    company = msgPrivateReplyModel.company,
                    reply_content = msgPrivateReplyModel.reply_content,
                    create_time = msgPrivateReplyModel.create_time.ToString("yyyy-MM-dd HH:mm")
                }
            ).ToList();
            
            return Request.CreateResponse(HttpStatusCode.OK, messageContent);
        }

        [HttpGet]
        public object GetMessageContentAndRead(int msg_no, string user_id)
        {
            if (msg_no == 0 || user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "data is null");
            MessageContent messageContent = new MessageContent();
            MsgModel msgModel = messageService.SelectMsgPrivateOneAndRead(msg_no, user_id);
            messageContent.msgPrivate = new MsgPrivate
            {
                msg_no = msgModel.msg_no,
                msg_title = msgModel.msg_title,
                msg_content = msgModel.msg_content,
                company = msgModel.company,
                company_en = msgModel.company_en,
                create_time = msgModel.create_time.ToString("yyyy-MM-dd HH:mm")
            };
            messageContent.msgPrivate.msg_member = messageService.transferMsg_member2Msg_company(msgModel.msg_member, prj_BIZ_System.Controllers.MessageCatalog.Private);
            string[] fileNames = messageService.SelectMsgPrivateFileByMsg_no(msg_no).Select(
                msgFileModel =>
                msgFileModel.msg_file_site
            ).ToArray();
            messageContent.msgPrivate.msg_file = string.Join(",", fileNames);
            messageContent.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no).Select(
                msgPrivateReplyModel =>
                new MsgPrivateReply
                {
                    company = msgPrivateReplyModel.company,
                    company_en = msgPrivateReplyModel.company_en,
                    reply_content = msgPrivateReplyModel.reply_content,
                    create_time = msgPrivateReplyModel.create_time.ToString("yyyy-MM-dd HH:mm")
                }
            ).ToList();
            
            return Request.CreateResponse(HttpStatusCode.OK, messageContent);
        }

        [HttpPost]
        public object MessageReply(MsgReplyModel model)
        {
           return messageService.InsertMsgPrivateReply(model);
        }

        [HttpPost]
        public object AddMessage(MsgModel model)
        {
            model.msg_member = model.msg_member.Replace(",", ", ") + ",";
            model.is_public = "0";
            return (long)messageService.InsertMsgPrivate(model);
        }

        [HttpGet]
        public object GetCompanySort(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user_id is null");

            var companySort = messageService.SelectUserKwForMobile(user_id).Select(
                userInfoModel =>
                new CompanySortModel
                {
                    user_id = userInfoModel.user_id,
                    company = userInfoModel.company,
                    company_en = userInfoModel.company_en,
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, companySort);
        }

        [HttpGet]
        public object GetMsgClusterPublic(string cluster_no, string user_id, string kw)
        {
            if (cluster_no.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster_no is null");
            var publicResult = messageService.SelectMsgClusterPublic(cluster_no, user_id, kw);
            return Request.CreateResponse(HttpStatusCode.OK, publicResult);
        }

        [HttpGet]
        public object GetMsgClusterPrivate(string cluster_no, string user_id, string kw)
        {
            if (cluster_no.IsNullOrEmpty()) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cluster_no is null");
            var publicResult = messageService.SelectMsgClusterPrivate(cluster_no, user_id, kw);
            return Request.CreateResponse(HttpStatusCode.OK, publicResult);
        }
    }
}
