﻿using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using prj_BIZ_System.App_Start;

namespace prj_BIZ_System.Services
{
    public class UserService : _BaseService
    {

        //UserInfoModel******************************************************************************//

        public IList<UserInfoModel> GetUserInfoList()
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAll", null);
        }

        public IList<UserInfoModel> GetUserInfoListkw(string user_id, string company, string cluster_no)
        {
            if (user_id == "") user_id = null;
            if (company == "") company = null;
            if (cluster_no == "") cluster_no = null;

            if (cluster_no == null)
            {
                UserInfoModel tempModel = new UserInfoModel { user_id = user_id, company = company };
                return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAllkw", tempModel);
            }
            else
            {
                int id = int.Parse(cluster_no);
                UserInfoModel tempModel = new UserInfoModel { user_id = user_id, company = company, id = id };
                return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAllkwc", tempModel);
            }

        }

        public UserInfoModel GeUserInfoOne(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOne", user_id);
        }

        public UserInfoModel GeUserInfoOneManager(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOneManager", user_id);
        }


        public UserInfoModel GeUserInfoOneBySales(string sales_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOneBySales", sales_id);
        }

        public UserInfoModel ChkUserInfoOne(string user_id, string user_pw)
        {
            UserInfoModel tempModel = new UserInfoModel { user_id = user_id, user_pw = user_pw };
            return (UserInfoModel)mapper.QueryForObject("UserInfo.CheckOne", tempModel);
        }

        public HonestbeeModel ChkComplete(string user_id)
        {
            HonestbeeModel tempModel = new HonestbeeModel { user_id = user_id };
            return (HonestbeeModel)mapper.QueryForObject("UserInfo.CheckComplete", tempModel);
        }

        public object UserInfoInsertOne(UserInfoModel userInfoModel)
        {
            if (!string.IsNullOrEmpty(userInfoModel.user_pw))
            {
                userInfoModel.user_pw = SecurityHelper.Encrypt256(userInfoModel.user_pw);
            }
            var result = mapper.Insert("UserInfo.InsertOne", userInfoModel);
            return result;
        }

        public object UserInfoInsertOneManager(UserInfoModel userInfoModel)
        {
            if (!string.IsNullOrEmpty(userInfoModel.user_pw))
            {
                userInfoModel.user_pw = SecurityHelper.Encrypt256(userInfoModel.user_pw);
            }
            var result = mapper.Insert("UserInfo.InsertOneManager", userInfoModel);
            return result;
        }


        public object UserInfoInsertForMobile(UserInfoModel userInfoModel)
        {
            var result = mapper.Insert("UserInfo.InsertOne", userInfoModel);
            return result;
        }

        public int UserInfoUpdateOne(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOne", userInfoModel);
            return (int)obj;
        }

        public int UserInfoUpdateOneManager(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOneManager", userInfoModel);
            return (int)obj;
        }


        public int UserInfoUpdateOneForMobile(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOneForMobile", userInfoModel);
            return (int)obj;
        }

        public int UserInfoDelectOne(string user_id)
        {
            Object obj = mapper.Delete("UserInfo.DeleteOne", user_id);
            return (int)obj;
        }


        public bool UserInfoUpdateIdEnable(int id , string id_enable)
        {
            var param = new UserInfoModel() { id = id, id_enable = id_enable };
            return mapper.Update("UserInfo.UpdateIdEnable", param) > 0 ;
        }

        public bool UserInfoUpdateAddr(string user_id, string addr)
        {
            var param = new UserInfoModel() { user_id = user_id, addr = addr };
            return mapper.Update("UserInfo.UpdateAddr", param) > 0;
        }

        public bool UserInfoUpdateCapital(string user_id, long capital)
        {
            var param = new UserInfoModel() { user_id = user_id, capital = capital/1000 };
            return mapper.Update("UserInfo.UpdateCapital", param) > 0;
        }
        //EnterpriseSortListModel******************************************************************************//

