using System.Diagnostics;
using CatHarvest;
using CatHarvest.UI;
using CatHarvest.Util.Particles;
using Sandbox;
using Sandbox.Entities;
using Sandbox.Services;

public sealed class HarvestPlayer : Component
{
	public int CatsUprooted { get; set; } = 0;
	public int CatsHarvested { get; set; } = 0;
	[Property] public bool OpenInventory { get; set; } = false;
	[Property] public ShrimpleCharacterController.ShrimpleCharacterController _playerController;
	public bool CloseInstructions { get; set; } = false;
	public bool HasCat { get; set; } = false;
	public enum PopupType
	{
		None,
		Uproot,
		SecretPickUp
	}
	
	public PopupType Popup { get; set; } = PopupType.None;
	
	public CameraComponent OverrideCamera { get; set; } = null;

	public Vector3 InputDirection { get; set; }
	public Angles ViewAngles { get; set; }

	public SkinnedModelRenderer ViewModel { get; set; }

	protected override void OnStart()
	{
		Tags.Add("Player");
	}

	protected override void OnUpdate()
	{

	}

	TimeSince lastStep = 0f;
	TimeSince autoClose = 0f;
	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		if ( !GameObject.IsValid() ) return;
		if ( HarvestGame.The.Finishing ) return;
		OpenInventory = Input.Down( "Score" );
		if ( autoClose >= 15f || OpenInventory)
			CloseInstructions = true;
		
		var eyeTraceSetup = Scene.Trace.Ray( new Ray( Scene.Camera.WorldPosition, Scene.Camera.WorldTransform.Forward ), 75f )
			.Size( new Vector3( 20f, 20f, 20f ) )
			.IgnoreGameObjectHierarchy(this.GameObject.Root)
			.WithTag( "Cat" );
		var eyeTrace = eyeTraceSetup.Run();
		var lookingAtCat = eyeTrace.Hit && eyeTrace.GameObject.Tags.Has( "cat" );
		if ( lookingAtCat && !HasCat )
		{
			var cat = eyeTrace.GameObject;
			if ( cat.GetComponent<WalkingCat>().IsValid() && cat.GetComponent<WalkingCat>().IsSecret )
			{
				Popup = PopupType.SecretPickUp;

				if ( Input.Pressed( "use" ) )
				{
					cat.Destroy();

					GetComponentInChildren<SkinnedModelRenderer>().Set( "wiwi", true );
					HarvestGame.EndGame( this, 0, true );
				}
			}
			else
			{
				if ( cat.GetComponent<WalkingCat>().IsValid() ) return;
				Popup = PopupType.Uproot;

				if ( Input.Pressed( "use" ) )
				{
					Sound.Play( $"meow{Game.Random.Int( 10 )}", cat.WorldPosition );
					var particle = LegacyParticle.Create( "particles/uproot.vpcf", cat.WorldPosition );
						particle.GameObject.DestroyAsync(3);
					
					cat.Destroy();
					// GameServices.SubmitScore( Client.PlayerId, 1 );
					Sandbox.Services.Stats.Increment("total_cats_uprooted",1);
					CatsUprooted++;
					GetComponentInChildren<SkinnedModelRenderer>().Set( "grab", true );
					HasCat = true;
				}
			}
			
		}
		else
		{
			Popup = PopupType.None;
		}

		if ( _playerController.Velocity.Length > 0f && lastStep >= 70 / _playerController.Velocity.Length && _playerController.IsOnGround )
		{
			var step = $"step{Game.Random.Int( 5 )}";
			Sound.Play( step, WorldPosition );
			lastStep = 0f;
		}
	}

	public static void Harvest()
	{
		var ply = Game.ActiveScene.Scene.GetComponentInChildren<HarvestPlayer>();
		if ( !ply.IsValid() ) return;
		ply.CatsHarvested++;
		ply.HasCat = false;
		ply.GetComponentInChildren<SkinnedModelRenderer>().Set("finished", true);
		Sandbox.Services.Stats.Increment("cats_harvested",1);
		
		Sound.Play( $"sad{Game.Random.Int( 1 )}", ply.WorldPosition );
		var particle = LegacyParticle.Create( "particles/dollars.vpcf", ply.WorldPosition + Game.ActiveScene.Scene.Camera.WorldRotation.Forward * 32f + ply.WorldRotation.Up * 48f);
		particle.GameObject.DestroyAsync(3);
		if ( ply.CatsUprooted == 96 )
		{
			HarvestGame.EndGame( ply, ply.CatsHarvested );
		}
	}
	
	public static void Rescue()
	{
		var ply = Game.ActiveScene.Scene.GetComponentInChildren<HarvestPlayer>();

		var cat = GameObject.Clone( "prefabs/walkingcat.prefab", ply.WorldTransform );
		var particle = LegacyParticle.Create( "particles/hearts.vpcf", cat.WorldPosition + Game.ActiveScene.Scene.Camera.WorldRotation.Forward * 32f + ply.WorldRotation.Up * 48f );
		particle.GameObject.DestroyAsync(3);
		ply.HasCat = false;
		ply.GetComponentInChildren<SkinnedModelRenderer>().Set( "finished", true );
		Sandbox.Services.Stats.Increment("cats_rescued",1);
		
		if ( ply.CatsUprooted == 96 )
		{
			HarvestGame.EndGame( ply, ply.CatsHarvested );
		}
	}

	
	public void OnKilled()
	{
		//Don't die! wtf
	}
}
