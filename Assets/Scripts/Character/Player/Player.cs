using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Interactor))]
public partial class Player : MonoBehaviour
{
	public static Player Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		animator = GetComponentInChildren<Animator>();
		controller = GetComponent<CharacterController>();
		interactor = GetComponent<Interactor>();

		AwakeAnimation();
	}

	void Start()
	{
		StartResources();
		StartControll();
		StartCamera();
		StartPhysics();
		StartAnimation();
		StartWeapon();
	}

	void Update()
	{
		float deltaTime = Time.deltaTime;

		UpdateResources(deltaTime);
		UpdateControll(deltaTime);
		UpdateCamera(deltaTime);
		UpdatePhysics(deltaTime);
		UpdateAnimation(deltaTime);
		UpdateWeapon(deltaTime);
	}
}