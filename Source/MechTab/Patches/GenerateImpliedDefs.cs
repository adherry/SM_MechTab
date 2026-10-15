using System;
using System.Collections.Generic;
using System.Linq;
using MechTab.Source;
using RimWorld;
using Verse;

namespace SpaceMoth
{
	public class DefGenerator_GenerateImpliedDefs_MechsWork
	{
		private static bool m_Generated = false;

		// Work types that got a column in the mech work table.
		public static HashSet<WorkTypeDef> ColumnWorkTypes = new HashSet<WorkTypeDef>();

		public static void Generate()
		{
			if (m_Generated == false)
			{
				m_Generated = true;

				foreach (PawnColumnDef impliedPawnColumnDef in ImpliedMechColumnDefs(false))
					DefGenerator.AddImpliedDef<PawnColumnDef>(impliedPawnColumnDef, false);
			}
		}

		private static IEnumerable<PawnColumnDef> ImpliedMechColumnDefs(bool hotReload)
		{
			PawnTableDef workTable = (MechTabModSettings.showGroupInfo == true) ? DefDatabase<PawnTableDef>.GetNamed("SM_PawnTable_MechsWork") : DefDatabase<PawnTableDef>.GetNamed("SM_PawnTable_MechsWorkSlim");
			bool moveWorkTypeLabelDown = false;
			foreach (WorkTypeDef workTypeDef in WorkTypeDefsUtility.WorkTypeDefsInPriorityOrder.Where<WorkTypeDef>((Func<WorkTypeDef, bool>)(d => d.visible)).Reverse<WorkTypeDef>())
			{
				if (CheckWorkTypeMechValid(workTypeDef) == true)
				{
					ColumnWorkTypes.Add(workTypeDef);
					moveWorkTypeLabelDown = !moveWorkTypeLabelDown;
					string defName = "MechWorkPriority_" + workTypeDef.defName;
					PawnColumnDef pawnColumnDef = hotReload ? DefDatabase<PawnColumnDef>.GetNamed(defName, false) ?? new PawnColumnDef() : new PawnColumnDef();
					pawnColumnDef.defName = defName;
					pawnColumnDef.workType = workTypeDef;
					pawnColumnDef.moveWorkTypeLabelDown = moveWorkTypeLabelDown;
					pawnColumnDef.workerClass = typeof(SM_PawnColumnWorker_WorkPriority);
					pawnColumnDef.sortable = true;
					pawnColumnDef.modContentPack = workTypeDef.modContentPack;
					workTable.columns.Insert(workTable.columns.FindIndex((Predicate<PawnColumnDef>)(x => x.Worker is PawnColumnWorker_CopyPasteWorkPriorities)) + 1, pawnColumnDef);
					yield return pawnColumnDef;
				}
			}
		}

		private static bool CheckWorkTypeMechValid(WorkTypeDef workTypeDef)
		{
			bool RetVal = false;
			
			// Check if any mechs can do the work even if the work type has no actual work mechs can do. (Think Drug making is the only example under reaserch)
			IEnumerable<PawnKindDef> PawnTypesList = DefDatabase<PawnKindDef>.AllDefs.Where<PawnKindDef>((Func< PawnKindDef, bool>)(pk => pk.race != null && pk.RaceProps.IsMechanoid == true));

			foreach (PawnKindDef MechPawn in PawnTypesList)
			{
				if (MechPawn.RaceProps.mechEnabledWorkTypes.Contains(workTypeDef))
				{
					RetVal = true;
					break;
				}
			}


			if (RetVal == false)
			{
				// Check all the work actions done in a worktype and ensure a mech can do it.
				IEnumerable<WorkGiverDef> WorkTypesList = workTypeDef.workGiversByPriority.Where<WorkGiverDef>((Func<WorkGiverDef, bool>)(wg => wg.canBeDoneByMechs));
				if (WorkTypesList.Any<WorkGiverDef>())
				{
					Log.Message("WorkType " + workTypeDef.defName + " can be done by mechs, but no mechs are capable");
				}
			}

			return RetVal;
		}

	}
}
