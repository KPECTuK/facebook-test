using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

namespace Assets.Scripts.Core.Services.Facebook
{
	public partial class FacebookService
	{
		private const string APP_ID_S = @"925989377540446";
		private const string CLIENT_TOCKEN_S = 
			@"EAAbIK0NarEYBAELlQHhskHj56mdHy2oPEFv0h6umE64xf8uhmJ4xL5BBQpmYBtHJNNkoZAukEVuhLcisq1TDsNChaxv4AUtZAz9P7nDFQY2psGYZBOEao22NUjrBmqBziZAMZB7gvhcPZAcs90RlAeultNxt5FUjakK1xI2DX1hTPEfUESklps9Aaws0nvrW4ZD";
		
		private static FacebookService _instance;
		// pause game here because the focus will be lost
		public readonly Action OnFocusLost = () => { };
		// resule game here because the focus will be obtained from authorization
		public readonly Action OnFocusCaptured = () => { };

		private readonly Queue<Type> _pendingOperationTypes = new Queue<Type>(); 
		private readonly Queue<IFacebookOperation> _operations = new Queue<IFacebookOperation>();
		private IFacebookOperation _current;
		private bool _isOperational;

		public void EnqueueOperation<TOperation>() where TOperation : FacebookOperationBase, new()
		{
			if(!_isOperational)
			{
				_pendingOperationTypes.Enqueue(typeof(TOperation));
				Debug.LogWarning("<color=blue>[facebook-srv]:> operation pending CONNECTED state: </color>" + typeof(TOperation).Name);
				return;
			}

			var operation = BuildOperation<TOperation>();
			if(ReferenceEquals(null, operation))
			{
				Debug.LogError("<color=blue>[facebook-srv]:> failed to build operation: </color>" + typeof(TOperation).Name);
				return;
			}

			_operations.Enqueue(operation);
		}

		// [Deprecated]
		public void EnqueueOperation<TOperation>(TOperation instance) where TOperation : FacebookOperationBase, new()
		{
			
		}

		public static FacebookService Create()
		{
			return ReferenceEquals(null, _instance) ? (_instance = new FacebookService()) : _instance;
		}

		public void OnClientConnect()
		{
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
				OnAppChangeState,
				OnInitilaized);
#else
			FB.Init(
				OnInitilaized,
				OnAppChangeState);
#endif
		}

		public void OnClientDisconnect()
		{
			_isOperational = false;
			_operations.Clear();
			_pendingOperationTypes.Clear();
			_current = null;
		}
		
		public void Dispatch()
		{
			if(!_isOperational)
				return;

			IFacebookOperation current;

			if(_pendingOperationTypes.Count > 0)
			{
				current = _current ?? BuildOperation(_pendingOperationTypes.Dequeue());
			}
			else
			{
				current = _current ?? (_operations.Count > 0 ? _operations.Peek() : null);
			}
			
			if(ReferenceEquals(null, current))
				return;

			if(ReferenceEquals(null, _current))
			{
				_current = current;
				_current.Execute();
				return;
			}

			_current = _current.IsComplete ? null : _current;
		}

		private void OnInitilaized()
		{
			Debug.Log("<color=blue>[facebook-srv]:> initialized</color>");
			_isOperational = true;
		}

		private void OnAppChangeState(bool isVisible)
		{
			Debug.Log(isVisible ? "<color=blue>[facebook-srv]:> resume from pause</color>" : "<color=blue>[facebook-srv]:> suspend to pause</color>");

			if(isVisible)
			{
				OnFocusCaptured();
				_isOperational = true;
			}
			else
			{
				OnFocusLost();
				_isOperational = false;
			}
		}
	}
}
