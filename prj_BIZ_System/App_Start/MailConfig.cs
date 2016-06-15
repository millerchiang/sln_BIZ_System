using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace prj_BIZ_System.App_Start
{
	public class MailConfig
	{
        public static string TemplateDir = "/MailTemplate/";

        public static string smtpServer = "smtp.gmail.com";
        public static int smtpPort = 587;
        public static string sendAccount = "mail@wavegis.com.tw";
        public static string password = "wavegis25983";
        public static MailSetting[] mailSetings; 

        public static void RegisterCustomSetting(string baseDir)
        {
            mailSetings = new MailSetting[10];

            //Email驗證
            MailSetting AccountMailValidate = new MailSetting { subject= "[BIZ MATCHMAKING SYSTEM]會員註冊驗證信", account = sendAccount, password= password, isEnableHtml=true};
            AccountMailValidate.template_file_name=new List<string>(new string[] { "AccountMailValidate.html" });
            AccountMailValidate.template_file_content = new List<string>();
            foreach (string file_name in AccountMailValidate.template_file_name)
            {
                AccountMailValidate.template_file_content.Add(File.ReadAllText(Path.Combine(baseDir + TemplateDir, file_name)));
            }
            mailSetings[(int)MailType.AccountMailValidate] = AccountMailValidate;

            //忘記密碼
            MailSetting ForgetPassword = new MailSetting { subject = "[BIZ MATCHMAKING SYSTEM]忘記密碼通知信", account = sendAccount, password = password, isEnableHtml = true };
            ForgetPassword.template_file_name = new List<string>(new string[] { "ForgetPassword.html" });
            ForgetPassword.template_file_content = new List<string>();
            foreach (string file_name in ForgetPassword.template_file_name)
            {
                ForgetPassword.template_file_content.Add(File.ReadAllText(Path.Combine(baseDir + TemplateDir, file_name)));
            }
            mailSetings[(int)MailType.ForgetPassword] = ForgetPassword;

            //報名審核
            MailSetting ActivityCheckNotify = new MailSetting { subject = "[BIZ MATCHMAKING SYSTEM]活動報名審核結果通知", account = sendAccount, password = password, isEnableHtml = true };
            ActivityCheckNotify.template_file_name = new List<string>(new string[] { "ActivityCheckNotify_Fail.html" , "ActivityCheckNotify_Success.html" });
            ActivityCheckNotify.template_file_content = new List<string>();
            foreach (string file_name in ActivityCheckNotify.template_file_name)
            {
                ActivityCheckNotify.template_file_content.Add(File.ReadAllText(Path.Combine(baseDir + TemplateDir, file_name)));
            }
            mailSetings[(int)MailType.ActivityCheckNotify] = ActivityCheckNotify;
        }
    }

    public class MailHelper
    {
        /// <summary>
        /// 填入會員Email認證內容 (會員帳號 , 驗證連結)
        /// </summary>
        public static Dictionary<string, string> fillAccountMailValidte(string user_id, string validate_link)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("(@會員帳號)", user_id);
            paramDict.Add("(@驗證連結)", validate_link);
            return paramDict;
        }

        /// <summary>
        /// 忘記密碼 (會員送出忘記密碼申請的時間 , 網站首頁 ,符合密碼規則之亂數密碼)
        /// </summary>
        public static Dictionary<string, string> fillForgetPassword(string apply_time , string page_index , string random_pw)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("(@會員送出忘記密碼申請的時間)", apply_time);
            paramDict.Add("(@網站首頁網址)", page_index);
            paramDict.Add("(@符合密碼規則之亂數密碼)", random_pw);
            return paramDict;
        }

        /// <summary>
        /// 活動報名審核結果通知( manager_check -> 後台審核 ("0"：不通過；"1"：通過)) ; 其他 -> model屬性值
        /// </summary>
        public static Dictionary<string, string> fillActivityCheckNotify(string manager_check,
            int activity_id, string activity_name, string starttime , string addr ,
            int quantity, string name, string phone, string email
            )
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("check"     , manager_check);
            paramDict.Add("(@活動編號)", activity_id.ToString());
            paramDict.Add("(@活動名稱)", activity_name);
            paramDict.Add("(@活動時間)", starttime);
            paramDict.Add("(@活動地點)", addr);
            paramDict.Add("(@與會人數)", quantity.ToString());
            paramDict.Add("(@主聯絡人)", name);
            paramDict.Add("(@手機號碼)", phone);
            paramDict.Add("(@聯絡信箱)", email);
            return paramDict;
        }
        /// <summary>
        /// 發送Email (收信者Email地址 , Email內容 , Email種類)
        /// </summary>
        public static void doSendMail(string to, Dictionary<string, string> param, MailType type)
        {
            if (!string.IsNullOrEmpty(to))
            {
                MailSetting[] mailSetings = MailConfig.mailSetings;

                int content_index = 0;
                if (param.ContainsKey("check"))
                {
                    content_index = int.Parse(param["check"]);
                }

                string content= mailSetings[(int)type].template_file_content[content_index];
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(mailSetings[(int)type].account);
                msg.IsBodyHtml = mailSetings[(int)type].isEnableHtml;
                foreach (KeyValuePair<string, string> kvp in param)
                {
                    content = content.Replace(kvp.Key, kvp.Value);
                }
                msg.Body = content;
                msg.Subject = mailSetings[(int)type].subject;
                msg.To.Add(new MailAddress(to.Trim()));

                SmtpClient client = new SmtpClient(MailConfig.smtpServer, MailConfig.smtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(mailSetings[(int)type].account, mailSetings[(int)type].password);
                client.Send(msg);
            }
        }

        /// <summary>
        /// 發送認證Email ( 流水號 , 使用者id , 使用者email , 主機位址 , 主機port號 )
        /// </summary>
        public static void sendAccountMailValidate(object id, string user_id , string email, string host, int port)
        {
            const string validateActionName = "AccountMailValidate";

            string link = id + "+" + user_id + "+" + DateTime.Now.ToString("yyyy-MM-dd");
            string validate_linkX = SecurityHelper.Encrypt(link);
            //檢查用
            string check_link = SecurityHelper.Decrypt(validate_linkX);

            var param = MailHelper.fillAccountMailValidte(user_id, "http://" + host + ":" + port.ToString() + "/User/" + validateActionName + "?validate_linkX=" + validate_linkX);
            if (!string.IsNullOrEmpty(email))
            {
                MailHelper.doSendMail(email, param, MailType.AccountMailValidate);
            }
        }

        /// <summary>
        /// 發送忘記密碼Email (使用者email )
        /// </summary>
        public static string sendForgetPassword(string email, string host, int port)
        {
            string apply_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string page_index = "http://" + host + ":" + port.ToString();
            string random_pw = "";
            while (!IsPasswordOK(random_pw))
            {
                random_pw = generateRandomPW();
            }

            var param = MailHelper.fillForgetPassword(apply_time , page_index ,  random_pw);
            if (!string.IsNullOrEmpty(email))
            {
                MailHelper.doSendMail(email, param, MailType.ForgetPassword);
            }
            return random_pw;
        }

        private static string generateRandomPW()
        {
            StringBuilder rand_pw = new StringBuilder();

            Random rd = new Random();
            int rand_pw_length = rd.Next(8,12+1);
            for ( int i =0; i < rand_pw_length; i++)
            {
                switch (rd.Next(2+1))
                {
                    case 0:
                        rand_pw.Append(Convert.ToChar(rd.Next(65, 90 + 1)));
                        break;
                    case 1:
                        rand_pw.Append(Convert.ToChar(rd.Next(48, 57 + 1)));
                        break;
                    case 2:
                        rand_pw.Append(Convert.ToChar(rd.Next(97, 122 + 1)));
                        break;
                }
            }
            
            return rand_pw.ToString();
        }

        public static bool IsPasswordOK(String InputString)
        {
            return (InputString != string.Empty && Regex.IsMatch(InputString, "^(?=.*[a-zA-Z])(?=.*\\d).{8,12}$"))
                ? true : false;
        }
    }

    /// <summary>
    /// 郵件寄發類型
    /// </summary>
    public enum MailType : int
    {
        /// <summary>
        /// 會員Email認證
        /// </summary>
        AccountMailValidate = 0,
        /// <summary>
        /// 忘記密碼
        /// </summary>
        ForgetPassword = 1,
        /// <summary>
        /// 活動報名審核結果通知
        /// </summary>
        ActivityCheckNotify = 2
    }

    public struct MailSetting
    {
        public string       subject;
        public string       from;
        public string       to;
        public List<string> template_file_name;
        public List<string> template_file_content;
        public bool         isEnableHtml;
        public string       account;
        public string       password;
    }

    public class SecurityHelper
    {
        public static string KeyDir = "/Key/";
        private static RSACryptoServiceProvider rsap;
        private static string pub;
        private static string pvt;

        public static void RegisterCustomSetting(string baseDir)
        {
            pub = File.ReadAllText(Path.Combine(baseDir + KeyDir, "AccountEmailValidate.pub"));
            pvt = File.ReadAllText(Path.Combine(baseDir + KeyDir, "AccountEmailValidate.pvt"));
            rsap = new RSACryptoServiceProvider(3072);
        }

        public static string Encrypt(string str)
        {
            string strX;
            byte[] cipher;
            rsap.FromXmlString(pub);
            cipher = rsap.Encrypt(Encoding.UTF8.GetBytes(str), false);
            strX = Convert.ToBase64String(cipher);
            return strX;
        }
        
        public static string Decrypt(string strX)
        {
            string str;
            byte[] cipher;
            rsap.FromXmlString(pvt);
            cipher = rsap.Decrypt(Convert.FromBase64String(strX.Replace(" ", "+")), false);
            str = Encoding.UTF8.GetString(cipher);
            return str;
        }
    }
}