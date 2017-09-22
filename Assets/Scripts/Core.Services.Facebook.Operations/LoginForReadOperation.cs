using System.Linq;
using Assets.Scripts.Extensions;
using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;
using UnityEngine;

namespace Assets.Scripts.Core.Services.Facebook.Operations
{
	public class LoginForReadOperation : FacebookOperationBase
	{
		protected GameCore Context { get { return GameCore.instance; } }

		public override void Execute()
		{
			if(!FB.IsInitialized)
			{
				Debug.Log("<color=blue>[facebook-srv]:> service is not initialized</color>");
				AssertResult(false);
				return;
			}

			if(FB.IsLoggedIn && AccessToken.CurrentAccessToken.Permissions.All(_ => !_.Equals(FacebookService.PERMISSION_PUBLISH_ACTIONS_S)))
			{
				Debug.Log("<color=blue>[facebook-srv]:> logged in already - bypassing..</color>");
				AssertResult(true);
				return;
			}

			Debug.Log("<color=blue>[facebook-srv]:> request login</color>");
			FB.LogInWithReadPermissions(Context.Resolve<FacebookService>().GetServiceRequiredPrivileges(), OnResponse);
		}

		private void OnResponse(ILoginResult result)
		{
			var isSuccess = FB.IsLoggedIn && !result.Cancelled && string.IsNullOrEmpty(result.Error);

			Debug.Log(isSuccess
				? AccessToken.CurrentAccessToken.Permissions.ToText("<color=blue>[facebook-srv]:> permissions for user: </color>" + AccessToken.CurrentAccessToken.UserId)
				: "<color=blue>[facebook-srv]:> user has failed login</color>");

			if(isSuccess)
				FB.ActivateApp();

			AssertResult(isSuccess);
		}
	}
}