using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static prj_BIZ_System.Models.MessageModel;

namespace prj_BIZ_System.Controllers
{
    public class MessageController : _BaseController
    {
        public MessageService messageService;
        public Message_ViewModel messageViewModel;
        public MessageController()
        {
            messageService = new MessageService();
            messageViewModel = new Message_ViewModel();
        }

        public ActionResult MessagePrivateList(string keyword)
        {
            IList<MsgPrivateModel> result = messageService.SelectMsgPrivate(keyword);
            ViewBag.keyword = keyword;
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }
        
        public ActionResult PrivateAdd()
        {
            return View();
        }

        public ActionResult DoPrivateAdd(MsgPrivateModel model)
        {
            model.creater_id = Request.Cookies["UserInfo"]["info"];
            messageService.InsertMsgPrivate(model);
            return View("MessagePrivateList");
        }

        public ActionResult PrivateDetailed(int msg_no)
        {
            if (msg_no!=0)
            {
                messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);
                messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                messageService.SelectMsgPrivateReplyMsg_no(msg_no);
            }
            return View();
        }
    }
}