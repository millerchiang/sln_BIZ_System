﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace prj_BIZ_System.App_Start
{
	public class MailConfig
	{
        public static string TemplateDir = "MailTemplate/";

        public static string smtpServer = "smtp.gmail.com";
        public static int smtpPort = 587;
        public static string sendAccount = "mail@wavegis.com.tw";
        public static string password = "wavegis25983";
        public static MailSetting[] mailSetings;
        public static string baseDir;

        public static void RegisterCustomSetting(string baseDir)
        {
            MailConfig.baseDir = baseDir;
            mailSetings = new MailSetting[100];
            AddMailSetting(MailType.AccountMailValidate, "[BIZ MATCHMAKING SYSTEM]會員註冊驗證信", new string[] { "AccountMailValidate.html" });
            AddMailSetting(MailType.ForgetPassword, "[BIZ MATCHMAKING SYSTEM]忘記密碼通知信", new string[] { "ForgetPassword.html" });
            AddMailSetting(MailType.ActivityCheckNotify, "[BIZ MATCHMAKING SYSTEM]活動報名審核結果通知", new string[] { "ActivityCheckNotify_Fail.html", "ActivityCheckNotify_Success.html" });
            AddMailSetting(MailType.ActivityAddBuyerNotify, "[BIZ MATCHMAKING SYSTEM]加入活動買主通知", new string[] { "ActivityAddBuyerNotify.html" });
        }

        private static void AddMailSetting(MailType mailType, string subject , string[] templates)
        {
            MailSetting mailSetting = new MailSetting { subject = subject , account = sendAccount, password = password, isEnableHtml = true };
            mailSetting.template_file_name = new List<string>(templates);
            mailSetting.template_file_content = new List<string>();
            foreach (string file_name in mailSetting.template_file_name)
            {
                mailSetting.template_file_content.Add(File.ReadAllText(Path.Combine(baseDir + TemplateDir, file_name)));
            }
            mailSetings[(int)mailType] = mailSetting;
        }
    }

    public class MailHelper
    {
        /// <summary>
        /// 填入會員Email認證內容 (會員帳號 , 驗證連結)
        /// </summary>
        private static Dictionary<string, string> fillAccountMailValidte(string user_id, string validate_link)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("(@會員帳號)", user_id);
            paramDict.Add("(@驗證連結)", validate_link);
            return paramDict;
        }

        /// <summary>
        /// 忘記密碼 (會員送出忘記密碼申請的時間 , 網站首頁 ,符合密碼規則之亂數密碼)
        /// </summary>
        private static Dictionary<string, string> fillForgetPassword(string apply_time , string page_index , string random_pw)
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
        private static Dictionary<string, string> fillActivityCheckNotify(
              string manager_check  ,int activity_id, string activity_name  , string starttime  , string endtime
            , string addr           ,int quantity   , string name           , string phone      , string email   )
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("check"     , manager_check);          //activity_register.manager_check (0或1)
            paramDict.Add("(@活動編號)", activity_id.ToString()); //activity_register.activity_id
            paramDict.Add("(@活動名稱)", activity_name);          //activity_info.activity_name
            paramDict.Add("(@活動時間)", starttime+" ~ "+endtime);//activity_info.starttime ~ activity_info.endtime
            paramDict.Add("(@活動地點)", addr);                   //activity_info.addr
            paramDict.Add("(@與會人數)", quantity.ToString());    //activity_register.quantity
            paramDict.Add("(@主聯絡人)", name);                   //activity_register.name_b
            paramDict.Add("(@手機號碼)", phone);                  //activity_register.telephone
            paramDict.Add("(@聯絡信箱)", email);                  //activity_register.email
            return paramDict;
        }

        /// <summary>
        /// 新增買主通知 (活動編號 ,活動名稱 ,活動開始時間 ,活動結束時間 , 活動地點 ,主辦單位)
        /// </summary>
        private static Dictionary<string, string> fillActivityAddBuyerNotify(
              int activity_id , string activity_name, DateTime starttime, DateTime endtime
            , string addr, string organizer)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("(@活動日期)", starttime.ToString("yyyy-MM-dd"));                //userInfo.email
            paramDict.Add("(@活動編號)", activity_id.ToString());   //activity_info.activity_id
            paramDict.Add("(@活動名稱)", activity_name);            //activity_info.activity_name
            paramDict.Add("(@活動時間)", starttime.ToString("yyyy-MM-dd HH:mm") + " ~ " + endtime.ToString("yyyy-MM-dd HH:mm"));//activity_info.starttime ~ activity_info.endtime
            paramDict.Add("(@活動地點)", addr);                   //activity_info.addr
            paramDict.Add("(@主辦單位)", organizer);                //activity_info.organizer
            return paramDict;
        }

        /// <summary>
        /// 發送Email (收信者Email地址 , Email內容 , Email種類)
        /// </summary>
        public static string doSendMail(string to, Dictionary<string, string> param, MailType type)
        {
            string errorInfo = "";
            if (checkEmail(to, out errorInfo)==200 && !string.IsNullOrEmpty(to))
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
            return errorInfo;
        }

        /// <summary>
        /// 發送認證Email ( 流水號 , 使用者id , 使用者email , 主機位址 , 主機port號 )
        /// </summary>
        public static void sendAccountMailValidate(object id, string user_id , string email)
        {
            //const string validateActionName = "AccountMailValidate";
            string validateActionName = VirtualPathUtility.ToAbsolute("~/User/AccountMailValidate");

            string ip = HttpContext.Current.Request.Url.Host;
            int port = HttpContext.Current.Request.Url.Port;
            string link = id + "+" + user_id + "+" + DateTime.Now.ToString("yyyy-MM-dd");
            string validate_linkX = SecurityHelper.Encrypt(link);
            //檢查用
            string check_link = SecurityHelper.Decrypt(validate_linkX);
            string full_host = HttpContext.Current.Request.IsSecureConnection ? "https://" + ip : "http://" + ip + ":" + port.ToString();
            var param = MailHelper.fillAccountMailValidte(user_id, full_host + validateActionName + "?validate_linkX=" + validate_linkX);
            if (!string.IsNullOrEmpty(email))
            {
                MailHelper.doSendMail(email, param, MailType.AccountMailValidate);
            }
        }

        /// <summary>
        /// 發送忘記密碼Email (使用者email )
        /// </summary>
        public static string sendForgetPassword(string email)
        {
            string ip = HttpContext.Current.Request.Url.Host;
            int port = HttpContext.Current.Request.Url.Port;
            string apply_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string page_index = "http://" + ip + ":" + port.ToString();
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

        /// <summary>
        /// 發送活動報名審核結果通知( manager_check -> 後台審核 ("0"：不通過；"1"：通過)) ; 其他 -> model屬性值
        /// </summary>
        public static void sendActivityCheckNotify( 
              string manager_check  , int activity_id , string activity_name  , string starttime    , string endtime
            , string addr           , int quantity    , string name           , string phone        , string email  )
        {
            if (!string.IsNullOrEmpty(email))
            {
                Dictionary<string,string> param = MailHelper.fillActivityCheckNotify( 
                      manager_check ,activity_id, activity_name , starttime , endtime
                    , addr          ,quantity   , name          , phone     , email );
                if (!string.IsNullOrEmpty(email))
                {
                    MailHelper.doSendMail(email, param, MailType.ActivityCheckNotify);
                }
            }
        }

        /// <summary>
        /// 發送新增買主通知 (使用者email , 活動編號 ,活動名稱 ,活動開始時間 ,活動結束時間 , 活動地點 ,主辦單位)
        /// </summary>
        public static void sendActivityAddBuyerNotify(
            string email , int activity_id, string activity_name, DateTime starttime
            , DateTime endtime , string addr, string organizer)
        {
            Dictionary<string, string> param = MailHelper.fillActivityAddBuyerNotify(
              activity_id, activity_name, starttime, endtime, addr, organizer);
            if (!string.IsNullOrEmpty(email))
            {
                MailHelper.doSendMail(email, param, MailType.ActivityAddBuyerNotify);
            }
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

        /// <summary>
        /// 檢查Mail伺服器 (收信者Email地址 , out string 錯誤訊息)
        /// </summary>
        public static int checkEmail(string mailAddress, out string errorInfo)
        {
            Regex reg = new Regex("^[_a-zA-Z0-9-]+([.][_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+([.][a-zA-Z0-9-]+)*$");
            if (!reg.IsMatch(mailAddress))
            {
                errorInfo = "Email Format error!";
                return 405;
            }

            string mailServer = getMailServer(mailAddress);
            if (mailServer == null)

            {
                errorInfo = "Email Server error!";
                return 404;
            }
            TcpClient tcpc = new TcpClient();
            tcpc.NoDelay = true;
            tcpc.ReceiveTimeout = 3000;
            tcpc.SendTimeout = 3000;

            try
            {
                tcpc.Connect(mailServer, 25);
                NetworkStream s = tcpc.GetStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                StreamWriter sw = new StreamWriter(s, Encoding.Default);
                string strResponse = "";
                string strTestFrom = mailAddress;
                sw.WriteLine("helo " + mailServer);
                sw.WriteLine("mail from:<" + mailAddress + ">");
                sw.WriteLine("rcpt to:<" + strTestFrom + ">");
                strResponse = sr.ReadLine();

                if (!strResponse.StartsWith("2"))
                {
                    errorInfo = "UserName error!";
                    return 403;
                }

                sw.WriteLine("quit");
                errorInfo = String.Empty;
                return 200;
            }
            catch (Exception ee)
            {
                errorInfo = ee.Message.ToString();
                return 403;
            }

        }

        /// <summary>
        /// 檢查Mail格式 (收信者Email地址)
        /// </summary>
        public static bool checkMailValidate(string mailAddress)
        {
            return (!string.IsNullOrEmpty(mailAddress)) && new Regex("^[_a-zA-Z0-9-]+([.][_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+([.][a-zA-Z0-9-]+)*$").IsMatch(mailAddress);
        }

        private static string getMailServer(string strEmail)
        {
            string strDomain = strEmail.Split('@')[1];
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.FileName = "nslookup";
            info.CreateNoWindow = true;
            info.Arguments = "-type=mx " + strDomain;
            Process ns = Process.Start(info);
            StreamReader sout = ns.StandardOutput;

            Regex reg = new Regex("mail exchanger = (?<mailServer>[^\\s].*)");
            string strResponse = "";
            while ((strResponse = sout.ReadLine()) != null)
            {
                Match amatch = reg.Match(strResponse);
                if (reg.Match(strResponse).Success) return amatch.Groups["mailServer"].Value;
            }
            return null;
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
        ActivityCheckNotify = 2,
        /// <summary>
        /// 新增買主通知
        /// </summary>
        ActivityAddBuyerNotify = 3
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
        public static string KeyDir = "Key/";
        private static RSACryptoServiceProvider rsap;
        private static SHA256 sha256;
        private static string pub;
        private static string pvt;

        public static void RegisterCustomSetting(string baseDir)
        {
            pub = File.ReadAllText(Path.Combine(baseDir + KeyDir, "AccountEmailValidate.pub"));
            pvt = File.ReadAllText(Path.Combine(baseDir + KeyDir, "AccountEmailValidate.pvt"));
            rsap = new RSACryptoServiceProvider(3072);
            sha256 = new SHA256CryptoServiceProvider();
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

        public static string Encrypt256(string str) {
            byte[] bytes = sha256.ComputeHash(Encoding.Default.GetBytes(str));
            string hexString;
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString("X2"));
            }
            hexString = strB.ToString();
            return hexString;
        }
    }
}