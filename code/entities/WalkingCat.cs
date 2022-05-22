using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cat_Harvest
{
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

			HarvestGame current = HarvestGame.Current as HarvestGame;

			base.Spawn();

			PlaybackRate = 0f;

			Tags.Add( "Cat" );

			SetModel( "models/cat/cat.vmdl" );

			current.AllCats.Add( this );

			
		}

		float nextMove = 0f;

		RealTimeSince frameTime = 0f;
		float lastDistance = 0f;
		float nextFrame = 0f;
		
		[Event.Tick.Client]
		public void ClientTick()
		{
			float frameDelta = 1f / 24f;
			float minFps = 1f;
			float minDistanceFalloff = 300f;
			float maxDistanceFalloff = 2000f;

			if ( frameTime >= nextFrame )
			{

				CurrentSequence.Time = (CurrentSequence.Time + frameTime) % CurrentSequence.Duration;
				lastDistance = Math.Max( CurrentView.Position.Distance( Position ) - minDistanceFalloff, 1f );
				nextFrame = MathX.LerpTo( frameDelta, minFps, lastDistance / maxDistanceFalloff );

				frameTime = 0f;

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

		public bool IsSecret()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			if ( this == current.SecretCat )
			{

				return true;

			}

			return false;

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

	}

}
