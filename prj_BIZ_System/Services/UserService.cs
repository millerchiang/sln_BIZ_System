using prj_BIZ_System.Models;
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

namespace prj_BIZ_System.Services
{
    public class UserService : _BaseService
    {

        //UserInfoModel******************************************************************************//

        public IList<UserInfoModel> GetUserInfoList()
        {
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectAll", null);
        }

        public UserInfoModel GeUserInfoOne(string user_id)
        {
            return (UserInfoModel)mapper.QueryForObject("UserInfo.SelectOne", user_id);
        }

        public UserInfoModel ChkUserInfoOne(string user_id, string user_pw)
        {
            UserInfoModel tempModel = new UserInfoModel { user_id = user_id, user_pw = user_pw };
            return (UserInfoModel)mapper.QueryForObject("UserInfo.CheckOne", tempModel);
        }


        public object UserInfoInsertOne(UserInfoModel userInfoModel)
        {
            var result = mapper.Insert("UserInfo.InsertOne", userInfoModel);
            return result;
        }

        public int UserInfoUpdateOne(UserInfoModel userInfoModel)
        {
            Object obj = mapper.Update("UserInfo.UpdateOne", userInfoModel);
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


        //EnterpriseSortListModel******************************************************************************//

        public IList<EnterpriseSortListModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortListModel>("UserInfo.SelectAll_sort", null);
        }

        //CompanySortModel******************************************************************************//

        public IList<CompanySortModel> SelectUserSortBySortId(int sort_id,string kw)
        {
            CompanySortModel param = new CompanySortModel() { sort_id = sort_id , company =kw};
            return mapper.QueryForList<CompanySortModel>("UserInfo.SelectUserSortBySortId", param);
        }


        public IList<UserInfoModel> SelectUserKw(string kw)
        {
            UserInfoModel param = new UserInfoModel() {company = kw };
            return mapper.QueryForList<UserInfoModel>("UserInfo.SelectUserKw", param);
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
            Dictionary<string, object> successUserInfos = new Dictionary<string, object>();
            Dictionary<string, object> failUserInfos = new Dictionary<string, object>();
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
                for(int r=0; r<= sheet.LastRowNum; r++)
                {
                    md = new UserInfoModel();
                    int d = 0 ;
                    headerRow = sheet.GetRow(r);

                                          headerRow.GetCell(d++); //編號
                    md.user_id          = headerRow.GetCell(d++).ToString(); //帳號*(國內:請用統編；國外: 自訂)
                    md.enterprise_type  = headerRow.GetCell(d++).ToString(); //密碼*(8 - 12字，英數混合，不含特殊字元)
                    md.enterprise_type  = headerRow.GetCell(d++).ToString(); //企業類型*(0:國內企業；1:國外企業)
                    md.company          = headerRow.GetCell(d++).ToString(); //公司名稱*(中文)
                    md.enterprise_type  = headerRow.GetCell(d++).ToString(); //公司名稱(英文)
                    md.leader           = headerRow.GetCell(d++).ToString(); //代表人(中文)
                    md.leader_en        = headerRow.GetCell(d++).ToString(); //代表人(英文)
                    md.addr             = headerRow.GetCell(d++).ToString(); //地址
                    md.contact          = headerRow.GetCell(d++).ToString(); //主聯絡人
                    md.phone            = headerRow.GetCell(d++).ToString(); //電話號碼*
                    md.email            = headerRow.GetCell(d++).ToString(); //電子郵件*
                    md.capital          = headerRow.GetCell(d++)!=null? Convert.ToInt32(headerRow.GetCell(d++).ToString()) : -1; //資本額*(單位:萬)
                    md.revenue          = headerRow.GetCell(d++).ToString(); //營業額*(1:500萬以下；2:501 - 1000萬；3:1501 - 3000萬；4:3001 - 5000萬；5:5000萬 - 1億；6:一億以上)
                    md.website          = headerRow.GetCell(d++).ToString(); //企業網址
                    md.info             = headerRow.GetCell(d++).ToString(); //企業簡介(中文)
                    md.info_en          = headerRow.GetCell(d++).ToString(); //企業簡介(英文)

                    if(md.capital == -1)
                    {
                        failUserInfos.Add(md.user_id, md);
                    }
                    else
                    {
                        object insertResult = null ;
                        try
                        {
                            insertResult = UserInfoInsertOne(md);
                            if (insertResult != null)
                            {
                                successUserInfos.Add(md.user_id, md);
                            }
                            else
                            {
                                failUserInfos.Add(md.user_id, md);
                            }
                        }
                        catch (Exception ex1)
                        {
                            failUserInfos.Add(md.user_id, md);
                        }
                        finally
                        {

                        }

                    }

                }
                result.Add("success", successUserInfos);
                result.Add("fail"   , failUserInfos);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                Console.WriteLine(errMsg);
            }
            finally
            {
                fs.Close();
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

        /*新增並修改產品*/
        public bool ProductListRefresh(string user_id, List<ProductListModel> old_prods, List<ProductListModel> new_prods)
        {
            #region 修改
            if (old_prods!=null)
            {
                foreach(ProductListModel old_prod in old_prods)
                {
                    old_prod.user_id = user_id;
                    old_prod.deleted = "1";
                    var obj = mapper.Update("UserInfo.UpdateProductList", old_prod);
                }
            }
            #endregion

            #region 新增

            if (new_prods != null)
            {
                foreach (ProductListModel new_prod in new_prods)
                {
                    new_prod.user_id = user_id;
                    new_prod.deleted = "1";
                    var obj = mapper.Insert("UserInfo.InsertProductList", new_prod);
                }
            }
            #endregion
            return true;
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
        #endregion

        #region 型錄上傳
        public IList<CatalogListModel> getAllCatalog(string user_id)
        {
            CatalogListModel param = new CatalogListModel() { user_id = user_id };
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListByUserId", param);
        }

        public IList<CatalogListModel> getAllCatalogTop5()
        {
            return mapper.QueryForList<CatalogListModel>("UserInfo.SelectCatalogListTop5", null);
        }

        /*新增型錄*/
        public bool CatalogListInsert(string user_id , string conver_fileName , string catalog_fileName)
        {
            CatalogListModel param = new CatalogListModel()
            {
                user_id = user_id,
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
    }
}