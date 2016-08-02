using prj_BIZ_System.App_Start;
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
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgPrivateModel> result = messageService.SelectMsgPrivate(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }
        
        public ActionResult PrivateAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            return View();
        }

        public ActionResult DoPrivateAdd(MsgPrivateModel model , List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.msg_no = (long)messageService.InsertMsgPrivate(model);

            #region 上傳訊息附件
            if (iupexls != null && model.msg_no != 0)
            {
                foreach( HttpPostedFileBase file in iupexls)
                {
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(model.creater_id))
                    {
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(file, UploadConfig.subDirForMessageFile+model.msg_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            messageService.InsertMsgPrivateFile(model.msg_no,file.FileName);//uploadResult["relativFilepath"]
                        }
                        else
                        {
                            Console.WriteLine("上傳失敗");
                        }
                    }
                }
            }
            #endregion

            return Redirect("MessagePrivateList");
        }

        
        public ActionResult PrivateDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (msg_no != 0 )
            {
                if (isOwnViewPower(msg_no)) //檢查權限
                {
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(messageViewModel);
                }
                else
                {
                    TempData["priDetailView_errmsg"] = "很抱歉!您沒有觀看這則訊息的權限";
                    return Redirect("MessagePrivateList");
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = "很抱歉!請點選正確的訊息連結";
                return Redirect("MessagePrivateList");
            }
        }

        private bool isOwnViewPower(int msg_no)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            return messageService.isOwnViewPower(msg_no , user_id);
        }

        public ActionResult doPrivateDetailed(MsgPrivateReplyModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            //int msg_no
            model.msg_reply = Request.Cookies["UserInfo"]["user_id"];
            messageService.InsertMsgPrivateReply(model);
            return Redirect("PrivateDetailed?msg_no="+model.msg_no);
        }

        
        public ActionResult jsonMsgMemberFromUserInfo(string term)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<UserInfoModel> result = messageService.SelectUserKw(user_id , term);
            return Json(
                result.Select( 
                    userInfo => new { value = userInfo.user_id, label = userInfo.company }
                ).ToList()
                , JsonRequestBehavior.AllowGet);
        }
    }
}