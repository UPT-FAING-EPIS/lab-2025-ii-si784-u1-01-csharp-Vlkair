using Bank.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bank.Domain.Tests
{
    [TestClass]
    public class BankAccountTests
    {
        [TestMethod]
        public void Debit_WithValidAmount_UpdatesBalance()
        {
            double beginningBalance = 11.99;
            double debitAmount = 4.55;
            double expected = 7.44;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Debit(debitAmount);

            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001);
        }

        [TestMethod]
        public void Credit_WithValidAmount_UpdatesBalance()
        {
            double beginningBalance = 11.99;
            double creditAmount = 5.01;
            double expected = 17.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Credit(creditAmount);

            double actual = account.Balance;
            Assert.AreEqual(expected, actual, 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Debit_WhenAmountIsMoreThanBalance_ShouldThrowArgumentOutOfRange()
        {
            double beginningBalance = 11.99;
            double debitAmount = 15.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Debit(debitAmount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Debit_WhenAmountIsNegative_ShouldThrowArgumentOutOfRange()
        {
            double beginningBalance = 11.99;
            double debitAmount = -5.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Debit(debitAmount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Credit_WhenAmountIsNegative_ShouldThrowArgumentOutOfRange()
        {
            double beginningBalance = 11.99;
            double creditAmount = -5.00;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Credit(creditAmount);
        }

        [TestMethod]
        public void Constructor_ShouldSetCustomerNameCorrectly()
        {
            string customerName = "Mr. Bryan Walton";
            double balance = 11.99;
            var account = new BankAccount(customerName, balance);

            Assert.AreEqual(customerName, account.CustomerName);
        }

        [TestMethod]
        public void Constructor_ShouldSetBalanceCorrectly()
        {
            string customerName = "Mr. Bryan Walton";
            double balance = 11.99;
            var account = new BankAccount(customerName, balance);

            Assert.AreEqual(balance, account.Balance, 0.001);
        }

        [TestMethod]
        public void Debit_WithZeroAmount_ShouldNotChangeBalance()
        {
            double beginningBalance = 11.99;
            double debitAmount = 0;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Debit(debitAmount);

            Assert.AreEqual(beginningBalance, account.Balance, 0.001);
        }

        [TestMethod]
        public void Credit_WithZeroAmount_ShouldNotChangeBalance()
        {
            double beginningBalance = 11.99;
            double creditAmount = 0;
            var account = new BankAccount("Mr. Bryan Walton", beginningBalance);

            account.Credit(creditAmount);

            Assert.AreEqual(beginningBalance, account.Balance, 0.001);
        }
    }
}
