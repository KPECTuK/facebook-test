using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook.Operaitons
{
	public class LoginForReadOperation : FacebookOperationBase
	{
		public override void Execute()
		{
			if(!FB.IsInitialized)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> service is not initialized</color>");
				return;
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> request login</color>");
			FB.LogInWithReadPermissions(Service.GetServiceRequiredPriveleges(), OnResponse);
		}

		private void OnResponse(ILoginResult result)
		{
			AssertResult(!result.Cancelled && string.IsNullOrEmpty(result.Error));

			if(!FB.IsLoggedIn)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> user had canceled login</color>");
				return;
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> user had logged in successfuly with permissions:</color>");
			foreach(var permission in AccessToken.CurrentAccessToken.Permissions)
			{
				GameCore.instance.LogMessage(string.Format("<color=blue>[facebook-srv]:> {0} -> {1}</color>", AccessToken.CurrentAccessToken.UserId, permission));
			}
		}
	}
}