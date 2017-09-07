using System.Collections.Generic;
using Assets.Scripts.CoreServices.Social;
using Assets.Scripts.MonoBehaviours;
using Facebook.MiniJSON;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook.Operaitons
{
	public class ListFriendsOperation : FacebookOperationBase
	{
		public override void Execute()
		{
			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> request friends</color>");

			FB.API("me/friends", HttpMethod.GET, OnResponse);
		}

		private void OnResponse(IGraphResult result)
		{
			AssertResult(!result.Cancelled && string.IsNullOrEmpty(result.Error));

			if(!IsSuccess)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> request friends error:\n</color>" + result.RawResult);
				return;
			}

			var deserialized = Json.Deserialize(result.RawResult) as IDictionary<string, object>;
			object contacts;
			if(deserialized != null && deserialized.TryGetValue("data", out contacts) && contacts is IEnumerable<object>)
			{
				var typedContacts = (IEnumerable<object>)contacts;
				var social = GameCore.instance.Resolve<SocialService>();
				foreach(IDictionary<string, object> typedContact in typedContacts)
				{
					if(typedContact == null)
						continue;

					string id;
					string name;

					if(!typedContact.TryGetValue("id", out id))
						continue;
					if(!typedContact.TryGetValue("name", out name))
						continue;

					social.AddContact(new FacebookUserData(id, name));
				}
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> request friends raw result:\n</color>" + result.RawResult);

			GameCore.instance.Dump();
		}
	}
}