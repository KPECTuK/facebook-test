namespace Assets.Scripts.Core.Services.Facebook
{
	public interface IFacebookOperation
	{
		string[] OperationPermissions { get; }
		bool IsComplete { get; }
		bool IsSuccess { get; }
		void Execute();
	}
}