        public IList<EnterpriseSortListModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortListModel>("UserInfo.SelectAll_sort", null);
        }

        public EnterpriseSortListModel GetSortById(int sort_id)
        {
            return (EnterpriseSortListModel)mapper.QueryForObject("UserInfo.SelectSortById", sort_id);
        }

        //CompanySortModel******************************************************************************//

        public IList<CompanySortModel> SelectUserSortBySortId(int sort_id,string kw)
        {
            CompanySortModel param = new CompanySortModel() { sort_id = sort_id , company =kw};
            return mapper.QueryForList<CompanySortModel>("UserInfo.SelectUserSortBySortId", param);
        }


        public IList<UserInfoModel> SelectUserKw(string kw)
        {
            UserInfoModel param = new UserInfoModel() {company = kw.ToUpper() };
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectUserKw", param);
        }

        public IList<UserInfoModel> SelectUserByProductName(string product_name)
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectUserByProductName", product_name.ToUpper());
        }

        public IList<UserInfoModel> SelectUserByCatalogName(string catalog_name)
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectUserByCatalogName", catalog_name.ToUpper());
        }


        //EnterpriseSortModel******************************************************************************//

        public IList<EnterpriseSortModel> SelectUserSortByUserId(string user_id)
        {
            EnterpriseSortModel param = new EnterpriseSortModel() { user_id = user_id };
            return mapper.QueryForList<EnterpriseSortModel>("UserInfo.SelectUserSortByUserId", param);
        }

        public bool RefreshUserSort(string user_id , int[] sort_ids)
        {
            EnterpriseSortModel param = new EnterpriseSortModel() { user_id = user_id };
            int deleteCount = mapper.Delete("UserInfo.DeleteUserSortByUserId", param);
            EnterpriseSortModel tempModel;
            if( sort_ids != null)
            {
                foreach ( int sort_id in sort_ids)
                {
                    tempModel = new EnterpriseSortModel(){ user_id = user_id, sort_id = sort_id };
                    mapper.Insert("UserInfo.InsertUserSortByUserId", tempModel);
                }
            }
            return true;
        }

        public Dictionary<string, object> UserInfoMultiInsert(string targetLocation)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<int, object> successUserInfos = new Dictionary<int, object>();
            Dictionary<int, object> failUserInfos = new Dictionary<int, object>();
            Dictionary<int, object> repeatUserInfos = new Dictionary<int, object>();
            Dictionary<string, string> tempRecord = new Dictionary<string, string>();
            Dictionary<string, object> statusUserInfos = new Dictionary<string, object>();
            List<List<object>>   allStatusUserInfos = new List<List<object>>();
            FileStream fs = null;
            HSSFWorkbook wb = null;
            HSSFSheet sheet = null;
            int colCount = 0;
            try
            {
                fs = new FileStream(targetLocation, FileMode.Open, FileAccess.Read);
                wb = new HSSFWorkbook(fs);
                sheet = (HSSFSheet)wb.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                colCount = headerRow.LastCellNum;
                UserInfoModel md = null;
                for(int r=1; r<= sheet.LastRowNum; r++)
                {
                    md = new UserInfoModel();
                    int d = 0 ;
                    headerRow = sheet.GetRow(r);
                    try
                    {
                        tempRecord = new Dictionary<string, string>();
                                                  //headerRow.GetCell(d++); //編號
                        tempRecord["user_id"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //帳號*(國內:請用統編；國外: 自訂)
                        tempRecord["user_pw"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //密碼*(8 - 12字，英數混合，不含特殊字元)
                        tempRecord["enterprise_type"] = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //企業類型*(0:國內企業；1:國外企業)
                        tempRecord["company"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //公司名稱*(中文)
                        tempRecord["company_en"]= headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //公司名稱(英文)
                        tempRecord["leader"]    = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //代表人(中文)
                        tempRecord["leader_en"] = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //代表人(英文)
                        tempRecord["addr"]      = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //地址(中文)
                        tempRecord["addr_en"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //地址(英文)
                        tempRecord["contact"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //主聯絡人
                        tempRecord["phone"]     = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //電話號碼*
                        tempRecord["email"]     = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //電子郵件*
                        //底下是測試用
                        //tempRecord["email"]     = headerRow.GetCell(d++) != null ? "fion@wavegis.com.tw":""; //電子郵件*
                        tempRecord["capital"]   = headerRow.GetCell(d++) != null && !string.IsNullOrEmpty(headerRow.GetCell(d-1).ToString()) ? headerRow.GetCell(d-1).ToString() : "0"; //資本額*(單位:千) 沒填的話給0
                        tempRecord["revenue"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //營業額*(1:500萬以下；2:501 - 1000萬；3:1501 - 3000萬；4:3001 - 5000萬；5:5000萬 - 1億；6:一億以上)
                        tempRecord["website"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //企業網址
                        tempRecord["info"]      = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //企業簡介(中文)
                        tempRecord["info_en"]   = headerRow.GetCell(d++) != null ? headerRow.GetCell(d-1).ToString():""; //企業簡介(英文)

                        

                        if (checkIsImportDataValidate(tempRecord, r))
                        {
                            failUserInfos.Add(r-1, tempRecord);
                            allStatusUserInfos.Add(new List<object>() {"fail", tempRecord });
                        }
                        else
                        {
                            object insertResult = null ;
                            try
                            {
                                md.user_id          = tempRecord["user_id"];
                                md.user_pw          = tempRecord["user_pw"];
                                md.enterprise_type  = tempRecord["enterprise_type"];
                                md.company          = tempRecord["company"];
                                md.company_en       = tempRecord["company_en"];
                                md.leader           = tempRecord["leader"];
                                md.leader_en        = tempRecord["leader_en"];
                                md.addr             = tempRecord["addr"];
                                md.addr_en          = tempRecord["addr_en"];
                                md.contact          = tempRecord["contact"];
                                md.phone            = tempRecord["phone"];
                                md.email            = tempRecord["email"].Trim();
                                md.capital          = Convert.ToInt64(tempRecord["capital"]);
                                md.revenue          = tempRecord["revenue"];
                                md.website          = tempRecord["website"];
                                md.info             = tempRecord["info"];
                                md.info_en          = tempRecord["info_en"];

                                md.id_enable        = "1";
                                insertResult = UserInfoInsertOne(md);
                                if (insertResult != null)
                                {
                                    successUserInfos.Add(r-1, tempRecord);
                                    allStatusUserInfos.Add(new List<object>() { "success", tempRecord });
                                }
                                else
                                {
                                    failUserInfos.Add(r-1, tempRecord);
                                    allStatusUserInfos.Add(new List<object>() { "fail", tempRecord });
                                }
                            }
                            catch (Exception ex1) //insert fail
                            {
                                string errMsg = ex1.ToString();
                                if(errMsg.IndexOf("23505", StringComparison.OrdinalIgnoreCase) > -1 ){
                                    repeatUserInfos.Add(r-1, tempRecord);
                                    allStatusUserInfos.Add(new List<object>() { "repeat", tempRecord });
                                }
                                else
                                {
                                    failUserInfos.Add(r-1, tempRecord);
                                    allStatusUserInfos.Add(new List<object>() { "fail", tempRecord });
                                }
                                logger.Error("No."+r+" rows is insert fail");
                                logger.Error(errMsg);
                            }
                        }
                    }
                    catch (Exception ex2) // null exception
                    {
                        string errMsg = ex2.ToString();
                        failUserInfos.Add(r-1, tempRecord);
                        allStatusUserInfos.Add(new List<object>() { "fail", tempRecord });
                    }

                }
                result.Add("success", successUserInfos);
                result.Add("fail"   , failUserInfos);
                result.Add("repeat" , repeatUserInfos);
                result.Add("allStatusUserInfos", allStatusUserInfos);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return result; 
        }

        private bool checkIsImportDataValidate(Dictionary<string,string> tempRecord,int r)
        {
            int capital = -1;
            bool result = false;

            if (string.IsNullOrEmpty(tempRecord["user_id"])) //帳號*(國內:請用統編；國外: 自訂)
            {
                logger.Warn("第{0}列資料的帳號名稱(user_id)={1}為空值!!", r + 1, tempRecord["user_id"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["user_pw"])) //密碼*(8 - 12字，英數混合，不含特殊字元)
            {
                logger.Warn("第{0}列資料的密碼(user_pw)={1}為空值!!", r + 1, tempRecord["user_pw"]);
                result = true;
            }
            else if (!MailHelper.IsPasswordOK(tempRecord["user_pw"])) //格式合法性
            {
                logger.Warn("第{0}列資料的密碼(user_pw)={1}格式不合法!!", r + 1, tempRecord["user_pw"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["enterprise_type"])) //企業類型*(0:國內企業；1:國外企業)
            {
                logger.Warn("第{0}列資料的企業類型(enterprise_type)={1}為空值!!", r + 1, tempRecord["enterprise_type"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["company"])) //公司名稱*(中文)
            {
                logger.Warn("第{0}列資料的公司名稱(company)={1}為空值!!", r + 1, tempRecord["company"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["phone"])) //電話號碼*
            {
                logger.Warn("第{0}列資料的電話號碼(phone)={1}為空值!!", r + 1, tempRecord["phone"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["email"])) //電子郵件*
            {
                logger.Warn("第{0}列資料的電子郵件(email)={1}為空值!!", r + 1, tempRecord["email"]);
                result = true;
            }
            else if (!MailHelper.checkMailValidate(tempRecord["email"].Trim())) //格式合法性
            {
                logger.Warn("第{0}列資料的電子郵件(email)={1}格式不合法!!", r + 1, tempRecord["email"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["revenue"])) //營業額*(1:500萬以下；2:501 - 1000萬；3:1501 - 3000萬；4:3001 - 5000萬；5:5000萬 - 1億；6:一億以上)
            {
                logger.Warn("第{0}列資料的營業額(revenue)={1}為空值!!", r + 1, tempRecord["revenue"]);
                result = true;
            }
            else if (string.IsNullOrEmpty(tempRecord["capital"])) // md.capital == -1
            {
                logger.Warn("第{0}列資料的資本額(capital)={1}為空值!!", r + 1, tempRecord["capital"]);
                result = true;
            }
            else if (!int.TryParse(tempRecord["capital"].Replace(",", ""), out capital))// md.capital == -1
            {
                logger.Warn("第{0}列資料的資本額(capital)={1}不是數值!!", r + 1, tempRecord["capital"]);
                result = true;
            }

            return result;
        }

        #region 產品說明
        /*顯示所有的產品*/
        public IList<ProductListModel> getAllProduct(string user_id)
        {
            ProductListModel param = new ProductListModel() { user_id  = user_id };
            return mapper.QueryForList<ProductListModel>("UserInfo.SelectProductListByUserId", param);
        }

        public IList<ProductListModel> getProductListByKw(string kw)
        {
            ProductListModel param = new ProductListModel() { product_name = kw.ToUpper() };
            return mapper.QueryForList<ProductListModel>("UserInfo.SelectProductListByKw", param);
        }

        public ProductListModel getProductOne(int? product_id)
        {
            ProductListModel param = new ProductListModel() { product_id = product_id };
            return mapper.QueryForObject<ProductListModel>("UserInfo.SelectProductListByProductId", param);
        }

        /*新增產品*/
        public object insertProductList(ProductListModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Insert("UserInfo.InsertProductList", param);
        }

        /*修改產品*/
        public int updateProductList(ProductListModel param)
        {
            //param.user_id = user_id;
            param.deleted = "1";
            return mapper.Update("UserInfo.UpdateProductList", param);
        }

        /*刪除產品*/
        public bool ProductListDelete(string user_id, int[] del_prods)
        {
            if (del_prods != null)
            {
                foreach (int del_prod in del_prods)
                {
                    var tempModel = new ProductListModel { user_id = user_id, product_id = del_prod };
                    mapper.Delete("UserInfo.DeleteProductListByProductId", tempModel);
                }
            }
            return true;
        }

        /*假刪除產品*/
        public bool ProductListDeleteFake(string user_id, int[] del_prods)
        {
            if (del_prods != null)
            {
                foreach (int del_prod in del_prods)
                {
                    var tempModel = new ProductListModel { user_id = user_id, product_id = del_prod };
                    mapper.Update("UserInfo.DeleteProductListByProductIdFake", tempModel);
                }
            }
            return true;
        }
        #endregion

        #region 型錄上傳
        public IList<CatalogListModel> getAllCatalog(string user_id)
        {
            CatalogListModel param = new CatalogListModel() { user_id = user_id };
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListByUserId", param);
        }

        public IList<CatalogListModel> getCatalogListByKw(string kw)
        {
            CatalogListModel param = new CatalogListModel() { catalog_name = kw.ToUpper() };
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListByKw", param);
        }


        public IList<CatalogListModel> getAllCatalogTop(int limit)
        {
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListTop", limit);
        }

        /*新增型錄*/
        public bool CatalogListInsert(string user_id , string catalog_name , string conver_fileName , string catalog_fileName)
        {
            CatalogListModel param = new CatalogListModel()
            {
                user_id = user_id,
                catalog_name = catalog_name,
                cover_file = conver_fileName,
                catalog_file = catalog_fileName,
                deleted = "1"
            };
            var obj = mapper.Insert("UserInfo.InsertCatalogList", param);
            return true;
        }

        /*查詢特定型錄*/
        public List<CatalogListModel> SelectCatalogListByCatalogNo(string user_id , int[] catalog_no)
        {
            if(catalog_no != null)
            {
                List<CatalogListModel> catalogNoList = new List<CatalogListModel>();
                foreach (var no in catalog_no)
                {
                    var tempModel = new CatalogListModel { user_id= user_id , catalog_no = no };
                    catalogNoList.Add(mapper.QueryForObject<CatalogListModel>("UserInfo.SelectCatalogListByCatalogNo", tempModel));
                }
                return catalogNoList;
            }
            else
            {
                return null;
            }
        }

        /* 刪除型錄 */
        public bool CatalogListsDelete(string user_id , int[] catalog_no)
        {
            if (catalog_no != null)
            {
                foreach (int no in catalog_no)
                {
                    var param = new CatalogListModel() { user_id = user_id, catalog_no = no };
                    mapper.Delete("UserInfo.DeleteCatalogListByCatalogNo", param);
                }
            }

            return true;
        }
        #endregion

        #region 影音型錄上傳
        public IList<VideoListModel> getAllVideo(string user_id)
        {
            VideoListModel param = new VideoListModel() { user_id = user_id };
            return mapper.QueryForList<VideoListModel>("UserInfo.SelectVideoListByUserId", param);
        }

        public IList<VideoListModel> getAllVideoTop(int limit)
        {
            return mapper.QueryForList<VideoListModel>("UserInfo.SelectVideoListTop", limit);
        }

        public IList<VideoListModel> getVideoListActive()
        {
            return mapper.QueryForList<VideoListModel>("UserInfo.SelectVideoListActive", null);
        }

        public IList<VideoListModel> getVideoListAll()
        {
            return mapper.QueryForList<VideoListModel>("UserInfo.SelectVideoListAll", null);
        }

        /*新增影音型錄*/
        public object VideoListInsert(string user_id, string video_name, string youtube_site)
        {
            VideoListModel param = new VideoListModel()
            {
                user_id = user_id,
                video_name = video_name,
                youtube_site = youtube_site
            };
            var obj = mapper.Insert("UserInfo.InsertVideoList", param);
            return obj;
        }

        /*查詢特定影音型錄*/
        public List<VideoListModel> SelectVideoListByVideoNo(string user_id, int[] video_no)
        {
            if (video_no != null)
            {
                List<VideoListModel> catalogNoList = new List<VideoListModel>();
                foreach (var no in video_no)
                {
                    var tempModel = new VideoListModel { user_id = user_id, video_no = no };
                    catalogNoList.Add(mapper.QueryForObject<VideoListModel>("UserInfo.SelectVideoListByVideoNo", tempModel));
                }
                return catalogNoList;
            }
            else
            {
                return null;
            }
        }

        /* 刪除影音型錄 */
        public object VideoListsDelete(string user_id, int[] video_no)
        {
            if (video_no != null)
            {
                foreach (int no in video_no)
                {
                    var param = new VideoListModel() { user_id = user_id, video_no = no };
                    var result = mapper.Delete("UserInfo.DeleteVideoListByVideoNo", param);
                    return result;
                }
            }

            return 0;
        }

        public bool ActiveVideo(int video_no)
        {
            mapper.Delete("UserInfo.DeleteActiveVideo", null);
            var param = new ActiveVideoModel() { video_no = video_no };
            mapper.Insert("UserInfo.InsertActiveVideo", param);
            return true;
        }

        public IList<ActiveVideoModel> SelectActiveVideo()
        {
            return mapper.QueryForList<ActiveVideoModel>("UserInfo.SelectActiveVideoAll", null);
        }
        #endregion
    }
}