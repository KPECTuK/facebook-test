using System;
using Facebook.Unity;
using UnityEngine;

namespace Assets.Scripts.Core.Services
{
	public class FacebookService
	{
		private const string APP_ID_S = @"925989377540446";
		private const string CLIENT_TOCKEN_S = @"EAANKLrPqAV4BAOGNWm3ZCAz2IHK5qcqffpcjwkKUXQXqgRvCQ6BlyA3QPzoPCrrrpM7HlxtsrcgdA85ZBjWT2vuXuqpxc1oM1XgQP3z6xAKUEGfP5umRntJ6wfC70ZC3PYYc4PTWZBrdOdHJc5hQP3lUHPrI0uq8k29AxvExxdqPOR444MKrqkTdSl9se3n8T63XZAaGlzyTXRpH8aBWe";

		private static FacebookService _instance;
		//
		public readonly Action OnFocusLost = () => { };
		public readonly Action OnFocusCaptured = () => { };

		public static FacebookService Create()
		{
			if(!ReferenceEquals(null, _instance))
				return _instance;

			_instance = new FacebookService();
#if UNITY_EDITOR
			FB.Init(
				APP_ID_S,
				CLIENT_TOCKEN_S,
				true,
				true,
				true,
				false,
				true,
				null,
				"en_US",
				_instance.OnAppChangeState,
				_instance.OnInitilaized);
#else
			FB.Init(
				_instance.OnInitilaized,
				_instance.OnAppChangeState);

#endif
			return _instance;
		}

		private void OnInitilaized()
		{
			Debug.Log(">> initialized");
		}

		private void OnAppChangeState(bool isVisible)
		{
			Debug.Log(isVisible ? ">> shown" : ">> hidden");

			if(isVisible)
			{
				OnFocusCaptured();
			}
			else
			{
				OnFocusLost();
			}
		}
	}
}
