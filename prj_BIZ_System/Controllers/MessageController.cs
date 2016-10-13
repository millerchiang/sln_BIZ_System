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

    public enum MessageCatalog
    {
        Private,
        Company,
        Cluster
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
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            var totalMsg = messageService.SelectMsgPrivate(keyword, user_id);
            IList<MsgModel> result = totalMsg.Where( msg => "0".Equals(msg.is_read)).ToList().Pages(Request, this, 10); ;
            IList<MsgModel> result2 = totalMsg.Where(msg => "1".Equals(msg.is_read)).ToList().Pages(Request, this, 10); ;

            ViewBag.keyword = keyword;
            ViewBag.contentTitle    = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.searchUrl       = getLabelString(MessageCatalog.Private, "searchUrl");
            ViewBag.addUrl          = getLabelString(MessageCatalog.Private, "addUrl");
            ViewBag.detailUrl       = getLabelString(MessageCatalog.Private, "detailUrl");
            messageViewModel.msgPrivateList = result;
            messageViewModel.msgPrivateList2 = result2;
            return View(messageViewModel);
        }
        
        public ActionResult PrivateAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            ViewBag.msgType      = getLabelString(MessageCatalog.Private, "msgType");
            ViewBag.contentTitle = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Private, "backUrl");
            ViewBag.doAddUrl     = getLabelString(MessageCatalog.Private, "doAddUrl");
            return View();
        }

        public ActionResult DoPrivateAdd(MsgModel model , List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

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
                            Console.WriteLine(LanguageResource.User.lb_upload_fail);
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
                return Redirect("~/Home/Index");

            string current_user_id = Request.Cookies["UserInfo"]["user_id"];
            ViewBag.contentTitle = getLabelString(MessageCatalog.Private, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Private, "backUrl");

            if (msg_no != 0 )
            {
                if (isOwnViewPower(msg_no,MessageType.Person)) //檢查權限
                {
                    //messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOneAndRead(msg_no , current_user_id);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member,MessageCatalog.Private);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(messageViewModel);
                }
                else
                {
                    TempData["priDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect(getLabelString(MessageCatalog.Private, "backUrl"));
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = LanguageResource.User.lb_click_correct;
                return Redirect(getLabelString(MessageCatalog.Private, "backUrl"));
            }
        }

        public ActionResult doPrivateDetailed(MsgReplyModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            //int msg_no
            model.msg_reply = Request.Cookies["UserInfo"]["user_id"];
            messageService.InsertMsgPrivateReply(model);
            return Redirect(getLabelString(MessageCatalog.Private, "detailUrl")+"?msg_no=" +model.msg_no);
        }

        
        public ActionResult jsonMsgMemberForPrivate(string term)
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
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgCompany(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.searchUrl    = getLabelString(MessageCatalog.Company, "searchUrl");
            ViewBag.addUrl       = getLabelString(MessageCatalog.Company, "addUrl");
            ViewBag.detailUrl    = getLabelString(MessageCatalog.Company, "detailUrl");
            messageViewModel.msgPrivateList = result;
            return View("MessagePrivateList", messageViewModel);
        }

        public ActionResult MessageCompanyAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            ViewBag.msgType      = getLabelString(MessageCatalog.Company, "msgType");
            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Company, "backUrl");
            ViewBag.doAddUrl     = getLabelString(MessageCatalog.Company, "doAddUrl");

            return View(getLabelString(MessageCatalog.Private, "addUrl")); //共用
        }

        public ActionResult DoCompanyAdd(MsgModel model, List<HttpPostedFileBase> iupexls)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            model.user_id = Request.Cookies["UserInfo"]["user_id"]; //先寫死成登入者帳號,之後要記得用業務槷反查
            model.creater_id = Request.Cookies["UserInfo"]["user_id"];
            model.msg_no = (long)messageService.InsertMsgCompany(model); //這裡直接與私人訊息共用Service , 不是寫錯

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
                            Console.WriteLine(LanguageResource.User.lb_upload_fail);
                        }
                    }
                }
            }
            #endregion

            return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
        }

        public ActionResult MessageCompanyDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            ViewBag.contentTitle = getLabelString(MessageCatalog.Company, "contentTitle");
            ViewBag.backUrl      = getLabelString(MessageCatalog.Company, "backUrl");

            if (msg_no != 0)
            {
                if (isOwnViewPower(msg_no,MessageType.CompanyPublic)) //檢查權限
                {
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member,MessageCatalog.Company);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(getLabelString(MessageCatalog.Private, "detailUrl"), messageViewModel); //共用
                }
                else
                {
                    TempData["priDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = LanguageResource.User.lb_click_correct;
                return Redirect(getLabelString(MessageCatalog.Company, "backUrl"));
            }
        }

        public ActionResult jsonMsgMemberForCompany(string term)
        {
            var user_id = Request.Cookies["UserInfo"]["user_id"];
            //var sales_id = Request.Cookies["UserInfo"]["user_id"];
            IList<SalesInfoModel> result = messageService.SelectSalesKw(user_id, term);
            return Json(
                result.Select(salesInfo => new { value = salesInfo.sales_id, label = salesInfo.sales_name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --聚落訊息--

        public ActionResult MessageClusterMain(string cluster_public_where , string cluster_private_where)
        {
            if (Request.Cookies["UserInfo"] == null || Request["cluster_no"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            ViewBag.cluster_public_where = cluster_public_where;
            ViewBag.cluster_private_where = cluster_private_where;
            IList<MsgModel> result_public  = messageService.SelectMsgClusterPublic(Request["cluster_no"], user_id , cluster_public_where);
            IList<MsgModel> result_private = messageService.SelectMsgClusterPrivate(Request["cluster_no"], user_id, cluster_private_where);
            return View();
        }

        public ActionResult MessageClusterAddPrivate()
        {
            return View();
        }

        public ActionResult MessageClusterAddPublic()
        {
            return View();
        }

        public ActionResult MessageClusterList(string keyword)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            var user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<MsgModel> result = messageService.SelectMsgPrivate(keyword, user_id).Pages(Request, this, 10); ;
            ViewBag.keyword = keyword;
            messageViewModel.msgPrivateList = result;
            return View(messageViewModel);
        }

        public ActionResult MessageClusterAdd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            return View();
        }

        public ActionResult MessageClusterDetailed(int msg_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            string current_user_id = Request.Cookies["UserInfo"]["user_id"];
            if (msg_no != 0)
            {
                if (isOwnViewPower(msg_no, MessageType.CompanyPublic)) //檢查權限
                {
                    
                    //messageViewModel.msgPrivate = messageService.SelectMsgPrivateOne(msg_no);
                    messageViewModel.msgPrivate = messageService.SelectMsgPrivateOneAndRead(msg_no , current_user_id);

                    ViewBag.msg_company = messageService.transferMsg_member2Msg_company(messageViewModel.msgPrivate.msg_member,MessageCatalog.Cluster);
                    messageViewModel.msgPrivateFileList = messageService.SelectMsgPrivateFileByMsg_no(msg_no);
                    messageViewModel.msgPrivateReplyList = messageService.SelectMsgPrivateReplyMsg_no(msg_no);
                    return View(messageViewModel);
                }
                else
                {
                    TempData["priDetailView_errmsg"] = LanguageResource.User.lb_msg_limit;
                    return Redirect("MessagePrivateList");
                }
            }
            else
            {
                TempData["priDetailView_errmsg"] = LanguageResource.User.lb_click_correct;
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

        /// <summary>
        /// ( catalog : Private, Company, Cluster )
        /// ( key :contentTitle／searchUrl／addUrl／detailUrl )
        /// </summary>
        private string getLabelString(MessageCatalog catalog,string key)
        {
            const string
                          msgType       = "msgType"
                        , contentTitle  = "contentTitle"
                        , searchUrl     = "searchUrl" 
                        , addUrl        = "addUrl" 
                        , doAddUrl      = "doAddUrl"
                        , detailUrl     = "detailUrl"
                        , backUrl       = "backUrl";

            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            resultDict.Add(msgType, "");
            resultDict.Add(contentTitle, "");
            resultDict.Add(searchUrl, "");
            resultDict.Add(addUrl, "");
            resultDict.Add(doAddUrl, "");
            resultDict.Add(detailUrl, "");
            resultDict.Add(backUrl, "");

            switch (catalog)
            {
                case MessageCatalog.Private:
                    resultDict[msgType]         = "Private";
                    resultDict[contentTitle]    = "私人";
                    resultDict[searchUrl]       = "MessagePrivateList";
                    resultDict[addUrl]          = "PrivateAdd";
                    resultDict[doAddUrl]        = "DoPrivateAdd";
                    resultDict[detailUrl]       = "PrivateDetailed";
                    resultDict[backUrl]         = "MessagePrivateList";
                    break;

                case MessageCatalog.Company:
                    resultDict[msgType]         = "Company";
                    resultDict[contentTitle]    = "公司";
                    resultDict[searchUrl]       = "MessageCompanyList";
                    resultDict[addUrl]          = "MessageCompanyAdd";
                    resultDict[doAddUrl]        = "DoCompanyAdd";
                    resultDict[detailUrl]       = "MessageCompanyDetailed";
                    resultDict[backUrl]         = "MessageCompanyList";
                    break;
            }


            return resultDict.ContainsKey(key)? resultDict[key] : "";

        }
    }
}