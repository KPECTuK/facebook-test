using System;
using System.Collections.Generic;
using Assets.Scripts.CoreServices.Social;
using Assets.Scripts.MonoBehaviours;
using Facebook.Unity;

namespace Assets.Scripts.Core.Services.Facebook
{
	public partial class FacebookService : IForeignService
	{
		public const string PERMISSION_PUBLISH_ACTIONS_S = @"publish_actions";

		private static FacebookService _instance;
		// pause game here because the focus will be lost
		public readonly Action OnFocusLost = () => { };
		// resule game here because the focus will be obtained from authorization
		public readonly Action OnFocusCaptured = () => { };

		private readonly Queue<Type> _pendingOperationTypes = new Queue<Type>();
		private readonly Queue<IFacebookOperation> _operations = new Queue<IFacebookOperation>();
		private IFacebookOperation _currentOperation;
		private bool _isOperational;
		
		public Uri AppLink { get; set; }

		public void EnqueueOperation<TOperation>() where TOperation : FacebookOperationBase, new()
		{
			if(!_isOperational)
			{
				_pendingOperationTypes.Enqueue(typeof(TOperation));
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> operation pending CONNECTED state: </color>" + typeof(TOperation).Name);
				return;
			}

			var operation = BuildOperation<TOperation>();
			if(ReferenceEquals(null, operation))
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> failed to build operation: </color>" + typeof(TOperation).Name);
				return;
			}

			_operations.Enqueue(operation);
		}

		// [Deprecated]
		public void EnqueueOperation<TOperation>(TOperation operation) where TOperation : FacebookOperationBase, new()
		{
			if(ReferenceEquals(null, operation))
			{
				GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> failed to build operation: </color>" + typeof(TOperation).Name);
				return;
			}
			_operations.Enqueue(operation);
		}

		public void OnClientConnect()
		{
			FB.Init(OnInitilaized, OnAppChangeState);
			//FB.Init(
			//	APP_ID_S,
			//	null,
			//	true,
			//	true,
			//	true,
			//	false,
			//	true,
			//	null,
			//	"en_US",
			//	OnAppChangeState,
			//	OnInitilaized);
		}

		public void OnClientDisconnect()
		{
			_isOperational = false;
			_operations.Clear();
			_pendingOperationTypes.Clear();
			_currentOperation = null;
		}

		public void Dispatch()
		{
			if(!_isOperational)
				return;

			var current = _currentOperation ?? (_operations.Count > 0 ? _operations.Dequeue() : null);
			current = current ?? (_pendingOperationTypes.Count > 0 ? BuildOperation(_pendingOperationTypes.Dequeue()) : null);

			if(ReferenceEquals(null, current))
				return;

			if(ReferenceEquals(null, _currentOperation))
			{
				_currentOperation = current;
				_currentOperation.Execute();
				return;
			}

			if(_currentOperation.IsComplete)
			{
				_currentOperation = null;
			}
		}

		private void OnInitilaized()
		{
			GameCore.instance.LogMessage("<color=blue>[facebook-srv]:> initialized</color>");
			_isOperational = true;
		}

		private void OnAppChangeState(bool isVisible)
		{
			GameCore.instance.LogMessage(isVisible ? "<color=blue>[facebook-srv]:> resume from pause</color>" : "<color=blue>[facebook-srv]:> suspend to pause</color>");

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
