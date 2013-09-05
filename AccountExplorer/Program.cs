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

namespace AccountExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new FirefoxDriver();
            XDocument xdoc = XDocument.Load("WebsiteData.xml");

            IEnumerable<XElement> websiteInfo = from w in xdoc.Descendants("website")
                                                select w;

            foreach (XElement x in websiteInfo)
            {
                string _website = x.Attribute("id").Value.ToString();
                driver.Navigate().GoToUrl(_website);

                switch (_website)
                {
                    case "http://att.com":
                        Att_Type(x, driver);
                        break;
                    default:
                        break;
                }

            }
            driver.Quit();

            Console.ReadKey();
        }

        static void Att_Type(XElement x, IWebDriver driver)
        {
            string _username = x.Element("username").Value.ToString();
            string _password = x.Element("password").Value.ToString();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until((d) => { return driver.FindElement(By.Id("userid")); });
                
            IWebElement phonenumber = driver.FindElement(By.Id("userid"));
            phonenumber.SendKeys(_username);
            IWebElement password = driver.FindElement(By.Id("userPassword"));
            password.SendKeys(_password);

            wait.Until((d) => { return d.Title.ToLower().StartsWith("account"); });

            System.Console.WriteLine("ATT Balance: " + driver.FindElement(By.ClassName("font30imp")).Text);
        }
    }
}
