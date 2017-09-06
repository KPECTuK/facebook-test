namespace Assets.Scripts.Core.Services.Facebook
{
	public abstract class FacebookOperationBase : IFacebookOperation, IFacebookOperationInjector
	{
		public string[] OperationPermissions { get; private set; }
		string[] IFacebookOperationInjector.OperationPermissions { set { OperationPermissions = value; } }
		protected FacebookService Service { get; private set; }
		FacebookService IFacebookOperationInjector.Service { set { Service = value; } }

		public bool IsComplete { get; private set; }
		public bool IsSuccess { get; private set; }

		public abstract void Execute();

		protected void AssertResult(bool isSuccess)
		{
			IsSuccess = isSuccess;
			IsComplete = true;
		}
	}
}