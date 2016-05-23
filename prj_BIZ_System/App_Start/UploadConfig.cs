using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.App_Start
{
    public class UploadConfig
    {
        /// <summary>
        /// 上傳檔案根目錄實體檔位置
        /// </summary>
        public static string CatalogRootDir { get; set; }

        /// <summary>
        /// 上傳檔案根目錄路徑
        /// </summary>
        public static string CatalogRootPath { get; set; }

        /// <summary>
        /// 型錄封面圖目錄路徑
        /// </summary>
        public static string subDirForCover { get; set; }

        /// <summary>
        /// 型錄檔案目錄路徑
        /// </summary>
        public static string subDirForCatalog { get; set; }

        /// <summary>
        /// Logo圖片儲存路徑
        /// </summary>
        public static string subDirForLogo { get; set; }

        /// <summary>
        /// 新聞訊息圖片儲存路徑
        /// </summary>
        public static string subDirForNews { get; set; }

        public static void RegisterCustomSetting(string rootPath, string realRootDir)
        {
            CatalogRootPath = "/" + rootPath + "/" + "UploadRootDir/";
            CatalogRootDir = Path.Combine(realRootDir, "UploadRootDir/");

            #region 型錄顯示 和 存檔位置
            subDirForCover = "Catalog/cover_file/";
            subDirForCatalog = "Catalog/catalog_file/";
            subDirForLogo = "Logo/";
            subDirForNews = "News/";
            #endregion
        }
    }


    public class UploadHelper
    {
        public static void doUploadFile(HttpPostedFileBase uploadFile, string subFileDir, string user_id)
        {
            string targetRootDir = Path.Combine(UploadConfig.CatalogRootDir, user_id);
            string targetFilePath = "";
            targetFilePath = Path.Combine(targetRootDir, subFileDir);
            if (!Directory.Exists(UploadConfig.CatalogRootDir))
            {
                Directory.CreateDirectory(UploadConfig.CatalogRootDir);
            }
            if (!Directory.Exists(targetRootDir))
            {
                Directory.CreateDirectory(targetRootDir);
            }
            if (!Directory.Exists(targetFilePath))
            {
                Directory.CreateDirectory(targetFilePath);
            }
            string targetCoverFilePath = Path.Combine(targetFilePath, uploadFile.FileName);
            uploadFile.SaveAs(targetCoverFilePath);
        }
    }
}