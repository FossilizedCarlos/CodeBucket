using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Linq;
using System.Threading;
using BitbucketSharp;
using System.Collections.Generic;


namespace BitbucketBrowser.UI
{
	public abstract class FollowersController : Controller<IList<FollowerModel>>
    {
		protected FollowersController()
			: base(true)
		{
            Style = UITableViewStyle.Plain;
		}

        protected override void OnRefresh()
        {
            BeginInvokeOnMainThread(delegate {
                var sec = new Section();
                foreach (var s in Model) 
                {
                    var realName = s.FirstName ?? "" + " " + s.LastName ?? "";
                    StyledStringElement sse;
                    if (!string.IsNullOrWhiteSpace(realName))
                        sse = new StyledStringElement(s.Username, realName, UITableViewCellStyle.Subtitle);
                    else
                        sse = new StyledStringElement(s.Username);
                    sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                    sse.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    sec.Add(sse);
                }
                Root.Add(sec);
            });
        }
	}
	
	public class UserFollowersController : FollowersController
	{
		private readonly string _name;

		public UserFollowersController(string name)
		{
			_name = name;
		}

        protected override IList<FollowerModel> OnUpdate()
        {
            var client = new Client("thedillonb", "djames");
            return client.Users[_name].GetFollowers().Followers;
        }
	}
	
	public class RepoFollowersController : FollowersController
	{
		private readonly string _name;
		private readonly string _owner;

		public RepoFollowersController(string owner, string name)
		{
			_name = name;
			_owner = owner;
		}

        protected override IList<FollowerModel> OnUpdate()
        {
            var client = new Client("thedillonb", "djames");
            return client.Users[_owner].Repositories[_name].GetFollowers().Followers;
        }
	}
}

