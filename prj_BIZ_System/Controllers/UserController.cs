using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.ViewModels;
using System;
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

        public UserController()
        {
            userService = new UserService();
        }

        public ActionResult UserList()
        {
            IList<UserInfoModel> userInfoModels = userService.GetUserInfoList();
            ViewData["list"] = userInfoModels;
            return View();
        }



        public ActionResult UserInsert()
        {

            if (Request["user_id"] != null)
            {
                UserInfoModel model = new UserInfoModel();
                model.user_id = Request["user_id"];
                model.user_pw = Request["user_pw"];
                model.enterprise_type = Request["enterprise_type"];
                model.company = Request["company"];
                //            model.endtime = DateTime.Parse(Request["endtime"]);
                model.company_en = Request["company_en"];
                model.leader = Request["leader"];
                model.addr = Request["addr"];
                model.leader_en = Request["leader_en"];
                model.addr_en = Request["addr_en"];
                model.contact = Request["contact"];
                model.contact_en = Request["contact_en"];
                model.phone = Request["phone"];
                model.email = Request["email"];
                model.capital = int.Parse(Request["capital"]);
                model.revenue = Request["revenue"];
                model.website = Request["website"];
                model.info = Request["info"];
                model.info_en = Request["info_en"];
                userService.UserInfoInsertOne(model);
            }
            return Redirect("../Home/Index");
//            return Redirect("UserList");
        }

        [HttpGet]
        public ActionResult Register()
        {
            UserInfoModel model = null;
            User_register_ViewModels urViewModel = new User_register_ViewModels();
            string user_id = Request["user_id"];
            if (Request["user_id"] != null) //修改
            {
                model = userService.GeUserInfoOne(user_id);
                ViewBag.user = model;
                IList < UserSortModel > userSortList  = userService.SelectUserSortByUserId(model.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(userSortList);
                ViewBag.nextAction = "UserUpdate";
                ViewBag.nextName = "確認修改";
            }
            else //新增
            {
                ViewBag.nextAction = "UserInsert";
                ViewBag.nextName = "確定送出";
            }

            IList<EnterpriseSortModel> enterpriseSortModel = userService.GetSortList();
            urViewModel.enterpriseSortModel = userService.GetSortList();
            ViewData["sortlist"] = enterpriseSortModel;

            return View(model);
        }



        public ActionResult DeleteUser()
        {
            UserInfoModel model = new UserInfoModel();
            model.user_id = Request["user_id"];
            userService.UserInfoDelectOne(model.user_id);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserUpdate(UserInfoModel model , int[] sort_id)
        {

//            model.user_id = Request["user_id"];
            model.user_pw = Request["user_pw"];
            model.enterprise_type = Request["enterprise_type"];
            model.company = Request["company"];
            //            model.endtime = DateTime.Parse(Request["endtime"]);
            model.company_en = Request["company_en"];
            model.leader = Request["leader"];
            model.addr = Request["addr"];
            model.leader_en = Request["leader_en"];
            model.addr_en = Request["addr_en"];
            model.contact = Request["contact"];
            model.contact_en = Request["contact_en"];
            model.phone = Request["phone"];
            model.email = Request["email"];
            model.capital = int.Parse(Request["capital"]);
            model.revenue = Request["revenue"];
            model.website = Request["website"];
            model.info = Request["info"];
            model.info_en = Request["info_en"];
            model.update_time = DateTime.Now;
            userService.UserInfoUpdateOne(model);
            bool refreshResult = userService.RefreshUserSort(model.user_id,sort_id);
            return Redirect("../Home/Index");
        }

        #region 產品說明
        public ActionResult ProductList()
        {
            string user_id =  _loginUserId;
            IList<ProductListModel> productLists = userService.getAllProduct(user_id);
            return View(productLists);
        }
        
        [HttpPost]
        public ActionResult ProductDelete(int[] del_prods)
        {
            try
            {
                string user_id =  _loginUserId;
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
                string user_id =  _loginUserId;
                userService.ProductListRefresh(user_id, old_prods, new_prods);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }
        #endregion

        #region 型錄上傳
        public ActionResult CatalogList()
        {
            string user_id =  _loginUserId;
            IList<CatalogListModel> catalogLists = userService.getAllCatalog(user_id);
            ViewBag.coverDir = UploadConfig.CatalogRootPath + user_id + "/" + UploadConfig.subDirForCover;
            return View(catalogLists);
        }

        public ActionResult CatalogCreate(int[] catalog_no)
        {
           

            return View();
        }

        [HttpPost]
        public ActionResult CatalogDelete(int[] catalog_no)
        {
            string user_id =  _loginUserId;
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
                    #region 建立資料夾
                    string user_id =  _loginUserId;
                    string targetRootDir = Path.Combine(UploadConfig.CatalogRootDir, user_id);
                    string targetCoverPath = "";
                    string targetCatalogPath = "";
                    targetCoverPath = Path.Combine(targetRootDir, UploadConfig.subDirForCover);
                    targetCatalogPath = Path.Combine(targetRootDir, UploadConfig.subDirForCatalog);

                    if (!Directory.Exists(targetRootDir)){
                        Directory.CreateDirectory(targetRootDir);
                        Directory.CreateDirectory(targetCoverPath);
                        Directory.CreateDirectory(targetCatalogPath);
                    }
                    #endregion

                    #region 上傳檔案
                    string targetCoverFilePath = Path.Combine(targetCoverPath, cover_file.FileName);
                    string targetCatalogFilePath = Path.Combine(targetCatalogPath, catalog_file.FileName);
                    cover_file.SaveAs(targetCoverFilePath);
                    catalog_file.SaveAs(targetCatalogFilePath);
                    #endregion

                    bool isUploadSuccess = userService.CatalogListInsert(user_id, cover_file.FileName, catalog_file.FileName);
                }
            }
            return Redirect("CatalogList");
        }
        #endregion

        public ActionResult _NavSearchPartial()
        {
            return PartialView();
        }
    }
}