namespace CatHarvest.Entities;

public partial class WalkingCat : AnimatedEntity
{

	private readonly Vector3 minBounds = new Vector3( -800, -770, 0 );
	private readonly Vector3 maxBounds = new Vector3( 750, 790, 0 );
	[Net] public bool IsDying { get; set; } = false;
	public bool Aggressive { get; set; } = false;
	public bool Passive { get; set; } = false;
	public HarvestPlayer Victim { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		PlaybackRate = 0f;

		Tags.Add( "Cat" );

		SetModel( "models/cat/cat.vmdl" );

		HarvestGame.The.AllCats.Add( this );
	}

	RealTimeSince frameTime = 0f;
	float lastDistance = 0f;
	float nextFrame = 0f;
	float nextMove = 0f;

	[Event.Tick]
	public void Tick()
	{
		if ( Game.IsClient )
		{
			float frameDelta = 1f / 24f;
			float minFps = 1f;
			float minDistanceFalloff = 300f;
			float maxDistanceFalloff = 2000f;

			if ( frameTime >= nextFrame )
			{
				CurrentSequence.Time = (CurrentSequence.Time + frameTime) % CurrentSequence.Duration;
				lastDistance = Math.Max( Camera.Position.Distance( Position ) - minDistanceFalloff, 1f );
				nextFrame = frameDelta.LerpTo( minFps, lastDistance / maxDistanceFalloff );

				frameTime = 0f;
			}

			return;
		}

		if ( nextMove <= Time.Now )
		{
			if ( !Passive )
			{
				if ( Position.x <= minBounds.x || Position.x >= maxBounds.x || Position.y <= minBounds.y || Position.y >= maxBounds.y )
				{
					Velocity = (Vector3.Zero - Position).Normal * (IsSecret() ? 1.5f : 4);
				}
				else
				{
					if ( Aggressive )
					{
						if ( Victim.IsValid() )
						{
							Velocity = ((Victim.Position + new Vector3( Game.Random.Float( 30f ) - 15f, Game.Random.Float( 30f ) - 15f, 0 )) - Position).Normal * 2;
						}

						Sound.FromEntity( $"angry{Game.Random.Int( 1 )}", this );
					}
					else
					{
						Velocity = new Vector3( Game.Random.Float( 2f ) - 1f, Game.Random.Float( 2f ) - 1f, 0f ).Normal * (IsSecret() ? 0.3f : 2);
						var meow = Sound.FromEntity( $"meow{Game.Random.Int( 10 )}", this ).SetVolume( IsSecret() ? 0.3f : 0.15f );

						if ( IsSecret() )
						{
							meow.SetPitch( 2f );
						}
					}
				}

				nextMove = Time.Now + Game.Random.Float( 2f ) + (Aggressive ? 0f : 6f);
			}
			else
			{
				Velocity = new Vector3( 0f, 4f, 0f );
			}
		}

		var traceGround = Trace.Ray( Position + Vector3.Up * 16, Position + Vector3.Down * 32 )
			.Ignore( this )
			.WithoutTags( "Player" )
			.WithoutTags( "Cat" )
			.Run();

		if ( traceGround.Hit )
		{
			Position = traceGround.EndPosition;
			Position += Rotation.Forward * 10 * Velocity.Length * Time.Delta;

			Rotation rotation = Velocity.EulerAngles.ToRotation();
			Rotation = Rotation.Slerp( Rotation, rotation, 2 * Time.Delta );
		}
	}

	TimeUntil hourOfDeath = 0f;

	public void Snap()
	{
		Particles.Create( "particles/ashes.vpcf", Position );
		hourOfDeath = 0.2f;
		IsDying = true;

		HarvestGame.The.AllCats.Remove( this );
	}

	public bool IsSecret() => HarvestGame.The.SecretCat == this;

	[Event.Tick.Server]
	public void OnTick()
	{
		if ( IsDying )
		{
			RenderColor = RenderColor.WithAlpha( (hourOfDeath - Time.Now) * 5 );

			if ( hourOfDeath <= 0 )
			{
				Delete();
			}
		}
	}
}
