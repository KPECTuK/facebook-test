using Facebook.Unity;
using UnityEngine;

namespace Assets.Scripts.Core.Services.Facebook.Operaitons
{
	public class ListFriendsOperation : FacebookOperationBase
	{
		public override void Execute()
		{
			Debug.Log("<color=blue>[facebook-srv]:> request friends</color>");
			FB.API("/{friend-list-id}", HttpMethod.GET, OnResponse, new WWWForm());
		}

		private void OnResponse(IGraphResult result)
		{
			AssertResult(!result.Cancelled && string.IsNullOrEmpty(result.Error));

			Debug.Log("<color=blue>[facebook-srv]:> request friends error:\n</color>" + result.Error);
		}
	}
}