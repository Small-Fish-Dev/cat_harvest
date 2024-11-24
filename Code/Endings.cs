
using Sandbox.Entities;

namespace CatHarvest;

public partial class HarvestGame
{
	public bool EndState { get; set; } = false;
	public int Ending { get; set; } = 0;
	public bool Jumpscare { get; set; } = false;
	public bool Snappening = false;

	public static readonly string[] EndingTitles = new string[]
	{
		"NEUTRAL ENDING",
		"PEACEFUL ENDING",
		"BALANCED ENDING",
		"GENOCIDE ENDING",
		"SECRET ENDING",
		"Thank you for playing"
	};

	public static readonly string[] EndingDescriptions = new string[]
	{
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
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

		current.EndState = true;
		current.Ending = 2;

		await current.Task.Delay( 5000 );

		current.EndState = false;
		current.Finishing = true;

		Game.ActiveScene.Scene.GetComponentInChildren<EndingCam>().IsBalancedEnding = true;
		Game.ActiveScene.Scene.GetComponentInChildren<EndingCam>().tsCreated = 0;
		
		current.Snappening = true;

		ChangeMusic( "political" );

		await current.Task.Delay( 8000 );

		current.EndState = true;
		current.Ending = 5;

		Sandbox.Services.Achievements.Unlock( "balanced_ending" );
		await current.Task.Delay( 6000 );
		CloseGame( ply );
	}

	public static async void NeutralEnding( HarvestPlayer ply )
	{
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

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
		Sandbox.Services.Achievements.Unlock( "neutral_ending" );
		await current.Task.Delay( 5000 );

		CloseGame( ply );
	}

	public static async void PeacefulEnding( HarvestPlayer ply )
	{
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

		current.EndState = true;
		current.Ending = 1;

		ChangeMusic( "sounds/silly.sound" );

		await current.Task.Delay( 5000 );

		int totCats = ply.CatsUprooted;
		current.Finishing = true;

		foreach ( var cat in current.AllCats )
		{
			cat.Destroy();
		}

		for ( int i = 0; i < totCats; i++ )
		{
			var cat = Sandbox.GameObject.Clone( "prefabs/walkingcat.prefab");
			cat.WorldPosition =
				new Vector3( Game.Random.Float( 1500f ) - 800f, Game.Random.Float( 1500f ), 15f );
			cat.GetComponent<WalkingCat>().Passive = true;
		}

		Game.ActiveScene.Scene.GetComponentInChildren<EndingCam>().IsPeacefulEnding = true;
		Game.ActiveScene.Scene.GetComponentInChildren<EndingCam>().tsCreated = 0;

		current.EndState = false;

		await current.Task.Delay( 7000 );

		current.EndState = true;
		current.Ending = 5;
		Sandbox.Services.Achievements.Unlock( "peaceful_ending" );
		await current.Task.Delay( 5000 );

		CloseGame( ply );
	}

	public static async void GenocideEnding( HarvestPlayer ply )
	{
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();
		int totCats = ply.CatsHarvested;

		current.EndState = true;
		current.Ending = 3;

		await current.Task.Delay( 5000 );

		current.EndState = false;
		ChangeMusic( "horror" );
		ply.WorldPosition = new Vector3( 0, 0, 30 );
		ply.GetComponent<ShrimpleWalker>().RunSpeed = 30f;
		ply.GetComponent<ShrimpleWalker>().WalkSpeed = 30f;

		for ( int i = 0; i < totCats; i++ )
		{
			var cat = Sandbox.GameObject.Clone( "prefabs/walkingcat.prefab");
			cat.WorldPosition =
				ply.WorldPosition + new Vector3( Game.Random.Float( 1500f ) - 800f, Game.Random.Float( 1500f ) - 800f, 15f );
			cat.GetComponent<WalkingCat>().Aggressive = true;
			cat.GetComponent<WalkingCat>().Victim = ply;
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

		Sandbox.Services.Achievements.Unlock( "genocide_ending" );
		await current.Task.Delay( 6000 );

		CloseGame( ply );
	}

	public static async void SecretEnding( HarvestPlayer ply )
	{
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

		ChangeMusic( "sounds/wonders.sound" );
		current.Finishing = true;

		await current.Task.Delay( 2000 );

		current.EndState = true;
		current.Ending = 4;

		await current.Task.Delay( 2200 );

		var sound = Sound.Play( "sad0", ply.WorldPosition );
		sound.Volume = 1f;
		sound.Pitch = 1.8f;

		await current.Task.Delay( 800 );

		Sound.Play( "munch", ply.WorldPosition ).Volume = 3;

		await current.Task.Delay( 3000 );

		current.Jumpscare = true;
		Sound.Play( "angry0" ).Volume = 255f;
		Sound.Play( "angry0" ).Volume = 1f;
		Sound.Play( "angry0" ).Volume = 10f;
		Sound.Play( "angry0" ).Volume = 50f; //Just to be sure, not sure why it doesn't play sometimes
		ChangeMusic( "" );
		Sandbox.Services.Achievements.Unlock( "secret_ending" );
		await current.Task.Delay( 2000 );

		//real funny
		if ( Game.IsEditor )
		{
			Game.Close();
			return;
		}
		while ( true )
		{
		}
	}
	public static void ChangeMusic( string music )
	{
		HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

		current.Music.Stop();
		var Song = Sound.Play( music );
		
	}

	public static void CloseGame( HarvestPlayer ply )
	{
		Game.Close();
	}

	float lastSnap = 0f;

	public void OnTick()
	{
		if ( Snappening && lastSnap <= Time.Now )
		{
			HarvestGame current = Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

			if ( AllCats.Count > 0 )
			{
				int randomCat = Game.Random.Int( AllCats.Count - 1 );
				GameObject cat = AllCats[randomCat];

				cat.GetComponent<WalkingCat>().Snap();
			}

			lastSnap = Time.Now + 0.3f;
		}
	}
}

