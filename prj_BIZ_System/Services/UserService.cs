using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.Collections;

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

        //EnterpriseSortModel******************************************************************************//

        public IList<EnterpriseSortModel> GetSortList()
        {
            return mapper.QueryForList<EnterpriseSortModel>("UserInfo.SelectAll_sort", null);
        }


        //UserSortModel******************************************************************************//

        public IList<UserSortModel> SelectUserSortByUserId(string user_id)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            return mapper.QueryForList<UserSortModel>("UserInfo.SelectUserSortByUserId", param);
        }

        public bool RefreshUserSort(string user_id , int[] sort_ids)
        {
            UserSortModel param = new UserSortModel() { user_id = user_id };
            int deleteCount = mapper.Delete("UserInfo.DeleteUserSortByUserId", param);
            UserSortModel tempModel;
            if( sort_ids != null)
            {
                foreach ( int sort_id in sort_ids)
                {
                    tempModel = new UserSortModel(){ user_id = user_id, sort_id = sort_id };
                    mapper.Insert("UserInfo.InsertUserSortByUserId", tempModel);
                }
            }
            return true;
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