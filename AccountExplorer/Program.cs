using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Net.Mail;
using System.Configuration;

namespace AccountExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new FirefoxDriver();
            
            XDocument xdoc = XDocument.Load("WebsiteData.xml");
            string EmailText = "<h1>Account Statuses for " + DateTime.Now.ToShortDateString() + "</h1><br />";

            IEnumerable<XElement> websiteInfo = from w in xdoc.Descendants("website")
                                                select w;

            foreach (XElement x in websiteInfo)
            {
                string _website = x.Attribute("id").Value.ToString();
                driver.Navigate().GoToUrl(_website);

                string _companyName = x.Element("companyName").Value.ToString();
                string _usernameElement = x.Element("usernameElement").Value.ToString();
                string _username = x.Element("username").Value.ToString();
                string _passwordElement = x.Element("passwordElement").Value.ToString();
                string _password = x.Element("password").Value.ToString();
                string _accountPageText = x.Element("accountPageText").Value.ToString();
                string _balanceType = x.Element("balanceType").Value.ToString();
                string _balanceElement = x.Element("balanceElement").Value.ToString();
                string _pinElement = x.Element("pinElement").Value.ToString();
                string _pin = x.Element("pin").Value.ToString();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until((d) => { return driver.FindElement(By.Id(_usernameElement)); });

                IWebElement username = driver.FindElement(By.Id(_usernameElement));
                username.SendKeys(_username);
                IWebElement password = driver.FindElement(By.Id(_passwordElement));
                password.SendKeys(_password);

                wait.Until((d) => { return d.Title.ToLower().Contains("account"); });

                if (_pin != "")
                {
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                    IWebElement pin = driver.FindElement(By.Id(_pinElement));
                    pin.SendKeys(_pin);
                }
                
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                string msg = "";
                switch (_balanceType)
                {
                    case "ClassName":
                        msg = _companyName + " Balance: " + driver.FindElement(By.ClassName(_balanceElement)).Text;
                        
                        break;
                    case "Id":
                        msg += _companyName + " Balance: " + driver.FindElement(By.Id(_balanceElement)).Text;
                        break;
                    default:
                        break;
                }

                System.Console.WriteLine(msg);
                EmailText += "<div>" + msg + "</div>";
            }
            driver.Quit();
            sendMail(EmailText);

        }

        static void sendMail(string EmailText)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(ConfigurationManager.AppSettings["emailAddress"].ToString());
            mail.From = new MailAddress("seutsey@gmail.com");
            mail.Subject = "Account Statuses for " + DateTime.Now.ToShortDateString();
            mail.Body = EmailText;
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 465;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["emailAddress"].ToString(), ConfigurationManager.AppSettings["emailPassword"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(mail);
        }
    }
}
