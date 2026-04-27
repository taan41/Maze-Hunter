using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
	public bool IsOpen => gameObject.activeSelf;
	
	public virtual void Open()
	{
		gameObject.SetActive(true);
	}

	public virtual void Close()
	{
		gameObject.SetActive(false);
	}
}