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

namespace AccountExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new FirefoxDriver();

            //SHA1 sha = new SHA1CryptoServiceProvider();
            //MD5 md5 = new MD5CryptoServiceProvider();

            //string hash = "a9xstCndTiLqqEiGgpcv1g==";

            //ToBase64Transform b64 = new ToBase64Transform();
            //byte[] pword = System.Convert.FromBase64String(hash);
            //string AttPassword = System.Text.Encoding.UTF8.GetString(pword);

            driver.Navigate().GoToUrl("https://www.att.com");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until((d) => { return driver.FindElement(By.Id("userid")); });

            IWebElement phonenumber = driver.FindElement(By.Id("userid"));
            phonenumber.SendKeys("724-708-2449");
            IWebElement password = driver.FindElement(By.Id("userPassword"));
            
            password.Submit();

            //wait.Until((d) => { return d.Title.ToLower().StartsWith("Account"); });
            System.Console.WriteLine("page title is: " + driver.Title);
            wait.Until((d) => { return d.Title.ToLower().StartsWith("account"); });

            System.Console.WriteLine("ATT Balance: " + driver.FindElement(By.ClassName("font30imp")).Text);

            driver.Quit();

            Console.ReadKey();
        }
    }
}
