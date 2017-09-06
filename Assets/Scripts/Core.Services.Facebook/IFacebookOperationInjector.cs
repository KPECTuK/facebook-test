namespace Assets.Scripts.Core.Services.Facebook
{
	internal interface IFacebookOperationInjector
	{
		string[] OperationPermissions { set; }
		FacebookService Service { set; }
	}
}