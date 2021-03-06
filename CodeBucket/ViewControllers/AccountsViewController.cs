using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeBucket.Data;
using CodeFramework.Elements;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Data;
using System.Linq;
using System.Collections.Generic;

namespace CodeBucket.ViewControllers
{
	/// <summary>
	/// A list of the accounts that are currently listed with the application
	/// </summary>
    public class AccountsViewController : BaseAccountsController
	{
        protected override List<AccountElement> PopulateAccounts()
        {
            var accounts = new List<AccountElement>();   
            foreach (var account in Application.Accounts)
            {
                var thisAccount = account;
                var t = new AccountElement(thisAccount);
                t.Tapped += () => AccountSelected(thisAccount);

                //Check to see if this account is the active account. Application.Account could be null 
                //so make it the target of the equals, not the source.
                if (thisAccount.Equals(Application.Account))
                    t.Accessory = UITableViewCellAccessory.Checkmark;
                accounts.Add(t);
            }
            return accounts;
        }


        protected override void AddAccountClicked()
        {
            DoLoginStuff(null);
        }


        protected override void AccountDeleted(IAccount account)
        {
            //Remove the designated username
            var thisAccount = (Account)account;
            Application.Accounts.Remove(thisAccount);

            if (Application.Account != null && Application.Account.Equals(thisAccount))
            {
                NavigationItem.LeftBarButtonItem.Enabled = false;
                Application.Account = null;
            }
        }


        private void AccountSelected(Account account)
        {
            var a = account as Account;
            //If the account doesn't remember the password we need to prompt
            if (a.DontRemember)
            {
                DoLoginStuff(account.Username);
            }
            //Change the user!
            else
            {
                Utils.Login.LoginAccount(account.Username, a.Password, this, (ex) => {
                    DoLoginStuff(account.Username);
                });
            }
        }

        private void DoLoginStuff(string user)
        {
            var loginController = new LoginViewController();
            if (user != null)
                loginController.Username = user;
            loginController.Login = (username, password) => {
                Utils.Login.LoginAccount(username, password, loginController);
            };
            NavigationController.PushViewController(loginController, true);
        }
    }
}

