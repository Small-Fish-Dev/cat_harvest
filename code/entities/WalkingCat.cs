using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cat_Harvest
{
	public partial class WalkingCat : AnimEntity
	{

		private readonly Vector3 minBounds = new Vector3( -800, -770, 0 );
		private readonly Vector3 maxBounds = new Vector3( 750, 790, 0 );
		[Net] public bool IsDying { get; set; } = false;
		public bool Aggressive { get; set; } = false;
		public HarvestPlayer Victim { get; set; }

		public override void Spawn()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			base.Spawn();

			Tags.Add( "Cat" );

			SetModel( "models/cat/cat.vmdl" );
			Scale = 1f;

			current.AllCats.Add( this );

		}

		float nextMove = 0f;

		[Event.Tick.Server]
		public void Tick()
		{

			if ( nextMove <= Time.Now )
			{

				if ( Position.x <= minBounds.x || Position.x >= maxBounds.x || Position.y <= minBounds.y || Position.y >= maxBounds.y )
				{

					Velocity = ( Vector3.Zero - Position ).Normal * 4;

				}
				else
				{

					if ( Aggressive )
					{

						if ( Victim.IsValid() )
						{

							Velocity = ( ( Victim.Position + new Vector3( Rand.Float( 30f ) - 15f, Rand.Float( 30f ) - 15f, 0 ) ) - Position ).Normal * 3;

						}

						Sound.FromEntity( $"angry{ Rand.Int( 1 ) }", this );

					}
					else
					{

						Velocity = new Vector3( Rand.Float( 2f ) - 1f, Rand.Float( 2f ) - 1f, 0f ).Normal * 2;
						Sound.FromEntity( $"meow{ Rand.Int( 10 ) }", this ).SetVolume( 0.2f );

					}

				}

				nextMove = Time.Now +  Rand.Float( 2f ) + ( Aggressive ? 0f : 6f );

			}

			TraceResult traceGround = Trace.Ray( Position + Vector3.Up * 16, Position + Vector3.Down * 32 )
				.Ignore( this )
				.WithoutTags( "Player" )
				.WithoutTags( "Cat" )
				.Run();

			if ( traceGround.Hit )
			{

				Position = traceGround.EndPos;
				Position += Rotation.Forward * 10 * Velocity.Length * Time.Delta;

				Rotation rotation = Velocity.EulerAngles.ToRotation();
				Rotation = Rotation.Slerp( Rotation, rotation, 2 * Time.Delta );

			}
			else
			{

				Position += Vector3.Down * 300 * Time.Delta;

			}

		}

		float hourOfDeath = 0f;

		public void Snap()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			Particles.Create( "particles/ashes.vpcf", Position );
			IsDying = true;
			hourOfDeath = Time.Now + 0.2f;

			current.AllCats.Remove( this );

		}

		[Event.Tick.Server]
		public void OnTick()
		{

			if ( IsDying )
			{

				RenderColor = RenderColor.WithAlpha( ( hourOfDeath - Time.Now ) * 5 );

				if( Time.Now >= hourOfDeath )
				{

					Delete();

				}

			}

		}

		[ServerCmd("spawncat")] //TODO REMEMBER DELETE!!!
		public static void SpawnCat()
		{

			var pos = ConsoleSystem.Caller.Pawn.Position;

			for ( int i = 0; i < 46; i++ )
			{

				var npc = new WalkingCat
				{

					Position = pos

				};

			}

		}



	}

}
