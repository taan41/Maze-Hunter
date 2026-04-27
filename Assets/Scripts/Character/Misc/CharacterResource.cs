
using System;

[Serializable]
public class CharacterResource
{
	public event Action OnChanged;
	public event Action OnDepleted;
	public void InvokeOnChanged() => OnChanged?.Invoke();
	public void InvokeOnDepleted() => OnDepleted?.Invoke();

	public float max;
	public float current;
	public float normalized => max > 0 ? current / max : 0f;
	public bool isFull => current >= max;
	public bool isEmpty => current <= 0;

	public void Change(float amount)
	{
		current = Math.Clamp(current + amount, 0, max);
		if (current == 0)
		{
			InvokeOnDepleted();
		}
		InvokeOnChanged();
	}

	public void Reset(float newMax = 0)
	{
		if (newMax > 0)
		{
			max = newMax;
		}
		current = max;
		InvokeOnChanged();
	}
}