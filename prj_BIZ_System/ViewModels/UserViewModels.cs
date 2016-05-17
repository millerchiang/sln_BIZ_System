using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.ViewModels
{
    public class User_register_ViewModels
    {
        public UserInfoModel userInfoModel { get; set; }
        public IList<EnterpriseSortModel> enterpriseSortModel { get; set; }
        public IList<UserSortModel> UserSortModel { get; set; }
    }
}