using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using prj_BIZ_System.Models;
using prj_BIZ_System.Services;
using prj_BIZ_System.Controllers;
using WebApiContrib.ModelBinders;
using prj_BIZ_System.App_Start;
using prj_BIZ_System.WebService.Model;
using prj_BIZ_System.Extensions;

namespace prj_BIZ_System.WebService
{
    [MvcStyleBinding]
    public class UserInfoController : ApiController
    {
        private UserService userService = new UserService();
        private PasswordService passwordService = new PasswordService();

        [HttpPost]
        public UserInfo CheckUserInfo(string user_id, string user_pw)
        {
            user_pw = user_pw.ToUpper();
            MatchService matchService = new MatchService();
            UserInfoModel userInfoModel = userService.ChkUserInfoOne(user_id, user_pw);
            if(userInfoModel == null)
            {
                return null;
            }
            string[] activityIdBuyers = matchService.GetUserWhenActivityBuyer(user_id).Select(
                buyerInfo =>
                buyerInfo.activity_id.ToString()
            ).ToArray();
            string activityIdBuyer = string.Join(",", activityIdBuyers);
            UserInfo userInfo = new UserInfo(userInfoModel);
            userInfo.activity_id_buyer = activityIdBuyer;
            return userInfo;
        }

        [HttpGet] 
        public object GetCompanyOpenDataFromWeb(string user_id)
        {
            if(user_id.Length < 8)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "user id incomplete");
            }
            ManagerController.CompanyData data = ManagerController.GetDataFromWeb(user_id);

