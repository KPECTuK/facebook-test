using Facebook.Unity;
using UnityEngine;

namespace Assets.Scripts.Core.Services.Facebook.Operaitons
{
	public class LoginForReadOperation : FacebookOperationBase
	{
		public override void Execute()
		{
			if(!FB.IsInitialized)
			{
				Debug.LogError("<color=blue>[facebook-srv]:> service is not initialized</color>");
				return;
			}

			Debug.Log("<color=blue>[facebook-srv]:> request login</color>");
			FB.LogInWithReadPermissions(Service.GetServiceRequiredPriveleges(), OnResponse);
		}

		private void OnResponse(ILoginResult result)
		{
			AssertResult(!result.Cancelled && string.IsNullOrEmpty(result.Error));

			if(!FB.IsLoggedIn)
			{
				Debug.Log("<color=blue>[facebook-srv]:> user had canceled login</color>");
				return;
			}

			Debug.Log("<color=blue>[facebook-srv]:> user had logged in successfuly with permissions:</color>");
			foreach(var permission in AccessToken.CurrentAccessToken.Permissions)
			{
				Debug.Log(string.Format("<color=blue>[facebook-srv]:> {0} -> {1}</color>", AccessToken.CurrentAccessToken.UserId, permission));
			}
		}
	}
}