using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.App_Start
{
    public class CustomConfig
    {
        public static string CatalogRootDir { get; set; }
        public static string CatalogRootPath { get; set; }

        /* 型錄封面圖讀取用的目錄路徑 */
        public static string CatalogCoverPath { get; set; }

        /* 型錄封面圖存檔用的目錄路徑 */
        public static string CatalogCoverDir { get; private set; }

        /* 型錄檔案的讀取用的目錄路徑 */
        public static string CatalogCatalogPath { get; set; }

        /* 型錄檔案的存檔用的目錄路徑 */
        public static string CatalogCatalogDir { get; private set; }

        public static void RegisterCustomSetting(string rootPath , string realRootDir)
        {
            CatalogRootPath = "/"+rootPath+"/"+"UploadRootDir/Catalog/";
            CatalogRootDir = Path.Combine(realRootDir, "UploadRootDir/Catalog/");

            #region 型錄顯示 和 存檔位置
            string subDirForCover = "cover_file/";
            string subDirForCatalog = "catalog_file/";
            CatalogCoverPath = CatalogRootPath + subDirForCover;
            CatalogCatalogPath = CatalogRootPath + subDirForCatalog;

            CatalogCoverDir = Path.Combine(CatalogRootDir, subDirForCover); //CatalogRootDir + subDirForCover; 
            CatalogCatalogDir = Path.Combine(CatalogRootDir, subDirForCatalog);  //CatalogRootDir + subDirForCatalog;
            #endregion
        }
    }
}