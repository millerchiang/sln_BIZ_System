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
    enum MessageType
    {
        Person,
        CompanyPublic , CompanyPrivate,
        ClusterPublic , ClusterPrivate
    }

    public class MessageController : _BaseController
    {
        public MessageService messageService;
        public Message_ViewModel messageViewModel;
        public MessageController()
        {
            messageService = new MessageService();
            messageViewModel = new Message_ViewModel();
        }

        #region --私人訊息--
        public ActionResult MessagePrivateList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgPrivate(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            ViewBag.contentTitle = "私人";
            ViewBag.searchUrl = "MessagePrivateList";
            ViewBag.addUrl = "PrivateAdd";
            ViewBag.detailUrl = "PrivateDetailed";
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }
        
        public ActionResult PrivateAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            ViewBag.contentTitle = "私人";
            ViewBag.backUrl = "MessagePrivateList";
            return View();
        }

        public ActionResult DoPrivateAdd(MsgModel model , List<HttpPostedFileBase> iupexls)
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
                if (isOwnViewPower(msg_no,MessageType.Person)) //檢查權限
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

        public ActionResult doPrivateDetailed(MsgReplyModel model)
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
                result.Select( userInfo => new { value = userInfo.user_id, label = userInfo.company }).ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region --公司訊息--
        public ActionResult MessageCompanyList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgCompany(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            ViewBag.contentTitle = "公司";
            ViewBag.searchUrl = "MessageCompanyList";
            ViewBag.addUrl = "MessageCompanyAdd";
            ViewBag.detailUrl = "MessageCompanyDetailed";
            messageViewModel.msgPrivateList = result;
            return View("MessagePrivateList", messageViewModel);
        }

        public ActionResult MessageCompanyAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            ViewBag.contentTitle = "公司";
            ViewBag.backUrl = "MessageCompanyList";
            return View("PrivateAdd");
        }

        public ActionResult DoCompanyAdd(MsgModel model, List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");
            model.user_id = Request.Cookies["UserInfo"]["user_id"]; //先寫死成登入者帳號,之後要記得用業務槷反查
            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.msg_no = (long)messageService.InsertMsgPrivate(model); //這裡直接與私人訊息共用Service , 不是寫錯

            #region 上傳訊息附件
            if (iupexls != null && model.msg_no != 0)
            {
                foreach (HttpPostedFileBase file in iupexls)
                {
                    if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(model.creater_id))
                    {
                        Dictionary<string, string> uploadResult = null;
                        uploadResult = UploadHelper.doUploadFile(file, UploadConfig.subDirForMessageFile + model.msg_no, UploadConfig.AdminManagerDirName);
                        if ("success".Equals(uploadResult["result"]))
                        {
                            messageService.InsertMsgPrivateFile(model.msg_no, file.FileName);//uploadResult["relativFilepath"]
                        }
                        else
                        {
                            Console.WriteLine("上傳失敗");
                        }
                    }
                }
            }
            #endregion

            return Redirect("MessageCompanyList");
        }

        public ActionResult MessageCompanyDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (msg_no != 0)
            {
                if (isOwnViewPower(msg_no,MessageType.CompanyPublic)) //檢查權限
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
                return Redirect("MessageCompanyList");
            }
        }

        #endregion

        #region --聚落訊息--
        public ActionResult MessageClusterList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgPrivate(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }

        public ActionResult MessageClusterAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            return View();
        }

        public ActionResult MessageClusterDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Login");

            if (msg_no != 0)
            {
                if (isOwnViewPower(msg_no, MessageType.CompanyPublic)) //檢查權限
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
        #endregion

        private bool isOwnViewPower(int msg_no, MessageType mtype)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            switch (mtype)
            {
                case MessageType.Person:
                    return messageService.isOwnViewPower(msg_no, user_id);

                case MessageType.CompanyPublic:
                case MessageType.CompanyPrivate:
                    return messageService.isOwnViewPower(msg_no, user_id);

                case MessageType.ClusterPublic:
                case MessageType.ClusterPrivate: 

                default:
                    return false;
            }
        }
    }
}