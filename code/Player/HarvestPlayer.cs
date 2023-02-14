namespace CatHarvest.Player;

public partial class HarvestPlayer : AnimatedEntity
{
	[Net] public int CatsUprooted { get; set; } = 0;
	[Net] public int CatsHarvested { get; set; } = 0;
	public bool OpenInventory { get; set; } = false;
	public bool CloseInstructions { get; set; } = false;
	[Net] public bool HasCat { get; set; } = false;

	public enum PopupType
	{
		None,
		Uproot,
		SecretPickUp
	}
	[Net, Predicted] public PopupType Popup { get; set; } = PopupType.None;

	[Net] public Cameras.BaseCamera OverrideCamera { get; set; } = null;

	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public HarvestViewModel ViewModel { get; set; }

	public override void Spawn()
	{
		EnableLagCompensation = true;

		SetModel( "models/citizen/citizen.vmdl" );

		Tags.Add( "Player" );

		CreateViewModel();

		EnableAllCollisions = true;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	public override void BuildInput()
	{
		InputDirection = Input.AnalogMove;

		var look = Input.AnalogLook;

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -85, 85 );
		ViewAngles = viewAngles.Normal;
	}

	TimeSince lastStep = 0f;
	TimeSince autoClose = 0f;

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Rotation = ViewAngles.ToRotation();

		SimulateController( cl );

		if ( Game.IsClient )
		{
			if ( Input.Down( InputButton.Score ) )
			{
				OpenInventory = true;
				CloseInstructions = true;
			}
			else
			{
				OpenInventory = false;
			}

			if ( autoClose >= 15f )
			{
				CloseInstructions = true;
			}
		}

		var tracePosition = Position + CollisionBox.Maxs.z * Vector3.Up;
		var traceDirection = Rotation.Forward;

		var eyeTraceSetup = Trace.Ray( new Ray( tracePosition, traceDirection ), 75f )
			.Size( new Vector3( 20f, 20f, 20f ) )
			.Ignore( this )
			.WithTag( "Cat" );
		var eyeTrace = eyeTraceSetup.Run();

		var lookingAtCat = eyeTrace is { Hit: true, Entity: WalkingCat or PlantedCat };

		if ( lookingAtCat && !HasCat )
		{
			var cat = eyeTrace.Entity;

			if ( cat is WalkingCat walkingCat && walkingCat.IsSecret() )
			{
				var secretTrace = eyeTraceSetup
					.Size( new Vector3( 10, 10, 10 ) )
					.Run();
				walkingCat = secretTrace.Entity as WalkingCat;

				if ( walkingCat.IsSecret() )
				{
					Popup = PopupType.SecretPickUp;

					if ( Input.Pressed( InputButton.Use ) && Game.IsServer )
					{
						cat.Delete();

						SetAnim( "wiwi", true );
						HarvestGame.EndGame( this, 0, true );
					}
				}
			}
			else
			{
				Popup = PopupType.Uproot;

				if ( Input.Pressed( InputButton.Use ) )
				{
					Sound.FromWorld( $"meow{Game.Random.Int( 10 )}", cat.Position );
					Particles.Create( "particles/uproot.vpcf", cat.Position );

					if ( Game.IsServer )
					{
						cat.Delete();

						// GameServices.SubmitScore( Client.PlayerId, 1 );
						Log.Info( "TODO: scoreboard" );
						CatsUprooted++;
						SetAnim( "grab", true );
						HasCat = true;
					}
				}
			}
		}
		else
		{
			Popup = PopupType.None;
		}

		if ( Velocity.Length > 0f && lastStep >= 70 / Velocity.Length && GroundEntity != null )
		{
			var step = $"step{Game.Random.Int( 5 )}";
			Sound.FromEntity( step, this );
			lastStep = 0f;
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		if ( OverrideCamera is { } camera )
		{
			camera.Update();
			return;
		}

		// Update rotation every frame, to keep things smooth
		Rotation = ViewAngles.ToRotation();

		Camera.Position = Position + CollisionBox.Maxs.z * Vector3.Up;
		Camera.Rotation = Rotation;

		// Set field of view to whatever the user chose in options
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		// Set the first person viewer to this, so it won't render our model
		Camera.FirstPersonViewer = this;
	}

	[ClientRpc]
	public void CreateViewModel()
	{
		ViewModel = new HarvestViewModel
		{
			Owner = Owner,
			EnableViewmodelRendering = true
		};
	}

	[ClientRpc]
	protected virtual void SetAnim( string name, bool state )
	{
		Game.AssertClient();

		(Game.LocalPawn as HarvestPlayer)?.ViewModel.SetAnimParameter( name, state );
	}

	[ConCmd.Server]
	public static void Harvest()
	{
		var ply = ConsoleSystem.Caller.Pawn as HarvestPlayer;

		ply.CatsHarvested++;
		ply.HasCat = false;
		ply.SetAnim( "finished", true );

		Sound.FromEntity( $"sad{Game.Random.Int( 1 )}", ply );
		Particles.Create( "particles/dollars.vpcf", ply.Position );

		if ( ply.CatsUprooted == 96 )
		{
			HarvestGame.EndGame( ply, ply.CatsHarvested );
		}
	}

	[ConCmd.Server]
	public static void Rescue()
	{
		var ply = ConsoleSystem.Caller.Pawn as HarvestPlayer;

		var cat = new WalkingCat
		{
			Position = ply.Position
		};

		Particles.Create( "particles/hearts.vpcf", cat.Position );

		ply.HasCat = false;
		ply.SetAnim( "finished", true );

		if ( ply.CatsUprooted == 96 )
		{
			HarvestGame.EndGame( ply, ply.CatsHarvested );
		}
	}

	public override void OnKilled()
	{
		//Don't die! wtf
	}
}
