using UnityEngine;
using UnityEngine.Events;

public class KeyListener : MonoBehaviour
{
	public KeyCode key = KeyCode.Escape;
	public UnityEvent onKeyPressed;

	void Update()
	{
		if (Input.GetKeyDown(key))
		{
			OnKeyPressed();
		}
	}

	public void OnKeyPressed()
	{
		if (onKeyPressed != null)
		{
			onKeyPressed.Invoke();
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}