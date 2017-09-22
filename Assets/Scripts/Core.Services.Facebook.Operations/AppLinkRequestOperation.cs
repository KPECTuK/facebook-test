using System;
using Assets.Scripts.Extensions;
using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook.Operations
{
	public class AppLinkRequestOperation : FacebookOperationBase
	{
		protected GameCore Context { get { return GameCore.instance; } }

		public override void Execute()
		{
			if(!FB.IsInitialized)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> service is not initialized</color>");
				AssertResult(false);
				return;
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> request app link</color>");
			FB.GetAppLink(OnResponse);
		}

		private void OnResponse(IAppLinkResult result)
		{
			var isSuccess = FB.IsLoggedIn && !result.Cancelled && string.IsNullOrEmpty(result.Error);

			try
			{
				Context.Resolve<FacebookService>().AppLink = new Uri(result.Url);
			}
			catch(Exception exception)
			{
				Context.LogMessage(exception.ToText());
				Context.LogMessage("using default");
				Context.Resolve<FacebookService>().AppLink = new Uri(@"https://fb.me/1919816435007501");
				isSuccess = true;
			}

			Context.LogMessage(isSuccess
				? string.Format(new[]
				{
					"raw: " + result.RawResult,
					"app Ref: " + result.Ref,
					"app Url: " + result.Url,
					"app TargetUrl: " + result.TargetUrl,
					"link storred: " + Context.Resolve<FacebookService>().AppLink,
				}.ToText())
				: "<color=blue>[facebook-srv]:> user has failed app link request</color>");

			AssertResult(isSuccess);
		}
	}
}