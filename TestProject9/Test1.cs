using NUnit.Framework; // GARANTA QUE ESTA LINHA ESTÁ NO TOPO
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace ProjetoTesteFacebook
{
    public class FacebookTests
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://www.facebook.com/");

            try
            {
                IWebElement acceptCookiesButton = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//button[contains(., 'Permitir todos os cookies')] | //button[contains(., 'Permitir cookies essenciais e opcionais')]")));
                acceptCookiesButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Pop-up de cookies não encontrado ou já aceito."); //
            }
        }

        [Test, Order(1)]
        public void TesteLoginInvalido()
        {
            IWebElement emailField = driver.FindElement(By.Id("email"));
            emailField.SendKeys("teste_invalido@email.com");

            IWebElement passwordField = driver.FindElement(By.Id("pass"));
            passwordField.SendKeys("senhaInvalida123");

            IWebElement loginButton = driver.FindElement(By.Name("login"));
            loginButton.Click();

            IWebElement errorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("_9ay7")));

            NUnit.Framework.Assert.IsTrue(errorMessage.Text.Contains("A senha que você inseriu está incorreta") || errorMessage.Text.Contains("O email que você inseriu não está conectado a uma conta"), "A mensagem de erro do login não apareceu como esperado.");
            Console.WriteLine("Teste de login inválido passou! Mensagem de erro exibida corretamente.");
        }

        [Test, Order(2)]
        public void TesteFluxoCadastro()
        {
            IWebElement createAccountButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-testid='open-registration-form-button']")));
            createAccountButton.Click();

            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("firstname")));
            Console.WriteLine("Modal de cadastro aberto com sucesso.");

            driver.FindElement(By.Name("firstname")).SendKeys("NomeTeste");
            driver.FindElement(By.Name("lastname")).SendKeys("SobrenomeTeste");
            driver.FindElement(By.Name("reg_email__")).SendKeys("email.teste.aleatorio@host.com");
            driver.FindElement(By.Name("reg_passwd__")).SendKeys("SenhaForte@12345");

            SelectElement daySelect = new SelectElement(driver.FindElement(By.Id("day")));
            daySelect.SelectByValue("15");

            SelectElement monthSelect = new SelectElement(driver.FindElement(By.Id("month")));
            monthSelect.SelectByText("Mai");

            SelectElement yearSelect = new SelectElement(driver.FindElement(By.Id("year")));
            yearSelect.SelectByValue("1995");

            IWebElement genderRadioButton = driver.FindElement(By.XPath("//label[text()='Feminino']/following-sibling::input"));
            genderRadioButton.Click();

            IWebElement signUpButton = driver.FindElement(By.Name("websubmit"));

            // A CHAMADA COMPLETA "NUnit.Framework.Assert.IsTrue" EVITA O ERRO CS0117
            NUnit.Framework.Assert.IsTrue(signUpButton.Displayed && signUpButton.Enabled, "O botão Cadastre-se não está visível ou habilitado.");
            Console.WriteLine("Teste de preenchimento do cadastro passou! Campos preenchidos e botão de cadastro pronto.");
        }

        [TearDown]
        public void Teardown()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }
    }
}