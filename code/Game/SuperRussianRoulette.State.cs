using Sandbox;
using System.Linq;

using Roulette;
using System.Runtime.CompilerServices;

namespace Roulette;

public enum GameStates // Current state of the game // Taken from various facepunch games, I assume this is the standard convention, who knows.
{
	Intermission, Voting, Match, SuddenDeath, Results
}

partial class SuperRussianRoulette : Game // Redundant :? not sure	
{
	public static GameStates CurrentState => (Current as SuperRussianRoulette)?.GameState ?? GameStates.Intermission; // Terrible, from deathmatch but I don't have time for a cleaner implementation.
	public static new SuperRussianRoulette Current => Game.Current as SuperRussianRoulette;

	[Net]
	public GameStates GameState { get; set; } = GameStates.Intermission;

	private bool PlayerThreshHold()
	{
		if ( All.OfType<RoulettePlayer>().Count() < 2 ) { return false; } else { return true; }
	}
}
