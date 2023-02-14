namespace CatHarvest.Player;

partial class HarvestPlayer
{
	public Vector3 WishVelocity { get; set; }

	/// <summary>
	/// The collider used for player's movement collisions.
	/// </summary>
	public BBox CollisionBox => new BBox(
		new Vector3( -16, -16, 0 ),
		new Vector3( 16, 16, Ducking ? 40 : 68 )
	);

	/// <summary>
	/// A boolean determining if the player is ducked or not.
	/// </summary>
	public bool Ducking { get; private set; } = false;

	// Private fields
	[Net] public float Speed { get; set; } = 120f;

	private float stepSize => 8f;
	private float maxStandableAngle => 45f;
	private Vector3 gravity => Vector3.Down * 650f;

	/// <summary>
	/// Simulates the player's movement.
	/// </summary>
	/// <param name="cl"></param>
	protected void SimulateController( IClient cl )
	{
		// Handle jumping.
		if ( Input.Pressed( InputButton.Jump ) && GroundEntity != null )
		{
			GroundEntity = null;
			Velocity += Vector3.Up * 200f;
		}

		// Handle ducking.
		Ducking = Input.Down( InputButton.Duck );

		// Handle rotation.
		var viewAngles = new Angles( 0, ViewAngles.yaw, 0 );
		Rotation = Angles.Lerp( Rotation.Angles(), viewAngles, 10f * Time.Delta )
			.ToRotation();

		// Handle the player's wish velocity.
		var eyeRotation = ViewAngles.WithPitch( 0 ).ToRotation();
		WishVelocity = (InputDirection
			* eyeRotation).Normal.WithZ( 0 );

		// Calculate velocity.
		var targetVelocity = WishVelocity
			* (Speed * (Input.Down( InputButton.Run ) ? 1.5f : 1))
			* (Ducking ? 0.5f : 1f);

		Velocity = Vector3.Lerp( Velocity, targetVelocity, 10f * Time.Delta )
			.WithZ( Velocity.z );

		if ( GroundEntity == null )
			Velocity += gravity * Time.Delta;

		// Initialize MoveHelper.
		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = maxStandableAngle
		};

		// Move the player using MoveHelper.
		helper.Trace = helper.Trace
			.Size( CollisionBox.Mins, CollisionBox.Maxs )
			.Ignore( this );

		if ( helper.HitWall )
			helper.ApplyFriction( 5f, Time.Delta );

		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, stepSize );

		Position = helper.Position;
		Velocity = helper.Velocity;

		// Check for ground collision.
		if ( Velocity.z <= stepSize )
		{
			var tr = helper.TraceDirection( Vector3.Down * 2f );
			GroundEntity = tr.Entity;

			// Move to the ground if there is something.
			if ( GroundEntity != null )
			{
				Position += tr.Distance * Vector3.Down;

				if ( Velocity.z < 0.0f )
					Velocity = Velocity.WithZ( 0 );
			}
		}
		else
			GroundEntity = null;
	}
}
