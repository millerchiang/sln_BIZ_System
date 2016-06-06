using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace prj_BIZ_System.Controllers
{

    public class UserController : Controller
    {
        public UserService userService;
        public User_ViewModel userModel;

        public UserController()
        {
            userService = new UserService();
            userModel = new User_ViewModel();
        }

        public ActionResult UserList()
        {
            userModel.userinfoList = userService.GetUserInfoList();
            return View(userModel);
        }


        [HttpGet]
        public ActionResult UserInfo()
        {
            userModel.enterprisesortList = userService.GetSortList();
            userModel.userinfo = userService.GeUserInfoOne(Request["user_id"]);
            userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
            return View(userModel);
        }


        [HttpGet]
        public ActionResult Register()
        {
            userModel.enterprisesortList = userService.GetSortList();
            ViewBag.Action = "UserInsertUpdate";
            string userid = Request["user_id"];
            if (userid == null)
            {
                if (Request.Cookies["UserInfo"] != null)
                    userid = Request.Cookies["UserInfo"]["user_id"];
            }
            else if (userid=="new")
            {
                userid = null; 
            }

            HttpCookie cookie = new HttpCookie("Action");

            if (userid == null) //新增
            {
                userModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "確定送出";
                cookie.Values.Add("edit", "Add");
                ViewBag.userSortList = "[]";
            }
            else //修改
            {
                userModel.userinfo = userService.GeUserInfoOne(userid);
                ViewBag.user = userModel.userinfo;
                userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(userModel.usersortList);
                ViewBag.logoDir = UploadHelper.getPictureDirPath(userModel.userinfo.user_id, "logo");
                if (ViewBag.userSortList == null)
                {
                    ViewBag.userSortList = "[]";
                }
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
                cookie.Values.Add("edit", "Update");
                cookie.Values.Add("user_id", userid);
            }

            Response.AppendCookie(cookie);
            return View(userModel);
        }



        public ActionResult DeleteUser()
        {
            userService.UserInfoDelectOne(Request["user_id"]);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserInsertUpdate(UserInfoModel model , int[] sort_id , HttpPostedFileBase logo_img)
        {
            if (Request.Cookies["Action"]["edit"] == "Add")//新增
            {
                if (logo_img != null && logo_img.ContentLength > 0 && !string.IsNullOrEmpty(model.user_id))
                {
                    UploadHelper.doUploadFile(logo_img, UploadConfig.subDirForLogo, model.user_id);
                    model.logo_img = logo_img.FileName;
                }
                var id = userService.UserInfoInsertOne(model);
                if( id != null)
                {
                    MailHelper.sendAccountMailValidate( id , model.user_id,model.email , Request.Url.Host , Request.Url.Port);
            }
                
            }
            else //修改
            {
                string current_user_id = Request.Cookies["UserInfo"]["user_id"];
                var old_model = userService.GeUserInfoOne(current_user_id);
                model.update_time = DateTime.Now;
                model.user_id = current_user_id;
                if (logo_img != null && logo_img.ContentLength > 0 && !string.IsNullOrEmpty(current_user_id))
                {
                    UploadHelper.deleteUploadFile(old_model.logo_img, "logo",current_user_id);
                    UploadHelper.doUploadFile(logo_img, UploadConfig.subDirForLogo, model.user_id);
                    model.logo_img = logo_img.FileName;
                }
                userService.UserInfoUpdateOne(model);

            }

            bool refreshResult = userService.RefreshUserSort(model.user_id,sort_id);
            string name = Request["company"];
            if (name == "")
                name = Request["company_en"];

            if (model.id_enable=="1")
                return Redirect("../Home/Index");
            else
            return Redirect("../Home/Verification?name=" + name + "&email=" + Request["email"]);
        }

        

        public ActionResult AccountMailValidate(string validate_linkX)
        {
            string link = SecurityHelper.Decrypt(validate_linkX);
            string[] datas = link.Split(new string[]{ "+" }, StringSplitOptions.RemoveEmptyEntries);

            const int expired_limit_days = 3; //期限
            const string status_success     = "您的會員帳號已成功開通，請於首頁登入使用，謝謝。";
            const string status_expired     = "您的會員驗證已過期，請重發驗證信或重新註冊，謝謝。";
            const string status_beValidated = "您已驗證過本會員帳號，請於首頁登入使用，謝謝。";
            const string status_fail        = "您的會員驗證參數錯誤，請重發驗證信或聯絡客服人員，謝謝。";

            UserInfoModel dbUser = userService.GeUserInfoOne(datas[1]);
            string result ;
            if ("0".Equals(dbUser.id_enable))
            {
                if (dbUser.id.ToString().Equals(datas[0].ToString()))
                {
                    if (datas[2]!=null)
                    {
                        DateTime checkTime = DateTime.Parse(datas[2]);
                        var days = new TimeSpan(DateTime.Now.Ticks - checkTime.Ticks).Days;
                        if (days > expired_limit_days ) 
                        {
                            result = status_expired;
                        }
                        else
                        {
                            if (userService.UserInfoUpdateIdEnable(dbUser.id, "1"))
                            {
                                result = status_success;
                            }
                            else
                            {
                                result = status_fail​;
                            }
                        }
                    }
                    else
                    {
                        result = status_fail​;
                    }
                }
                else
                {
                    result = status_fail​;
                }
            }
            else if("1".Equals(dbUser.id_enable))
            {
                result = status_beValidated;
            }
            else
            {
                result = status_fail;
            }

            TempData["MailValidateResult"] = result;

            return Redirect("../Home/Login");
        }

        #region 產品說明
        public ActionResult ProductList()
        {
            string user_id = Request.Cookies["Action"]["user_id"];
            IList<ProductListModel> productLists = userService.getAllProduct(user_id);
            return View(productLists);
        }
        
        [HttpPost]
        public ActionResult ProductDelete(int[] del_prods)
        {
            try
            {
                string user_id =  Request.Cookies["Action"]["user_id"];
                bool isDelSuccess = userService.ProductListDelete(user_id, del_prods);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPost]
        public ActionResult ProductInsert(List<ProductListModel> old_prods, List<ProductListModel> new_prods)
        {
            try
            {
                string user_id =  Request.Cookies["Action"]["user_id"];
                userService.ProductListRefresh(user_id, old_prods, new_prods);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }
        #endregion

        #region 型錄管理
        public ActionResult CatalogList()
        {
            string user_id =  Request.Cookies["Action"]["user_id"];
            IList<CatalogListModel> catalogLists = userService.getAllCatalog(user_id);
            ViewBag.coverDir = UploadHelper.getPictureDirPath(user_id, "catalog_cover");
            ViewBag.catalogDir = UploadHelper.getPictureDirPath(user_id, "catalog_file");
            return View(catalogLists);
        }

        public ActionResult CatalogCreate(int[] catalog_no)
        {
            return View();
        }

        [HttpPost]
        public ActionResult CatalogDelete(int[] catalog_no)
        {
            string user_id =  Request.Cookies["Action"]["user_id"];
            IList<CatalogListModel> catalogLists =userService.SelectCatalogListByCatalogNo(user_id, catalog_no);

            #region 刪除檔案
            string targetRootDir = Path.Combine(UploadConfig.CatalogRootDir, user_id);
            string targetCoverPath = "";
            string targetCatalogPath = "";
            targetCoverPath = Path.Combine(targetRootDir, UploadConfig.subDirForCover);
            targetCatalogPath = Path.Combine(targetRootDir, UploadConfig.subDirForCatalog);

            foreach (CatalogListModel catalog in  catalogLists)
            {
                System.IO.File.Delete(Path.Combine(targetCoverPath, catalog.cover_file));
                System.IO.File.Delete(Path.Combine(targetCatalogPath, catalog.catalog_file));
            }
            #endregion

            #region 刪除DB資料
                userService.CatalogListsDelete(user_id, catalog_no);
            #endregion

            return Redirect("CatalogList");
        }

        [HttpPost]
        public ActionResult CatalogUpload(HttpPostedFileBase cover_file , HttpPostedFileBase catalog_file)
        {
            if (cover_file != null && catalog_file !=null)
            {
                if(cover_file.ContentLength > 0 && catalog_file.ContentLength > 0)
                {
                    string user_id = Request.Cookies["Action"]["user_id"];
                    UploadHelper.doUploadFile(cover_file, UploadConfig.subDirForCover , user_id);
                    UploadHelper.doUploadFile(catalog_file, UploadConfig.subDirForCatalog, user_id);
                    bool isUploadSuccess = userService.CatalogListInsert(user_id, cover_file.FileName, catalog_file.FileName);
                }
            }
            return Redirect("CatalogList");
        }
        #endregion

        public ActionResult _NavSearchPartial()
        {
            IList<EnterpriseSortListModel> result ;
            var isCacheON = CacheConfig._NavSearchPartial_load_cache_isOn;
            if (isCacheON)
            {
                if(CacheDataStore.EnterpriseSortListModelCache == null)
                {
                    CacheDataStore.EnterpriseSortListModelCache = userService.GetSortList();
                }
                result = CacheDataStore.EnterpriseSortListModelCache;
            }
            else
            {
                result = userService.GetSortList();
            }
            return PartialView(result);
        }
    }
}