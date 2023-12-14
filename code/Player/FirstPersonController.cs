using Sandbox;
using System.Drawing;
using System.Linq;
using System.Runtime;

public class FirstPersonController : Component
{
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Camera { get; set; }
	[Property] public float WalkSpeed { get; set; } = 200f;
	[Property] public float RunSpeed { get; set; } = 400f;

	public Angles EyeAngles;
	public Rotation EyeRotation => EyeAngles.ToRotation();
	public Vector3 WishVelocity { get; private set; }
	public CharacterController Controller => Body?.Components.GetAll<CharacterController>().FirstOrDefault() ?? null;
	public Vector3 Gravity => Vector3.Down * 800;

	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( Camera != null )
		{
			EyeAngles = Camera.Transform.Rotation.Angles();
			EyeAngles.roll = 0;
		}
	}

	protected override void OnUpdate()
	{
		if ( Camera != null )
		{
			EyeAngles.pitch += Input.MouseDelta.y * 0.1f; // Up-Down
			EyeAngles.yaw -= Input.MouseDelta.x * 0.1f; // Left-Right
			EyeAngles.roll = 0;

			Camera.Transform.Rotation = EyeRotation;
		}
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();

		if ( Controller != null )
		{
			if ( Controller.IsOnGround )
			{
				Controller.Velocity = Controller.Velocity.WithZ( 0 );
				Controller.Accelerate( WishVelocity );
				Controller.ApplyFriction( 4.0f );
			}
			else
			{
				Controller.Velocity += Gravity * Time.Delta * 0.5f;
				Controller.Accelerate( WishVelocity.ClampLength( 50 ) );
				Controller.ApplyFriction( 0.1f );
			}

			Controller.Move();

			if ( !Controller.IsOnGround )
				Controller.Velocity += Gravity * Time.Delta * 0.5f;
			else
				Controller.Velocity = Controller.Velocity.WithZ( 0 );
		}
	}

	public void BuildWishVelocity()
	{
		var eyeRotation = EyeRotation;

		WishVelocity = 0;

		if ( Input.Down( "Forward" ) ) WishVelocity += eyeRotation.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += eyeRotation.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += eyeRotation.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += eyeRotation.Right;

		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength )
			WishVelocity = WishVelocity.Normal;

		WishVelocity *= Input.Down( "Run" ) ? RunSpeed : WalkSpeed;
	}
}
