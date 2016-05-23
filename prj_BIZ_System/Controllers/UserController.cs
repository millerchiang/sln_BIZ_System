﻿using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
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


            if (Request["user_id"] == null) //新增
            {
                userModel.userinfo = new UserInfoModel();
                ViewBag.PageType = "Create";
                ViewBag.SubmitName = "確定送出";
                Response.Cookies["UserInfo"]["edit"] = "Add";

            }
            else //修改
            {
                userModel.userinfo = userService.GeUserInfoOne(Request["user_id"]);
                ViewBag.user = userModel.userinfo;
                userModel.usersortList = userService.SelectUserSortByUserId(userModel.userinfo.user_id);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.userSortList = serializer.Serialize(userModel.usersortList);
                ViewBag.PageType = "Edit";
                ViewBag.SubmitName = "修改";
                Response.Cookies["UserInfo"]["edit"] = "Update";
            }
            return View(userModel);
        }



        public ActionResult DeleteUser()
        {
            userService.UserInfoDelectOne(Request["user_id"]);
            return Redirect("UserList");
        }

        [HttpPost]
        public ActionResult UserInsertUpdate(UserInfoModel model , int[] sort_id)
        {
            if (Request.Cookies["UserInfo"]["edit"] == "Add")//新增
            {
                userService.UserInfoInsertOne(model);
            }
            else //修改
            {
                model.update_time = DateTime.Now;
                userService.UserInfoUpdateOne(model);
            }

            bool refreshResult = userService.RefreshUserSort(model.user_id,sort_id);
            string name = Request["company"];
            if (name == "")
                name = Request["company_en"];
            return Redirect("../Home/Verification?name=" + name + "&email=" + Request["email"]);
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