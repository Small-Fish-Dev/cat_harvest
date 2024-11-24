using System;
using System.Transactions;
using CatHarvest;
using CatHarvest.Util.Particles;

namespace Sandbox.Entities;

public class WalkingCat : Component
{
	[Property] private CharacterController _characterController { get; set; }
	private readonly Vector3 minBounds = new Vector3( -800, -770, 0 );
	private readonly Vector3 maxBounds = new Vector3( 750, 790, 0 );
	[Property] public bool IsDying { get; set; } = false;
	[Property] public bool Aggressive { get; set; } = false;
	[Property] public bool Passive { get; set; } = false;
	[Property] public HarvestPlayer Victim { get; set; }

	protected override void OnStart()
	{
		base.OnStart();
		Tags.Add( "cat" );
	}
	
	RealTimeSince frameTime = 0f;
	float lastDistance = 0f;
	float nextFrame = 0f;
	float nextMove = 0f;
	[Property] public bool IsSecret;

	
	protected override void OnFixedUpdate()
	{
		OnTick();
		if ( !GameObject.IsValid() || GameObject.IsDestroyed ) return;
		var traceGround = Scene.Trace.Ray(WorldPosition + Vector3.Up * 16, WorldPosition + Vector3.Down * 32f)
			.IgnoreGameObjectHierarchy( this.GameObject.Root )
			.WithoutTags( "Player" )
			.WithoutTags( "Cat" )
			.Run();
		if ( IsProxy )
		{
			float frameDelta = 1f / 24f;
			float minFps = 1f;
			float minDistanceFalloff = 300f;
			float maxDistanceFalloff = 2000f;

			if ( frameTime >= nextFrame )
			{
				lastDistance = Math.Max( Scene.Camera.WorldPosition.Distance( WorldPosition ) - minDistanceFalloff,
					1f );
				nextFrame = frameDelta.LerpTo( minFps, lastDistance / maxDistanceFalloff );

				frameTime = 0f;
			}

			return;
		}
		if ( nextMove <= Time.Now )
		{
			if ( !Passive )
			{
				if ( WorldPosition.x <= minBounds.x || WorldPosition.x >= maxBounds.x ||
				     WorldPosition.y <= minBounds.y || WorldPosition.y >= maxBounds.y )
				{
					_characterController.Velocity = (Vector3.Zero - WorldPosition).Normal * (IsSecret ? 1.5f : 4);
				}
				else
				{
					if ( Aggressive )
					{
						if ( Victim.IsValid() )
						{
							_characterController.Velocity =
								((Victim.WorldPosition + new Vector3( Game.Random.Float( 30f ) - 15f,
									Game.Random.Float( 30f ) - 15f, 0 )) - WorldPosition).Normal * 2;
						}

						Sound.Play( $"angry{Game.Random.Int( 1 )}", WorldPosition );
					}
					else
					{
						_characterController.Velocity =
							new Vector3( Game.Random.Float( 2f ) - 1f, Game.Random.Float( 2f ) - 1f, 0f ).Normal *
							(IsSecret ? 0.3f : 2);
						var meow = Sound.Play( $"meow{Game.Random.Int( 10 )}", WorldPosition );
						meow.Volume = IsSecret ? 0.3f : 0.15f;

						if ( IsSecret )
						{
							meow.Pitch = 2f;
						}
					}
				}

				nextMove = Time.Now + Game.Random.Float( 2f ) + (Aggressive ? 0f : 6f);
			}
			else
			{
				_characterController.Velocity = new Vector3( 0f, 4f, 0f );
			}
		}
		_characterController.Move();

		if ( traceGround.Hit )
		{
			WorldPosition = traceGround.EndPosition;
			WorldPosition += WorldRotation.Forward * 8 * _characterController.Velocity.Length * Time.Delta;
			Rotation rotation = _characterController.Velocity.EulerAngles.ToRotation();
			WorldRotation = Rotation.Slerp( WorldRotation, rotation, 2 * Time.Delta );
		}
		
		if ( IsDying )
		{
			Log.Info("test");
			GetComponent<SkinnedModelRenderer>().Tint = Color.FromBytes( 255, 255, 255, (int)(hourOfDeath - Time.Now) * 5 );

			if ( hourOfDeath <= 0 )
			{
				Destroy();
			}
		}
	}
	
	TimeUntil hourOfDeath = 0f;

	public void Snap()
	{
		var particle = LegacyParticle.Create( "particles/ashes.vpcf", WorldPosition );
		particle.GameObject.DestroyAsync(3);
		hourOfDeath = 0.2f;
		IsDying = true;

		HarvestGame.The.AllCats.Remove( this.GameObject );
	}

	public void OnTick()
	{
	}
}
