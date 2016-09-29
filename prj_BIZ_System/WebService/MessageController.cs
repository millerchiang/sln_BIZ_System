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
        public IList<MsgPrivate> GetMessagePrivateList(string user_id, string date)
        {
           if (user_id.IsNullOrEmpty() || date.IsNullOrEmpty()) return null;

            DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss:fff", System.Globalization.CultureInfo.CurrentCulture);
            IList<MsgPrivate> msgPrivates = messageService.SelectMsgPrivateForMobile(user_id, dt).Select(
                msgModel =>
                new MsgPrivate
                {
                    msg_no = msgModel.msg_no,
                    msg_title = msgModel.msg_title,
                    company = msgModel.company,
                    create_time = msgModel.create_time.ToString("yyyy-MM-dd HH:mm:ss:fff")                }
            ).ToList();
            IList<long> readMsgNo = messageService.SelectMsgReadNo(user_id);
            msgPrivates.ForEach(msgPrivate => {
                msgPrivate.is_read =
                readMsgNo.Contains(msgPrivate.msg_no) ? "1" : "0";
            });
            return msgPrivates;
        }

        [HttpGet]
        public MessageContent GetMessageContent(int msg_no)
        {
            if (msg_no == 0) return null;
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
            return messageContent;
        }

        [HttpGet]
        public MessageContent GetMessageContentAndRead(int msg_no, string user_id)
        {
            if (msg_no == 0) return null;
            MessageContent messageContent = new MessageContent();
            MsgModel msgModel = messageService.SelectMsgPrivateOneAndRead(msg_no, user_id);
            messageContent.msgPrivate = new MsgPrivate
            {
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
            return messageContent;
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
        public IList<CompanySortModel> GetCompanySort(string user_id)
        {
            if (user_id.IsNullOrEmpty()) return null;

            return messageService.SelectUserKwForMobile(user_id).Select(
                userInfoModel =>
                new CompanySortModel
                {
                    user_id = userInfoModel.user_id,
                    company = userInfoModel.company,
                    company_en = userInfoModel.company_en,
                }
            ).ToList();
        }
    }
}
