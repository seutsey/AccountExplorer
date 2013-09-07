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
                string _dueOnElement = x.Element("dueOnElement").Value;
                string _dueOnText = x.Element("dueOnText").Value;
                string _dueOnType = x.Element("dueOnType").Value;

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                try
                {
                    IWebElement username = driver.FindElement(By.Id(_usernameElement));
                    username.SendKeys(_username);
                    IWebElement password = driver.FindElement(By.Id(_passwordElement));
                    password.SendKeys(_password);

                    wait.Until((d) => { return d.Title.ToLower().Contains("account"); });

                    if (_pin != "")
                    {
                        IWebElement pin = driver.FindElement(By.Id(_pinElement));
                        pin.SendKeys(_pin);
                    }
                
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                    string msg = "";
                    switch (_balanceType)
                    {
                        case "ClassName":
                            msg += _companyName + " Balance: " + driver.FindElement(By.ClassName(_balanceElement)).Text;
                            break;
                        case "Id":
                            msg += _companyName + " Balance: " + driver.FindElement(By.Id(_balanceElement)).Text;
                            break;
                        default:
                            break;
                    }

                    msg += " - " + dueDateGetter(_dueOnElement, _dueOnText, driver, _dueOnType);

                    System.Console.WriteLine(msg);
                    EmailText += "<div>" + msg + "</div>";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            driver.Quit();
            sendMail(EmailText);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();

        }

        private static string dueDateGetter(string _dueOnElement, string _dueOnText, IWebDriver driver, string _dueOnType)
        {
            string msg = "";
            switch (_dueOnType)
            {
                case "Class":
                    List<IWebElement> els = driver.FindElements(By.ClassName(_dueOnElement)).ToList<IWebElement>();
                    foreach (IWebElement el in els)
                    {
                        if (el.Text.Contains(_dueOnText))
                        {
                            msg +=  el.Text;
                            break;
                        }
                    }
                    break;
                case "Id":
                    msg += "Due On: " + driver.FindElement(By.Id(_dueOnElement)).Text;
                break;
            }   
            return msg;
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
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email - " + ex.Message);
            }
        }
    }
}
