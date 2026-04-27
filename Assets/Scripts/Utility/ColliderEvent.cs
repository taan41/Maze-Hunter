using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderEvent : MonoBehaviour
{
	public event Action<Collision> OnCollisionEnterEvent;
	public event Action<Collision> OnCollisionExitEvent;
	public event Action<Collider> OnTriggerEnterEvent;
	public event Action<Collider> OnTriggerExitEvent;

	[SerializeField] Collider _collider;
	[SerializeField] bool isTrigger;
	[SerializeField] bool startEnabled = false;

	void Awake()
	{
		if (_collider == null) _collider = GetComponent<Collider>();
		_collider.isTrigger = isTrigger;
		_collider.enabled = startEnabled;
	}

	void OnCollisionEnter(Collision collision)
	{
		OnCollisionEnterEvent?.Invoke(collision);
	}

	void OnCollisionExit(Collision collision)
	{
		OnCollisionExitEvent?.Invoke(collision);
	}

	void OnTriggerEnter(Collider other)
	{
		OnTriggerEnterEvent?.Invoke(other);
	}

	void OnTriggerExit(Collider other)
	{
		OnTriggerExitEvent?.Invoke(other);
	}

	public void ToggleCollider(bool enabled)
	{
		_collider.enabled = enabled;
	}
}