<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Data;
using System.Net;
using System.Net.Mail;


public class Handler : IHttpHandler
{
    DBHelper db = new DBHelper("120.24.246.68", "basic", "000000", "cims_com");

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string str1 = context.Request.Form["Name"];
        string str2 = context.Request.Form["Email"];
        string str3 = context.Request.Form["Phone"];
        string str4 = context.Request.Form["Message"];
        if (getEmaillist().Rows.Count > 0)
        {
            sendemail(getEmaillist(), "15814080899@163.com", "yzy123456", "smtp.163.com", str1, str2, str3, str4, 465);
            savedata(str1, str2, str3, str4);
        }
        context.Response.Write("1");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">接收方邮件地址</param>
    /// <param name="sendaddress">发送方邮件地址</param>
    /// <param name="sendpassword">发送方邮件密码</param>
    /// <param name="smtpadd">发送方smtp服务器地址</param>
    /// <param name="workorderid"></param>
    /// <param name="vid"></param>
    /// <param name="procedureid"></param>
    /// <param name="smtpport"></param>
    private void sendemail(DataTable data, string sendaddress, string sendpassword, string smtpadd, string name, string msgEmail, string phone, string msg, int smtpport = 25)
    {
        try
        {
            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress(sendaddress);
            message.From = fromAddr;
            string addstr = "";
            if (data.Rows.Count > 0)
            {
                int index = 0;
                foreach (DataRow dr in data.Rows)
                {
                    if (dr["Email"] == null)
                    {
                        continue;
                    }
                    if (index == 0)
                    {
                        addstr += dr["EMail"].ToString();
                        index++;
                    }
                    else
                    {
                        addstr += "," + dr["EMail"].ToString();
                    }
                }
            }
            //设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add(addstr);
            //设置抄送人
            //message.CC.Add("1842947875@qq.com,471964854@qq.com");
            //设置邮件标题
            message.Subject = "留言邮件";
            //设置邮件内容
            //message.Body = "<h1><a id="+"Header1_HeaderTitle"+" class=" + "headermaintitle" + "href =" + "http://www.cnblogs.com/madyina/" + " ><b>石曼迪分享</b></a></h1>";
            string str;
            str = @"<!DOCTYPE html>
<html>
	<head>
		<title>留言板</title>
	</head>";

            str +="<body style='margin:0;padding:0;'><div style='width:100%;min-width:500px;overflow:hidden;box-sizing: border-box;'><p style='width:100%;height:50px;line-height:50px;font-size:20px;text-align:center;background:#286090;color:white;margin:0 0 20px 0;'>留言内容</p><form method='post'><div style='width:80%;padding:0 10%;margin-bottom:20px;'><p style='margin:0;margin-bottom:10px;'>姓名</p>";
            str += "<input type='text' autocomplete='off' style='width:100%;height:34px;border:1px solid #ccc;border-radius:4px;outline:none;text-indent: 15px;' value='" + name + "'></div>";//姓名

            str += "<div style='width:80%;padding:0 10%;margin-bottom:20px;'><p style='margin:0;margin-bottom:10px;'>邮箱</p>";
            str += "<input type='text' autocomplete='off' style='width:100%;height:34px;border:1px solid #ccc;border-radius:4px;outline:none;text-indent: 15px;'  value='" + msgEmail + "'></div>";//邮箱内容

            str += "<div style='width:80%;padding:0 10%;margin-bottom:20px;'><p style='margin:0;margin-bottom:10px;'>手机号</p>";
            str +=" <input type='text' autocomplete='off' style='width:100%;height:34px;border:1px solid #ccc;border-radius:4px;outline:none;text-indent: 15px;' value='" + phone + "'>";//手机号

            str += " </div><div style='width:80%;padding:0 10%;margin-bottom:20px;'><p style='margin:0;margin-bottom:10px;'>留言</p>";
            str += "<div contenteditable='true' style='width:100%;line-height:26px;border:1px solid #ccc;border-radius:4px;outline:none; padding:4px 15px;resize:none;box-sizing:border-box;'>" + msg + "</div></div></form></div>";//具体留言
            str +="</body></html>";


            string mailBody = str;

            //邮件内容
            message.Body = mailBody;
            message.IsBodyHtml = true;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
            SmtpClient client = new SmtpClient();
            //在这里我使用的是qq邮箱，所以是smtp.qq.com，如果你使用的是126邮箱，那么就是smtp.126.com。
            client.Host = "smtp.163.com";
            //使用安全加密连接。


            //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            client.Port = 25;
            //不和请求一块发送。
            client.UseDefaultCredentials = false;
            //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
            client.Credentials = new NetworkCredential(sendaddress,sendpassword);
            //发送
            client.Send(message);
            // client.Dispose();
        }
        catch(Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    private DataTable getEmaillist()
    {
        string sql = "SELECT * from emaillist";
        DataTable dt = new DataTable();
        dt = db.getTB(sql);
        return dt;
    }


    private void savedata(string name, string phone, string email, string message)
    {
        string sql = "insert into msg(Name,Phone,Email,Message) values('" + name + "','" + phone + "','" + email + "','" + message + "')";
        db.SendCommand(sql);
    }

}