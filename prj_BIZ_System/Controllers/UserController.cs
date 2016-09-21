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

    public class UserController : _BaseController
    {
        public UserService userService;
        public User_ViewModel userModel;

        public PasswordService passwordService;
        public Password_ViewModel passwordViewModel;

        public SalesService salesInfoService;
        public Sales_ViewModel salesViewModel;

        public UserController()
        {
            userService = new UserService();
            userModel = new User_ViewModel();

            passwordService = new PasswordService();
            passwordViewModel = new Password_ViewModel();

            salesInfoService = new SalesService();
            salesViewModel = new Sales_ViewModel();
        }


        [HttpGet]
        public ActionResult UserInfo()
        {
//            if (Request.Cookies["UserInfo"] == null)
//                return Redirect("~/Home/Index");
            string user_id = Request["user_id"];
//            userModel.enterprisesortList = userService.GetSortList();
            userModel.userinfo = userService.GeUserInfoOne(user_id);
            userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
            userModel.productsortList = userService.getAllProduct(user_id).Pages<ProductListModel>(Request, this, 10);
            userModel.cataloglistList = userService.getAllCatalog(user_id);
            userModel.videolistList= userService.getAllVideo(user_id);

            ViewBag.coverDir = UploadHelper.getPictureDirPath(user_id, "catalog_cover");
            ViewBag.catalogDir = UploadHelper.getPictureDirPath(user_id, "catalog_file");
            ViewBag.logoDir = UploadHelper.getPictureDirPath(userModel.userinfo.user_id, "logo");
            docookie("_mainmenu", "UserInfo");
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
                ViewBag.tname = LanguageResource.User.lb_signup;
                userModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = LanguageResource.User.lb_submit_sure;
                cookie.Values.Add("edit", "Add");
                ViewBag.userSortList = "[]";
            }
            else //修改
            {
                ViewBag.tname = LanguageResource.User.lb_myinfo;
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
                ViewBag.SubmitName = LanguageResource.User.lb_edit;
                cookie.Values.Add("edit", "Update");
                cookie.Values.Add("user_id", userid);
            }

            Response.AppendCookie(cookie);
            docookie("_mainmenu", "Register");

            return View(userModel);
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
                model.user_pw = SecurityHelper.Encrypt256(model.user_pw);
                var id = userService.UserInfoInsertOne(model);
                if( id != null)
                {
                    MailHelper.sendAccountMailValidate( id , model.user_id,model.email );
                }
                
            }
            else //修改
            {
                if (Request.Cookies["UserInfo"] == null)
                    return Redirect("~/Home/Index");

                string current_user_id = Request.Cookies["UserInfo"]["user_id"];
                var old_model = userService.GeUserInfoOne(current_user_id);
                model.update_time = DateTime.Now;
                model.user_id = current_user_id;
                if (logo_img != null && logo_img.ContentLength > 0 && !string.IsNullOrEmpty(current_user_id))
                {
                    if (old_model.logo_img!=null)
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
//                return Redirect("../Home/Index");
              return Redirect("Register");
            else
                return Redirect("../Home/Verification?user_id=" + model.user_id + "&name=" + name + "&email=" + model.email);
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
                        var days = new TimeSpan(DateTime.Now.Ticks - checkTime.Ticks).TotalDays;
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

            return Redirect("../Home/Index");
        }

        #region 產品說明
        public ActionResult ProductList(string user_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            //string user_id = Request.Cookies["Action"]["user_id"];
            ViewBag.user_id = user_id;
            IList<ProductListModel> productLists = userService.getAllProduct(user_id);
            return View(productLists);
        }

        [HttpPost]
        public ActionResult doProductInsertOrUpdate(ProductListModel model)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            if(model.product_id == null)
            {
                model.user_id = Request.Cookies["UserInfo"]["user_id"];
                int product_id = (int)userService.insertProductList(model);
            }
            else
            {
                model.user_id = Request.Cookies["UserInfo"]["user_id"];
                int updateCount = (int)userService.updateProductList(model);
            }

            return Redirect("ProductListEdit");
        }

        [HttpPost]
        public ActionResult ProductDelete(int[] del_prods)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            try
            {
                string user_id =  Request.Cookies["UserInfo"]["user_id"];
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
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            try
            {
                string user_id =  Request.Cookies["UserInfo"]["user_id"];
                userService.ProductListRefresh(user_id, old_prods, new_prods);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        public ActionResult ProductListEdit()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<ProductListModel> productLists = userService.getAllProduct(user_id).Pages< ProductListModel>(Request,this,10);
            return View(productLists);
        }

        public ActionResult ProductDetail(int? product_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            ProductListModel result = userService.getProductOne(product_id);
            return View(result);
        }

        public ActionResult ProductDetailEdit(int? product_id)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            ProductListModel result = userService.getProductOne(product_id);
            return result==null? View(new ProductListModel()) : View(result);
        }
        #endregion

        #region 型錄管理
        public ActionResult CatalogList()
        {
            if (Request.Cookies["Action"] == null)
                return Redirect("~/Home/Index");
            string user_id =  Request.Cookies["Action"]["user_id"];
            IList<CatalogListModel> catalogLists = userService.getAllCatalog(user_id);
            ViewBag.coverDir = UploadHelper.getPictureDirPath(user_id, "catalog_cover");
            ViewBag.catalogDir = UploadHelper.getPictureDirPath(user_id, "catalog_file");
            return View(catalogLists);
        }

        public ActionResult CatalogCreate(int[] catalog_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            return View();
        }

        [HttpPost]
        public ActionResult CatalogDelete(int[] catalog_no)
        {
            if (Request.Cookies["Action"] == null)
                return Redirect("~/Home/Index");
            string user_id =  Request.Cookies["Action"]["user_id"];
            IList<CatalogListModel> catalogLists =userService.SelectCatalogListByCatalogNo(user_id, catalog_no);

            #region 刪除檔案
            string targetRootDir = Path.Combine(UploadConfig.UploadRootDir, user_id);
            string targetCoverPath = "";
            string targetCatalogPath = "";
            targetCoverPath = Path.Combine(targetRootDir, UploadConfig.subDirForCover);
            targetCatalogPath = Path.Combine(targetRootDir, UploadConfig.subDirForCatalog);

            foreach (CatalogListModel catalog in  catalogLists)
            {
                UploadHelper.deleteUploadFile(catalog.cover_file, "catalog_cover", user_id);
                UploadHelper.deleteUploadFile(catalog.catalog_file, "catalog_file", user_id);
            }
            #endregion

            #region 刪除DB資料
                userService.CatalogListsDelete(user_id, catalog_no);
            #endregion

            return Redirect("CatalogList");
        }

        [HttpPost]
        public ActionResult CatalogUpload(string catalog_name , HttpPostedFileBase cover_file , HttpPostedFileBase catalog_file)
        {
            if (Request.Cookies["Action"] == null)
                return Redirect("~/Home/Index");
            if (cover_file != null && catalog_file !=null)
            {
                if(cover_file.ContentLength > 0 && catalog_file.ContentLength > 0)
                {
                    string user_id = Request.Cookies["Action"]["user_id"];
                    UploadHelper.doUploadFile(cover_file, UploadConfig.subDirForCover , user_id);
                    UploadHelper.doUploadFile(catalog_file, UploadConfig.subDirForCatalog, user_id);
                    bool isUploadSuccess = userService.CatalogListInsert(user_id, catalog_name , cover_file.FileName, catalog_file.FileName);
                }
            }
            return Redirect("CatalogList");
        }
        #endregion

        #region 影音型錄管理
        public ActionResult VideoList()
        {

            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            IList<VideoListModel> videoLists = userService.getAllVideo(user_id);
            return View(videoLists);
        }

        public ActionResult VideoListCreate(int[] video_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            return View();
        }

        [HttpPost]
        public ActionResult VideoDelete(int[] video_no)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["UserInfo"]["user_id"];
            userService.VideoListsDelete(user_id, video_no);
            return Redirect("VideoList");
        }

        [HttpPost]
        public ActionResult VideoUpload(string video_name, string youtube_site)
        {
            if (Request.Cookies["Action"] == null)
                return Redirect("~/Home/Index");
            string user_id = Request.Cookies["Action"]["user_id"];
            var isUploadSuccess = userService.VideoListInsert(user_id, video_name, youtube_site);
            return Redirect("VideoList");
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

        #region 密碼編輯
        // GET: Password
        public ActionResult EditPasswd()
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            return View();
        }

        //修改密碼
        public ActionResult PasswordInsertUpdate(string old_pw, string new_pw)
        {
            if (Request.Cookies["UserInfo"] == null)
                return Redirect("~/Home/Index");

            string current_id = "";
            string current_user_id = Request.Cookies["UserInfo"]["user_id"]; // 取 user_id 的 cookie

            string errMsg = "修改成功";

            if (!string.IsNullOrEmpty(current_user_id))
            {
                current_id = current_user_id;
                var securityOldPassword = SecurityHelper.Encrypt256(old_pw);
                var securityNewPassword = SecurityHelper.Encrypt256(new_pw);
                if (passwordService.getUserPassword(current_id).Equals(securityOldPassword))
                {
                    if (!passwordService.UpdateUserPassword(current_id, securityNewPassword))
                    {
                        errMsg = "修改失敗";
                    }
                }
                else
                {
                    errMsg = "輸入的舊密碼不正確";
                }
            }

            TempData["pw_errMsg"] = errMsg;

            return Redirect("Register");
        }
        #endregion
        
    }
}