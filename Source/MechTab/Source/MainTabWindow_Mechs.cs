using MechTab.Source;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SpaceMoth
{
	// Note: This code is based off of the code in MainTabWindow_MechsWork within Rimworld itself
	public class SM_MainTabWindow_MechsWork : MainTabWindow_PawnTable
	{
		private const int SpaceBetweenPriorityArrowsAndWorkLabels = 40;
		protected override float ExtraTopSpace => 40f;

		protected override PawnTableDef PawnTableDef => (MechTabModSettings.showGroupInfo == true)?DefDatabase<PawnTableDef>.GetNamed("SM_PawnTable_MechsWork"): DefDatabase<PawnTableDef>.GetNamed("SM_PawnTable_MechsWorkSlim");

		protected override IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer).Where<Pawn>((Func<Pawn, bool>)(p => p.RaceProps.IsMechanoid && p.OverseerSubject != null && (MechTabModSettings.hideNonWorkingMechs == false || HasWorkColumn(p))));

		// True if the mech can do at least one work type that has a column in this tab.
		// Combat mechs (e.g. Lancer) have no work types; some modded mechs only have hidden ones.
		private static bool HasWorkColumn(Pawn pawn)
		{
			List<WorkTypeDef> workTypes = pawn.RaceProps.mechEnabledWorkTypes;
			if (workTypes == null)
				return false;

			for (int index = 0; index < workTypes.Count; ++index)
			{
				if (DefGenerator_GenerateImpliedDefs_MechsWork.ColumnWorkTypes.Contains(workTypes[index]))
					return true;
			}
			return false;
		}

		public SM_MainTabWindow_MechsWork()
		{
			DefGenerator_GenerateImpliedDefs_MechsWork.Generate();
		}

		// Table def the current table was built from. The game keeps this window (and its table) for the whole session.
		private PawnTableDef builtTableDef;

		public override void PreOpen()
		{
			// "Show Group Columns" changed since the table was built: rebuild it with the other layout.
			if (builtTableDef != null && builtTableDef != PawnTableDef)
				Notify_ResolutionChanged();
			builtTableDef = PawnTableDef;
			base.PreOpen();
		}

		public override void DoWindowContents(Rect rect)
		{
			if (!ModLister.BiotechInstalled)
				return;

			base.DoWindowContents(rect);
			
			if (Event.current.type == EventType.Layout)
				return;

			this.DoManualPrioritiesCheckbox();

			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Text.Anchor = TextAnchor.UpperCenter;
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(570f, rect.y + 5f, 160f, 30f), "<= " + "HigherPriority".Translate());
			Widgets.Label(new Rect(830f, rect.y + 5f, 160f, 30f), "LowerPriority".Translate() + " =>");
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		
		private void DoManualPrioritiesCheckbox()
		{
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect = new Rect(5f, 5f, 140f, 30f);
			int num1 = Current.Game.playSettings.useWorkPriorities ? 1 : 0;
			Widgets.CheckboxLabeled(rect, (string)"ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities);
			int num2 = Current.Game.playSettings.useWorkPriorities ? 1 : 0;
			if (num1 != num2)
			{
				// Same as vanilla: the setting is global, so notify every player pawn, not only the mechs listed in this tab.
				foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
				{
					if (pawn.Faction == Faction.OfPlayer && pawn.workSettings != null)
						pawn.workSettings.Notify_UseWorkPrioritiesChanged();
				}
			}
			if (Current.Game.playSettings.useWorkPriorities)
			{
				using (new TextBlock(new Color(1f, 1f, 1f, 0.5f)))
					Widgets.Label(new Rect(rect.x, rect.yMax - 6f, rect.width, 60f), "PriorityOneDoneFirst".Translate());
			}
			if (Current.Game.playSettings.useWorkPriorities)
				return;
			UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
		}
	}
}
