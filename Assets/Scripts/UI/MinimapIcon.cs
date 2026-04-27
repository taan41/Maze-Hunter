using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(SpriteRenderer), typeof(RotationConstraint))]
public class MinimapIcon : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public RotationConstraint rotationConstraint;

	void Start()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		if (rotationConstraint == null) rotationConstraint = GetComponent<RotationConstraint>();

		// Connect constrain to minimap camera
		ConstraintSource source = new()
		{
			sourceTransform = CameraManager.Instance.miminapCamera.transform,
			weight = 1
		};
		rotationConstraint.AddSource(source);
		rotationConstraint.constraintActive = true;
	}
}