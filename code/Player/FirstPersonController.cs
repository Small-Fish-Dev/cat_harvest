public class FirstPersonController : Component
{
	[Property] public CharacterController CharacterController { get; set; }
	[Property] public GameObject Camera { get; set; }
	[Property] public float WalkSpeed { get; set; } = 120f; // How fast we walk
	[Property] public float RunSpeed { get; set; } = 250f; // How fast we run
	[Property] public float JumpStrength { get; set; } = 300f; // How high we jump

	public Angles EyeAngles { get; private set; }
	public Vector3 WishVelocity { get; private set; }

	protected override void OnEnabled() // Called as soon as the component gets enabled
	{
		base.OnEnabled();

		if ( Camera != null )
			EyeAngles = Camera.Transform.Rotation.Angles(); // Starting eye angles set to whatever the camera is
	}

	protected override void OnUpdate() // Called every frame
	{
		if ( Camera != null )
		{
			EyeAngles += Input.AnalogLook * 5f; // Rotate our view angles
			Camera.Transform.Rotation = EyeAngles.ToRotation(); // Set the camera's rotation based off of our eye angles
		}
	}

	protected override void OnFixedUpdate() // Called every tick
	{
		ComputeWishVelocity();

		if ( CharacterController != null )
		{
			ComputeHelper();
			ComputeJump();
		}
	}

	public void ComputeWishVelocity()
	{
		var playerYaw = Rotation.FromYaw( EyeAngles.yaw ); // Horizontal movement only needs yaw based off of where we're looking
		var direction = Input.AnalogMove * playerYaw; // Rotate your inputs based on your eye angles
		var wishSpeed = Input.Down( "Run" ) ? RunSpeed : WalkSpeed; // If we're running we use the running speed, else the walking speed

		WishVelocity = direction * wishSpeed; // The direction is normal, so we multiply its magnitude with the wish speed
	}

	public void ComputeHelper()
	{
		if ( CharacterController.IsOnGround ) // If we're touching the ground VVV
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 ); // Nullify any vertical velocity to stick to the ground
			CharacterController.Accelerate( WishVelocity ); // Accelerate by our wish velocity
			CharacterController.ApplyFriction( 4.0f ); // Make movements on ground responsive
		}
		else // If we're in air VVV
		{
			CharacterController.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta; // Apply the scene's gravity to the controller
			CharacterController.Accelerate( WishVelocity.ClampLength( WalkSpeed / 2f ) ); // Give some control in air but not too much
			CharacterController.ApplyFriction( 0.1f ); // Make movements in air slippery
		}

		CharacterController.Move(); // Move our character
	}

	public void ComputeJump()
	{
		if ( CharacterController.IsOnGround ) // If you're on the ground
			if ( Input.Down( "Jump" ) ) // And you're holding the JUMP button
				CharacterController.Punch( Vector3.Up * JumpStrength ); // Make the player jump (Punch unsticks you from the ground and applies velocity)
	}
}
