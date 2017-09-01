using Assets.Scripts.Core.Services;
using UnityEngine;

// ReSharper disable once UnusedMember.Global
// ReSharper disable once CheckNamespace
public class AppController : MonoBehaviour
{
	private FacebookService _service;

	// ReSharper disable once UnusedMember.Local
	private void OnEnable()
	{
		_service = FacebookService.Create();
	}
}