            if (data == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "no user data");
            }
            var enterprise_sort_ids = new HashSet<string>(data.Business_Item);
            var enterpriseSortList = userService.GetSortList()
                                                .Where(sort => enterprise_sort_ids.Contains(sort.enterprise_sort_id))
                                                .ToList();
            var sort_ids = enterpriseSortList.GetSelectList(sort => sort.sort_id);
            var enterprise_sorts = enterpriseSortList.GetSelectList(sort =>
                                                       sort.enterprise_sort_id + " " +
                                                       sort.enterprise_sort_name
                                                     );
            var companyOpenData = new
            {
                company = data.Company_Name,
                addr = data.Company_Location,
                capital = data.Capital_Stock_Amount,
                sort_id = string.Join(",", sort_ids),
                enterprise_sort = string.Join(",", enterprise_sorts)
            };
            return Request.CreateResponse(HttpStatusCode.OK, companyOpenData);
        }

        [HttpPost]
        public int ModifyUserInfo(UserInfoModel userInfoModel, string sort_id)
        {
            userInfoModel.user_pw = userInfoModel.user_pw.ToUpper();
            userInfoModel.id_enable = "1";
            bool isInsertSuccess = insertEnterpriseId(userInfoModel.user_id, sort_id);
            return isInsertSuccess == true ? userService.UserInfoUpdateOneForMobile(userInfoModel) : 0;
        }

        [HttpGet]
        public UserEnterpriseInfo UserInfo(string user_id)
        {
            if (user_id == null) return null;
            string[] enterprise_type = new string[]
            {
                "國內企業",
                "國外企業"
            };
            string[] enterprise_type_en = new string[]
            {
                "Domestic",
                "Foreign"
            };
            string[] revenue = new string[] 
            {
                "500萬以下", "501-1000萬", "1001-1500萬",
                "1501-3000萬", "3001-5000萬", "5000萬-1億",
                "一億以上"
            };
            string[] revenue_en = new string[]
            {
                "5 millions or Less", "5-10 millions", "10-15 millions",
                "15-30 millions", "30-50 millions", "50 millions-1 billion",
                "1 billion"
            };
            UserEnterpriseInfo userEnterpriseInfo = new UserEnterpriseInfo();

            UserInfoModel userInfoModel = userService.GeUserInfoOne(user_id);
            userEnterpriseInfo.userinfo = new UserInfo(userInfoModel);
            userEnterpriseInfo.userinfo.user_pw = null;
            int enterprise_typeNum = int.Parse(userEnterpriseInfo.userinfo.enterprise_type);
            int revenueNum = int.Parse(userEnterpriseInfo.userinfo.revenue);
            userEnterpriseInfo.userinfo.enterprise_type = enterprise_type[enterprise_typeNum];
            userEnterpriseInfo.userinfo.enterprise_type_en = enterprise_type_en[enterprise_typeNum];
            userEnterpriseInfo.userinfo.revenue = revenue[revenueNum];
            userEnterpriseInfo.userinfo.revenue_en = revenue_en[revenueNum];

            userEnterpriseInfo.usersortList = userService.SelectUserSortByUserId(user_id);
            return userEnterpriseInfo;
        }

        [HttpPost]
        public object UserInfoInsert(UserInfoModel userInfoModel, string sort_id)
        {
            userInfoModel.user_pw = userInfoModel.user_pw.ToUpper();
            userInfoModel.id_enable = "0";
            string errorInfo;
            int emailCode = MailHelper.checkEmail(userInfoModel.email, out errorInfo);

            if (!errorInfo.Equals("")) return "Email fail";

            bool isInsertSuccess = insertEnterpriseId(userInfoModel.user_id, sort_id);
            object userInfoId = null;
            if (isInsertSuccess)
            {
                try
                {
                    userInfoId = userService.UserInfoInsertForMobile(userInfoModel);
                    if (userInfoId != null)
                    {
                        SendAccountMailValidate(userInfoId, userInfoModel.user_id, userInfoModel.email);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("user_info_pkey"))
                    {
                        return "Account already exists";
                    }
                }
            }
            if (isInsertSuccess == true && userInfoId != null)
            {
                return userInfoId;
            }
            else
            {
                return "Userinfo insert fail";
            }
        }

        private bool insertEnterpriseId(string user_id, string sort_id)
        {
            bool isInsertSuccess = true;
            if (sort_id != null)
            {
                string[] strings = sort_id.Split(',');
                int[] enterprise_sort_id = new int[strings.Length];

                for (int i = 0; i < strings.Length; i++)
                {
                    enterprise_sort_id[i] = int.Parse(strings[i]);
                }
                try
                {
                    userService.RefreshUserSort(user_id, enterprise_sort_id);
                }
                catch (Exception ex)
                {
                    isInsertSuccess = false;
                }
            }
            else
            {
                isInsertSuccess = false;
            }

            return isInsertSuccess;
        }

        [HttpPost]
        public void SendAccountMailValidate(object id, string user_id, string email)
        {

            MailHelper.sendAccountMailValidate(id, user_id, email);
        }

        [HttpPost]
        public string ReSetPassword(string user_id, string email)
        {
            string errMsg = "200";
            UserInfoModel md = passwordService.SelectOneByIdEmail(user_id, email);
            if (md != null)
            {
                string new_pw = MailHelper.sendForgetPassword(md.email);
                var securityPassword = SecurityHelper.Encrypt256(new_pw);
                bool isUpdateSuccess = passwordService.UpdateUserPassword(md.user_id, securityPassword);
                if (!isUpdateSuccess)
                {
                    errMsg = "304";
                    return errMsg;
                }
            }
            else
            {
                errMsg = "400";
                return errMsg;
            }

            return errMsg;
        }

        [HttpGet]
        public object GetSortList()
        {
            return userService.GetSortList().Select(
                enterpriseSortListModel =>
                new
                {
                    enterpriseSortListModel.sort_id,
                    enterpriseSortListModel.enterprise_sort_id,
                    enterpriseSortListModel.enterprise_sort_name,
                    enterpriseSortListModel.enterprise_sort_name_en
                }
            ).ToList();
        }

        [HttpGet]
        public IList<CompanySortModel> GetCompanySortById(string sort_id)
        {
            return userService.SelectUserSortBySortId(int.Parse(sort_id), "");
        }

        [HttpGet]
        public IList<CompanySortModel> GetCompanySortByName(string company_name)
        {
            return userService.SelectUserKw(company_name).Select(
                userInfoModel =>
                new CompanySortModel
                {
                    user_id = userInfoModel.user_id,
                    company = userInfoModel.company,
                    company_en = userInfoModel.company_en,
                }
            ).ToList();
        }

        [HttpPost]
        public object VideoListInsert(string user_id, string video_name, string youtube_site)
        {
            if (user_id == null || video_name == null || youtube_site == null)
            {
                string message = string.Format("data has null.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            var result = userService.VideoListInsert(user_id, video_name, youtube_site);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public object GetAllProduct(string user_id)
        {
            if (user_id.IsNullOrEmpty())
            {
                string message = string.Format("user_id null.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            var allProduct = userService.getAllProduct(user_id).Select(
                                            product =>
                                            new
                                            {
                                                product.product_category,
                                                product.product_name,
                                                product.product_info,
                                                product.model_no,
                                                product.patent_or_winners,
                                                product.specifications_or_other,
                                                product.product_category_en,
                                                product.product_name_en,
                                                product.product_info_en,
                                                product.model_no_en,
                                                product.patent_or_winners_en,
                                                product.product_pic_site,
                                                product.specifications_or_other_en
                                            }
                                        );
            return Request.CreateResponse(HttpStatusCode.OK, allProduct);
        }

        [HttpGet]
        public object GetAllCatalog(string user_id)
        {
            if (user_id.IsNullOrEmpty())
            {
                string message = string.Format("user_id null.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            var allCatalog = userService.getAllCatalog(user_id).Select(
                                            catalogListModel => 
                                            new
                                            {
                                                catalogListModel.catalog_no,
                                                catalogListModel.catalog_name,
                                                catalogListModel.cover_file,
                                                catalogListModel.catalog_file
                                            }
                                        );
            return Request.CreateResponse(HttpStatusCode.OK, allCatalog);
        }

        [HttpGet]
        public object GetAllVideo(string user_id)
        {
            if (user_id.IsNullOrEmpty())
            {
                string message = string.Format("user_id null.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
            IList<Video> allVideo = userService.getAllVideo(user_id).Select(
                videoListModel =>
                new Video
                {
                    video_no = videoListModel.video_no,
                    video_name = videoListModel.video_name,
                    youtube_site = videoListModel.youtube_site
                }
            ).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, allVideo);
        }

        [HttpPost]
        public object DeleteVideoByNo(string user_id, int video_no)
        {
            int[] video_nos = new[] { video_no };

            if (user_id.IsNullOrEmpty() || video_no == null)
            {
                string message = string.Format("user_id or video_no null.");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            } 
            var result = userService.VideoListsDelete(user_id, video_nos);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
