using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace GameOverScreen;

// TODO split all classes to multiple files
// TODO add Language support for all and move strings to XML

public class ChoiceLetter_GameOverScreen : ChoiceLetter
{
	// TODO as mentioned above, move all this stuff to XML and language pack
	private const string colonyPerishedIntro = "Colony has perished in the harsh reality of this rimworld.";
	private const string colonyPerishedEnding = "Stories of its inhabitants have ended";
	public void PlayEndgameScreen()
	{
		GameVictoryUtility.ShowCredits(GameVictoryUtility.MakeEndCredits(colonyPerishedIntro, colonyPerishedEnding, null), null, true);
	}

	public override IEnumerable<DiaOption> Choices
	{
		get
		{
			yield return new DiaOption("Show endgame credits")
			{
				action = this.PlayEndgameScreen,
				resolveTree = true
			};
			yield return new DiaOption("Keep playing")
			{
				resolveTree = true
			};
		}
	}
}

public class Monitor_GameOverScreen : GameComponent
{
	private int lastCheckTick = 0;
	private bool showedEngamePopup = false;
	/* TODO add check if ManInBlack is possible and delay that.
	 * Although it polls state from GameEnder so should be precise.
	 */
	private const int checkFrequency = 300; // ~ 5s

	// This is copied from original screen, probably should get this text from XML
	private const string letter_tagged_text = "Everyone is dead or gone. This story is over.\n";

	public Monitor_GameOverScreen(Game game)
	{
		// Nothing to initialize
	}

	private bool CheckGameEnding()
	{
		if (Find.GameEnder.gameEnding & !this.showedEngamePopup)
		{
			this.showedEngamePopup = true;
			return true;
		}
		else if (Find.GameEnder.gameEnding)
		{
			// Do not spam popups till colony is fixed
			return false;
		}
		else
		{
			this.showedEngamePopup = false;
			return false;
		}
	}

	private void CreateEndgameLetter()
	{
		LetterDef endGameLetterDef = DefDatabase<LetterDef>.GetNamed("GameOverScreen");
		Letter endGameLetter = (ChoiceLetter)LetterMaker.MakeLetter("GameOverScreen",
									    letter_tagged_text,
									    endGameLetterDef);
		Find.LetterStack.ReceiveLetter(endGameLetter);
	}

	public override void GameComponentTick()
	{
		if (Find.TickManager.TicksGame > lastCheckTick + checkFrequency)
		{
			lastCheckTick = Find.TickManager.TicksGame;
			if (this.CheckGameEnding())
			{
				this.CreateEndgameLetter();
			}
		}
	}
}

public class Mod_GameOverScreen : Mod
{
	private Monitor_GameOverScreen monitor;

	public Mod_GameOverScreen(ModContentPack content) : base(content)
	{
		this.monitor = new Monitor_GameOverScreen(Current.Game);
	}
}
