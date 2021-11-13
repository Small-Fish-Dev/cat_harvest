
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net] public bool EndState { get; set; } = false;
		[Net] public int Ending { get; set; } = 0;
		[Net] public bool Jumpscare { get; set; } = false;
		public bool Snappening = false;
		public static readonly string[] EndingTitles = new string[] {
			"NEUTRAL ENDING",
			"PEACEFUL ENDING",
			"BALANCED ENDING",
			"GENOCIDE ENDING",
			"SECRET ENDING",
			"Thank you for playing"
		};
		public static readonly string[] EndingDescriptions = new string[] {
			"After a hard day of work, you went back home to sleep.",
			"The world has been restored - and everyone is much happier.",
			"Perfectly balanced, as all things should be.",
			"Run.",
			"You found El Wiwi. You passed out not long after.",
			"There are 5 total endings, will you find them all?"
		};

		public static void EndGame( HarvestPlayer ply, int harvested, bool secret = false )
		{

			if ( secret )
			{

				SecretEnding( ply );
				return;

			}

			switch ( harvested )
			{

				case 48:
					BalancedEnding( ply );
					break;

				case <= 0:
					PeacefulEnding( ply );
					break;

				case >= 96:
					GenocideEnding( ply );
					break;

				default:
					NeutralEnding( ply );
					break;

			}

		}

		public static async void BalancedEnding( HarvestPlayer ply )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 2;

			await current.Task.Delay( 5000 );

			current.EndState = false;
			current.Finishing = true;

			ply.Camera = new BalancedEndingCamera();

			current.Snappening = true;

			ChangeMusic( "political" );

			await current.Task.Delay( 8000 );

			current.EndState = true;
			current.Ending = 5;

			await current.Task.Delay( 6000 );

			CloseGame( ply );

		}

		public static async void NeutralEnding( HarvestPlayer ply  )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 0;

			ChangeMusic( "bravo" );

			await current.Task.Delay( 4000 );

			current.EndState = false;
			current.Ending = 0;
			current.Finishing = true;

			await current.Task.Delay( 3000 );

			current.EndState = true;
			current.Ending = 5;

			await current.Task.Delay( 5000 );

			CloseGame( ply );

		}

		public static async void PeacefulEnding( HarvestPlayer ply  )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 1;

			ChangeMusic( "silly" );

			await current.Task.Delay( 5000 );

			int totCats = ply.CatsUprooted;
			current.Finishing = true;

			foreach ( WalkingCat cat in current.AllCats )
			{

				cat.Delete();

			}
			
			for ( int i = 0; i < totCats; i++ )
			{

				var cat = new WalkingCat
				{

					Position = new Vector3( Rand.Float( 1500f ) - 800f, Rand.Float( 1500f ), 15f ),
					Passive = true

				};

			}

			ply.Camera = new PeacefulEndingCamera();

			current.EndState = false;

			await current.Task.Delay( 7000 );

			current.EndState = true;
			current.Ending = 5;

			await current.Task.Delay( 5000 );

			CloseGame( ply );

		}

		public static async void GenocideEnding( HarvestPlayer ply  )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;
			WalkController controller = ply.Controller as WalkController;
			int totCats = ply.CatsHarvested;

			current.EndState = true;
			current.Ending = 3;

			await current.Task.Delay( 5000 );

			current.EndState = false;
			ChangeMusic( "horror" );
			ply.Position = new Vector3( 0, 0, 30 );
			controller.WalkSpeed = 30f;
			controller.DefaultSpeed = 30f;
			controller.SprintSpeed = 40f;

			for ( int i = 0; i < totCats; i++ )
			{

				var cat = new WalkingCat
				{

					Position = ply.Position + new Vector3( Rand.Float( 1500f ) - 800f, Rand.Float( 1500f ) - 800f, 15f ),
					Aggressive = true,
					Victim = ply

				};

			}



			for ( int i = 0; i < totCats; i++ )
			{

				ply.CatsHarvested--;
				ply.CatsUprooted--;

				await current.Task.Delay( 25 );

			}

			await current.Task.Delay( 4000 );

			current.EndState = true;
			current.Ending = 5;

			await current.Task.Delay( 6000 );

			CloseGame( ply );

		}

		public static async void SecretEnding( HarvestPlayer ply )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			ChangeMusic( "wonders" );
			current.Finishing = true;

			await current.Task.Delay( 2000 );

			current.EndState = true;
			current.Ending = 4;

			await current.Task.Delay( 2200 );

			var sound = Sound.FromEntity( "sad0", ply ).SetVolume( 1f );
			sound.SetPitch( 1.8f );

			await current.Task.Delay( 800 );

			Sound.FromEntity( "munch", ply ).SetVolume( 3f );

			await current.Task.Delay( 3000 );

			current.Jumpscare = true;
			Sound.FromScreen( "angry0" ).SetVolume( 255f );
			Sound.FromScreen( "angry0" ).SetVolume( 1f );
			Sound.FromScreen( "angry0" ).SetVolume( 10f );
			Sound.FromScreen( "angry0" ).SetVolume( 50f ); //Just to be sure, not sure why it doesn't play sometimes
			ChangeMusic( "" );

			await current.Task.Delay( 2000 );

			while ( true ) { };

		}

		[ClientRpc]
		public static void ChangeMusic( string music )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.Music.Stop();
			current.Music = current.PlaySound( music );

		}

		public static void CloseGame( HarvestPlayer ply )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			ply.Client.Kick();
			current.Delete();

		}

		float lastSnap = 0f;

		[Event.Tick.Server]
		public void OnTick()
		{

			if( Snappening && lastSnap <= Time.Now )
			{ 

				HarvestGame current = HarvestGame.Current as HarvestGame;

				if ( AllCats.Count > 0 )
				{

					int randomCat = Rand.Int(AllCats.Count - 1 );
					WalkingCat cat = AllCats[randomCat];

					cat.Snap();

				}

				lastSnap = Time.Now + 0.3f;

			}

		}

	}

}
