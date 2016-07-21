﻿using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.WebService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiContrib.ModelBinders;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class MessageController : ApiController
    {
        private MessageService messageService = new MessageService();
        
        [HttpGet]
        public IList<MsgPrivate> GetMessagePrivateList(string user_id, string date)
        {
            if (user_id == null || date == null) return null;

            DateTime dt = Convert.ToDateTime(date);
            IList<MsgPrivate> msgPrivates = messageService.SelectMsgPrivateForMobile(user_id, dt).Select(
                msgPrivateModel =>
                new MsgPrivate
                {
                    msg_no = msgPrivateModel.msg_no,
                    msg_title = msgPrivateModel.msg_title,
                    company = msgPrivateModel.company,
                    create_time = msgPrivateModel.create_time.ToString("yyyy-MM-dd HH:mm:ss:fff")
                }
            ).ToList();
            return msgPrivates;
        }

        [HttpGet]
        public MessageContent GetMessageContent(int msg_no)
        {
            if (msg_no == 0) return null;
            MessageContent messageContent = new MessageContent();
            MsgPrivateModel msgPrivateModel = messageService.SelectMsgPrivateOne(msg_no);
            messageContent.msgPrivate = new MsgPrivate {
                msg_no = msgPrivateModel.msg_no,
                msg_title = msgPrivateModel.msg_title,
                msg_content = msgPrivateModel.msg_content,
                company = msgPrivateModel.company,
                create_time = msgPrivateModel.create_time.ToString("yyyy-MM-dd HH:mm")
            };
            messageContent.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
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
        public object MessageReply(MsgPrivateReplyModel model)
        {
           return messageService.InsertMsgPrivateReply(model);
        }
    }
}
