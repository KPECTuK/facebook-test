using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Services.Facebook;

namespace Assets.Scripts.CoreServices.Social
{
	public interface IForeignService { }

	public interface IForeignUser : IEquatable<IForeignUser>
	{
		string Serialize();
	}

	public abstract class ForeignUserData<TService> : IForeignUser where TService : IForeignService
	{
		public abstract bool Equals(IForeignUser other);
		public abstract string Serialize();
	}

	public class FacebookUserData : ForeignUserData<FacebookService>
	{
		public string Id { get; private set; }
		public string Name { get; private set; }

		public FacebookUserData(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public override bool Equals(IForeignUser other)
		{
			return
				!ReferenceEquals(null, other) &&
				other is FacebookUserData &&
				Id.Equals(((FacebookUserData)other).Id);
		}

		public override string Serialize()
		{
			return string.Format(@"{{ ""id"": ""{0}"", ""name"": ""{1}"" }}", Id, Name);
		}

		public override bool Equals(object @object)
		{
			return Equals(@object as IForeignUser);
		}
	}

	public class CharatcerData { }

	public class SocialService
	{
		private readonly List<IForeignUser> _foreignContacts = new List<IForeignUser>();
		//
		private readonly List<CharatcerData> _ingameContacts = new List<CharatcerData>();
		private readonly List<CharatcerData> _ingameAvailables = new List<CharatcerData>();

		public void AddAvailable(CharatcerData data) { }

		public void AddContact(CharatcerData data) { }

		public void AddContact(IForeignUser data)
		{
			if(!_foreignContacts.Contains(data))
				_foreignContacts.Add(data);
		}

		public IEnumerable<IForeignUser> GetForeigns()
		{
			return _foreignContacts;
		}
	}
}
