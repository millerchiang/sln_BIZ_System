using prj_BIZ_System.App_Start;
using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prj_BIZ_System.Controllers
{
    public class _BaseController : Controller
    {
        public UserInfoModel _loginUser { get; set; }
        public string _CatalogCoverDir { get; set; }
        public string _CatalogCatalogDir { get; set; }
        public _BaseController()
        {
            // 測試用資料
            _loginUser = new UserInfoModel() { user_id = "12345678" };
            _CatalogCoverDir = CustomConfig.CatalogCoverDir;
            _CatalogCatalogDir = CustomConfig.CatalogCatalogDir;
        }
    }
}