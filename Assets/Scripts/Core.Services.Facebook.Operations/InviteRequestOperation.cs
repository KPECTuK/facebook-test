using System;
using Assets.Scripts.CoreServices.Social;
using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook.Operations
{
	public class InviteRequestOperation : FacebookOperationBase
	{
		protected GameCore Context { get { return GameCore.instance; } }

		public IForeignUser FacebookContact { private get; set; }

		public override void Execute()
		{
			if(!FB.IsInitialized)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> try invite message request: service is not initialized - passing by..</color>");
				AssertResult(false);
				return;
			}

			if(ReferenceEquals(null, FacebookContact))
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> try invite message request: contact is abset - passing by..</color>");
				AssertResult(false);
				return;
			}

			if(ReferenceEquals(null, Context.Resolve<FacebookService>().AppLink))
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> try invite message request: applink is not initialized - passing by..</color>");
				AssertResult(false);
				return;
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> try invite message request: executing</color>");

			FB.AppRequest(
				"game-invite",
				OGActionType.ASKFOR,
				FacebookContact.Id,
				null, //filters
				string.Empty, // data
				"game-invite-title",
				OnResponse);

			//FB.Mobile.AppInvite(
			//	url,
			//	null, // preview
			//	OnResponse);
		}

		private void OnResponse(IAppRequestResult result)
		{
			var isSuccess = FB.IsLoggedIn && !result.Cancelled && string.IsNullOrEmpty(result.Error);

			GameCore.instance.LogMessage(isSuccess
				? string.Format("<color=blue>[facebook-srv]:> invite has been sent successfully to: {0}</color>", FacebookContact.Name)
				: "<color=blue>[facebook-srv]:> user has failed to send invitation</color>");

			AssertResult(isSuccess);
		}
		//private void OnResponse(IAppInviteResult result)
		//{
		//	var isSuccess = FB.IsLoggedIn && !result.Cancelled && string.IsNullOrEmpty(result.Error);

		//	GameCore.instance.LogMessage(isSuccess
		//		? string.Format("<color=blue>[facebook-srv]:> invite has been sent successfully to: {0}</color>", FacebookContact)
		//		: "<color=blue>[facebook-srv]:> user has failed to send invitation</color>");

		//	AssertResult(isSuccess);
		//}

		//public void GetDeepLink ()
		//{
		//    FB.GetAppLink(DeepLinkCallBack);
		//}

		//public void DeepLinkCallBack(IAppLinkResult result)
		//{
		//    if (string.IsNullOrEmpty(result.Error))
		//    {
		//        IDictionary<string, object> dict = result.ResultDictionary;

		//        if (dict.ContainsKey("target_url"))
		//        {
		//            string url = dict["target_url"].ToString();
		//            string keyword = "request_ids=";
		//            int k = 0;
		//            while (k < url.Length - keyword.Length && !url.Substring(k, keyword.Length).Equals(keyword))
		//                k++;
		//            k += keyword.Length;
		//            int l = k;
		//            while (url[l] != '&' && url[l] != '%')
		//                l++;
		//            string id = url.Substring(k, l - k);

		//            FB.API("/" + id + "_" + AccessToken.CurrentAccessToken.UserId, HttpMethod.GET, DeepLinkCallBackCallBack);
		//        }
		//        else
		//        {
		//            Debug.Log("Applink Error :" + result.Error);
		//        }
		//    }
		//}

		//void DeepLinkCallBackCallBack(IGraphResult result)
		//{
		//    Debug.Log("Request callback");
		//    Debug.Log("========================");

		//    if (string.IsNullOrEmpty(result.Error))
		//    {
		//        IDictionary<string, object> dict = result.ResultDictionary;

		//        if (dict.ContainsKey("from"))
		//        {
		//            IDictionary<string,object> from = dict["from"] as Dictionary<string, object>;
		//            if (from.ContainsKey("name"))
		//            {
		//                Debug.Log(from["name"]);
		//                deepLinkUserName = from["name"].ToString();
		//            }
		//            if (from.ContainsKey("id"))
		//            {
		//                deepLinkUserName = from["name"].ToString();
		//                FB.API("/" + dict["id"], HttpMethod.DELETE, null);
		//            }
		//        }
		//    }
		//    else
		//    {
		//        Debug.Log("Error in request:" + result.Error);
		//    }
		//}
	}
}