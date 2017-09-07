using System.Linq;
using System.Text;
using Assets.Scripts.Core.Services.Facebook;
using Assets.Scripts.Core.Services.Facebook.Operaitons;
using Assets.Scripts.CoreServices.Social;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace

namespace Assets.Scripts.MonoBehaviours
{
	public class GameCore : MonoBehaviour
	{
		private FacebookService _facebookService;
		private SocialService _socialService;

		private Text _logText;

		private const int STRINGS_TOTAL_I = 20;

		public static GameCore instance { get; private set; }

		// ReSharper disable once UnusedMember.Local
		private void OnEnable()
		{
			instance = this;
			_logText = transform.GetComponentInChildren<Text>(true);
			//
			_socialService = new SocialService();
			_facebookService = new FacebookService();
			_facebookService.EnqueueOperation<LoginForReadOperation>();
			_facebookService.EnqueueOperation<ListFriendsOperation>();
			_facebookService.OnClientConnect();
		}

		public void Dump()
		{
			LogMessage(_socialService.GetForeigns().Aggregate(new StringBuilder(), (builder, _) => builder.AppendLine(_.Serialize())).ToString());
		}

		public void LogMessage(string message)
		{
			var log = _logText.text.Split('\n');
			var messageLog = message.Split('\n');
			var result = log.Concat(messageLog).Reverse().Take(STRINGS_TOTAL_I).Reverse().ToArray();
			_logText.text = result.Where(_ => !string.IsNullOrEmpty(_)).Aggregate(new StringBuilder(), (builder, _) => builder.AppendLine(_)).ToString();
		}

		public T Resolve<T>()
		{
			if(_socialService is T)
				return (T)(_socialService as object);
			if(_facebookService is T)
				return (T)(_facebookService as object);
			return default(T);
		}

		// ReSharper disable once UnusedMember.Local
		private void LateUpdate()
		{
			_facebookService.Dispatch();
		}
	}
}
