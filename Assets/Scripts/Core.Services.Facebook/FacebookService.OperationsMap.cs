using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Services.Facebook.Operaitons;
using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook
{
	public partial class FacebookService
	{
		private static readonly string[] _zeroLevelPermissions = { string.Empty };
		private readonly Dictionary<Type, string[]> _operationMap = new Dictionary<Type, string[]>
		{
			{ typeof(LoginForReadOperation), _zeroLevelPermissions },
			{ typeof(LoginForPublishOperation), _zeroLevelPermissions },
			{ typeof(ListFriendsOperation), new[] { "public_profile", "user_friends" } },
		};

		public IEnumerable<Type> GetOperationsAvailable()
		{
			return _operationMap.Keys;
		}

		public IEnumerable<string> GetServiceRequiredPriveleges()
		{
			return _operationMap
				.Values
				.SelectMany(_ => _)
				.Where(_ => !string.IsNullOrEmpty(_))
				.Distinct()
				.ToArray();
		}

		public IEnumerable<string> GetOpeartionsPrivelegies(IEnumerable<Type> types)
		{
			var result = new List<string>();
			foreach(var type in types)
			{
				string[] permission;
				if(_operationMap.TryGetValue(type, out permission))
					result.AddRange(permission.Where(_ => !string.IsNullOrEmpty(_)));
			}
			return result.Distinct().ToArray();
		}

		public bool IsAllowed<TOperation>() where TOperation : FacebookOperationBase
		{
			string[] required;
			if(!_operationMap.TryGetValue(typeof(TOperation), out required))
				return false;

			var currentAllowed = FB.IsInitialized && FB.IsLoggedIn ? AccessToken.CurrentAccessToken.Permissions.Concat(_zeroLevelPermissions) : _zeroLevelPermissions;
			return currentAllowed.Intersect(required).Count() == required.Length;
		}
	
		public TOperation BuildOperation<TOperation>() where TOperation : FacebookOperationBase, new()
		{
			return BuildOperation(typeof(TOperation)) as TOperation;
		}

		private IFacebookOperation BuildOperation(Type operaitonType)
		{
			if(!FB.IsInitialized)
				return null;

			string[] required;
			if(!_operationMap.TryGetValue(operaitonType, out required))
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> query is not in recognized: </color>" + operaitonType.Name);
				return null;
			}

			var currentAllowed = ReferenceEquals(null, AccessToken.CurrentAccessToken) 
				? _zeroLevelPermissions 
				: AccessToken.CurrentAccessToken.Permissions.Concat(_zeroLevelPermissions);
			if(currentAllowed.Intersect(required).Count() != required.Length)
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> insufficient permissions to build query: </color>" + operaitonType.Name);
				return null;
			}

			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> building query: </color>" + operaitonType.Name);

			var operation = Activator.CreateInstance(operaitonType);
			var injector = (IFacebookOperationInjector)operation;
			injector.OperationPermissions = required;
			injector.Service = this;

			return operation as IFacebookOperation;
		}
	}
}
