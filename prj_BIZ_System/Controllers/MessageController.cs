using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            messageService.InsertMsgPrivate(model);
            return Redirect("MessagePrivateList");
        }

        public ActionResult PrivateDetailed(int msg_no)
        {
            if (msg_no!=0)
            {
                messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);
                messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
            }
            return View(messageViewModel);
        }

        public ActionResult doPrivateDetailed(int msg_no)
        {
            return View();
        }
    }
}