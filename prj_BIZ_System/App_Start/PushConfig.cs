using Newtonsoft.Json.Linq;
using NLog;
using prj_BIZ_System.Models;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BizTimer.Config
{
    class PushConfig
    {

        public static string gcm_senderId = "983053733461";
        public static string gcm_authToken = "AIzaSyAUBB8wDxauyZjOYs34UdKNLfm9aXqV9js";

        public static string apns_dev_certificate = "Key/IOS_certificate/Biz_develop.p12";
        public static string apns_dev_passwd = "1qaz2wsx"; //"1234567";

        public static void RegisterCustomSetting(string rootPath)
        {
            apns_dev_certificate = rootPath + apns_dev_certificate;
        }
    }

    public class PushHelper
    {
        public static void doPush(IList<MsgPushModel> MY_PUSH_List)
        {
            if (MY_PUSH_List!=null)
            {
                doPushForAndroid(MY_PUSH_List.Where<MsgPushModel>(item => "Android".Equals(item.device_os, StringComparison.CurrentCultureIgnoreCase)).ToList());
                doPushForIOS(MY_PUSH_List.Where<MsgPushModel>(item => "IOS".Equals(item.device_os, StringComparison.CurrentCultureIgnoreCase)).ToList());
            }
        }

        private static void doPushForAndroid(IList<MsgPushModel> MY_PUSH_List)
        {
            var gcmBroker = PushFactory.getGcmInstance();

            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine("GCM Notification Failed: ID="+gcmNotification.MessageId+", Desc="+description);
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine("GCM Notification Failed: ID="+succeededNotification.MessageId);
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine("GCM Notification Failed: ID="+n.MessageId+", Desc="+e); //e.Description 改成 e 
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine("Device RegistrationId Expired: "+oldId);

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine("Device RegistrationId Changed To: "+newId);
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine("GCM Rate Limited, don't send more until after "+retryException.RetryAfterUtc);
                    }
                    else
                    {
                        Console.WriteLine("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("GCM Notification Sent!");
            };

            // Start the broker
            gcmBroker.Start();

            foreach (MsgPushModel md in MY_PUSH_List)
            {
                // Queue a notification to send
                List<string> tempList = new List<string> { md.device_id };
                GcmNotification notification = new GcmNotification {
                    RegistrationIds = tempList,
                    Data = JObject.Parse("{\"msg_title\":\"" + md.msg_title + "\",\"msg_content\":\"" + md.msg_content + "\",\"reply_content\":\"" + md.reply_content + "\",\"msg_no\":\"" + md.msg_no + "\",\"msg_reply_no\":\"" + md.msg_reply_no + "\",\"company\":\"" + md.company + "\",\"company_en\":\"" + md.company_en + "\"}")
                };
                gcmBroker.QueueNotification(notification);
            }

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            gcmBroker.Stop();

        }

        private static void doPushForIOS(IList<MsgPushModel> MY_PUSH_List)
        {
            var apnsBroker = PushFactory.getApnsInstance();

            if (apnsBroker == null)
            {
                string errMsg = "憑證檔找不到";
                Console.WriteLine(errMsg);
                return ;
            }

            // Wire up events
            apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine("Apple Notification Failed: ID="+apnsNotification.Identifier+", Code="+statusCode);

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException           
                        Console.WriteLine("Apple Notification Failed for some unknown reason : "+ex.InnerException);
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnsBroker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("Apple Notification Sent!");
            };

            // Start the broker
            apnsBroker.Start();

            foreach (MsgPushModel md in MY_PUSH_List)
            {
                // Queue a notification to send
                apnsBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = md.device_id.Replace(" ", ""), //deviceToken
                    //Payload = JObject.Parse("{\"aps\":{\"badge\":1,\"content-available\":1 ,\"alert\":{\"title\":\"" + md.msg_content + "\",\"body\":\"" + md.company+" - "+md.reply_content + "\"}}}")
                    Payload = JObject.Parse("{\"aps\":{\"content-available\":1,\"sound\":\"default\","+"\"alert\":{\"title\":\"" + md.company + "\",\"body\":\"" + (md.msg_reply_no != 0 ? md.reply_content : md.msg_title) + "\"}"+",\"msg_type\":" + md.msg_type + ",\"msg_no\":" + md.msg_no + ",\"msg_reply_no\":" + md.msg_reply_no + "}}")
                });
            }
            apnsBroker.Stop();
        }
    }

    class PushFactory
    {
        private static GcmServiceBroker gcmBroker;
        private static ApnsServiceBroker apnsBroker;

        public static GcmServiceBroker getGcmInstance()
        {
            var config = new GcmConfiguration(PushConfig.gcm_senderId, PushConfig.gcm_authToken, null);
            gcmBroker = new GcmServiceBroker(config);
            return gcmBroker;
        }

        public static ApnsServiceBroker getApnsInstance()
        {
            if (File.Exists(PushConfig.apns_dev_certificate))
            {
                var config = new ApnsConfiguration(
                    ApnsConfiguration.ApnsServerEnvironment.Sandbox
                    //ApnsConfiguration.ApnsServerEnvironment.Production
                    , PushConfig.apns_dev_certificate
                    , PushConfig.apns_dev_passwd); //"push-cert.p12" "push-cert-pwd"
                apnsBroker = new ApnsServiceBroker(config);
            }
            return apnsBroker;
        }
    }
}
