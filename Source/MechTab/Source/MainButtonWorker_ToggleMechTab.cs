using RimWorld;

namespace SpaceMoth
{
	// Uses vanilla's mech tab button worker: it is shown only when the current map has player mechs, and caches
	// that check for 180 ticks. The game asks Visible/Disabled several times per UI event, so an uncached check
	// scanned the faction's pawns (and all map pawns when there were no mechs) many times per frame.
	// The class is kept so MainButtonDefs and patches referencing it keep working.
	public class SM_MainButtonWorker_ToggleMechWorkTab : MainButtonWorker_ToggleMechTab
	{
	}
}
