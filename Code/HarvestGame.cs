global using System;
global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Editor;
global using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CatHarvest.UI;
using Sandbox.Entities;

namespace CatHarvest;


public partial class HarvestGame : Component
{
	public static HarvestGame The => Game.ActiveScene.Scene.GetComponentInChildren<HarvestGame>();

	[Property]public List<GameObject> AllCats { get; set; } = new List<GameObject>();
	public WalkingCat SecretCat { get; set; }
	public bool Finishing { get; set; } = false;
	public SoundHandle Music { get; set; }

	protected override void OnStart()
	{
		SecretCat = GameObject.Clone( "prefabs/walkingcat.prefab" )
			.Clone( new Vector3( Game.Random.Float( 1500f ) - 800f, Game.Random.Float( 1500f ), 25f ) )
			.GetComponent<WalkingCat>();
		SecretCat.WorldScale = 0.1f;
		SecretCat.IsSecret = true;
		Music = Sound.Play( "sounds/relax.sound" );
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		var allObjects = Scene.GetAllObjects(true)
			.Where( x => x.Tags.Has( "cat") && x.GetComponent<WalkingCat>().IsValid() );
		
		AllCats = allObjects.ToList();
		
		TheSnappening();
	}

	[ConCmd( "spawncats" )]
	public static void SpawnCats()
	{
		var ply = Game.ActiveScene.GetComponent<HarvestPlayer>();

		for ( var i = 0; i < 1000; i++ )
		{
			var cat = Sandbox.GameObject.Clone( "prefabs/walkingcat.prefab", global::Transform.Zero );
		}
	}
}

