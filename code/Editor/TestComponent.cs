using Sandbox;
using System;

public class TestComponent : Component
{
	public void Hello()
	{
		Log.Info( "I'm hitting myself" );
	}
}
