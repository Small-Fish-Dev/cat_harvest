using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	public partial class WalkingCat : AnimEntity
	{

		public override void Spawn()
		{

			base.Spawn();

			SetModel( "models/citizen/citizen.vmdl" );
			CollisionGroup = CollisionGroup.Prop;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 16, 2 ) );

		}

	}

}
