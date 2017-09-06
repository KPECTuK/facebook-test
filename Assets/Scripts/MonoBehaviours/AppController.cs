using Assets.Scripts.Core.Services.Facebook;
using Assets.Scripts.Core.Services.Facebook.Operaitons;
using UnityEngine;

// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace Assets.Scripts.MonoBehaviours
{
	public class AppController : MonoBehaviour
	{
		private FacebookService _service;

		// ReSharper disable once UnusedMember.Local
		private void OnEnable()
		{
			_service = FacebookService.Create();
			_service.EnqueueOperation<LoginForReadOperation>();
			_service.OnClientConnect();
			_service.EnqueueOperation<ListFriendsOperation>();
		}

		// ReSharper disable once UnusedMember.Local
		private void LateUpdate()
		{
			_service.Dispatch();
		}
	}
}
