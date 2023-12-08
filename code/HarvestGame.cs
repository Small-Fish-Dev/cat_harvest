global using System;
global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Editor;
global using System.Collections.Generic;
global using CatHarvest.Player;
//global using CatHarvest.UI;
global using CatHarvest.Entities;

namespace CatHarvest;
/*
public partial class HarvestGame : GameManager
{
	public static HarvestGame The { get; private set; }

	[Net] public IList<WalkingCat> AllCats { get; set; } = new List<WalkingCat>();
	[Net] public WalkingCat SecretCat { get; set; }
	[Net] public bool Finishing { get; set; } = false;
	public Sound Music { get; set; }

	public HarvestGame()
	{
		The = this;

		if ( Game.IsServer )
		{
			_ = new HarvestHUD();

			SecretCat = new WalkingCat
			{
				Position = new Vector3( Game.Random.Float( 1500f ) - 800f, Game.Random.Float( 1500f ), 25f ),
				Scale = 0.1f
			};

			SecretCat.SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -0.5f, -0.5f, -0.5f ),
				new Vector3( 0.5f, 0.5f, 0f ) ); //Needs physics to be able to be picked up;
		}
		else
		{
			Music = PlaySound( "relax" );
		}

		Precache.Add( "angry0" );
		Precache.Add( "sounds/angry0.vsnd" ); //To be sure
	}

	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		var player = new HarvestPlayer();
		client.Pawn = player;
	}

	[ConCmd.Server( "spawncats" )]
	public static void SpawnCats()
	{
		var ply = ConsoleSystem.Caller.Pawn as HarvestPlayer;

		for ( var i = 0; i < 1000; i++ )
		{
			var cat = new WalkingCat
			{
				Position = ply.Position +
						   new Vector3( Game.Random.Float( -1000, 1000 ), Game.Random.Float( -1000, 1000 ), Game.Random.Float( -500, 500 ) )
			};
		}
	}
}

*/
