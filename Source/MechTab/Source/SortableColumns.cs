using RimWorld;
using System;
using Verse;

namespace SpaceMoth
{
	// Sort helpers shared by the sortable columns. Compare runs O(n log n) times per sort, so these use the pawns'
	// existing label strings instead of building new (colored) ones per comparison.
	internal static class SortUtility
	{
		// Same name the label column shows, without the colored kind suffix (the kind is the tie breaker instead).
		public static string SortLabel(Pawn pawn, PawnColumnDef def)
		{
			if (!pawn.RaceProps.Humanlike && !pawn.RaceProps.Animal && pawn.Name != null && !pawn.Name.Numerical)
				return pawn.Name.ToStringShort;
			return def.useLabelShort ? pawn.LabelShort : pawn.LabelNoCount;
		}

		public static int CompareLabels(Pawn a, Pawn b, PawnColumnDef def)
		{
			int result = string.Compare(SortLabel(a, def), SortLabel(b, def), StringComparison.OrdinalIgnoreCase);
			if (result != 0)
				return result;
			return string.Compare(a.KindLabel, b.KindLabel, StringComparison.OrdinalIgnoreCase);
		}

		// Mechs without an overseer or control group have nothing to compare; they go last.
		public static int CompareNullsLast(object a, object b)
		{
			if (a == null)
				return b == null ? 0 : 1;
			return b == null ? -1 : 0;
		}
	}

	public class SM_PawnColumnWorker_Overseer_Sortable : PawnColumnWorker_Overseer
	{
		public override int Compare(Pawn a, Pawn b)
		{
			Pawn overseerA = a.GetOverseer();
			Pawn overseerB = b.GetOverseer();
			if (overseerA == null || overseerB == null)
				return SortUtility.CompareNullsLast(overseerA, overseerB);
			return SortUtility.CompareLabels(overseerA, overseerB, this.def);
		}
	}

	public class SM_PawnColumnWorker_WorkMode_Sortable : PawnColumnWorker_WorkMode
	{
		public override int Compare(Pawn a, Pawn b)
		{
			MechWorkModeDef modeA = a.GetMechWorkMode();
			MechWorkModeDef modeB = b.GetMechWorkMode();
			if (modeA == null || modeB == null)
				return SortUtility.CompareNullsLast(modeA, modeB);
			return string.Compare(modeA.label, modeB.label, StringComparison.Ordinal);
		}
	}

	public class SM_PawnColumnWorker_ControlGroup_Sortable : PawnColumnWorker_ControlGroup
	{
		public override int Compare(Pawn a, Pawn b)
		{
			MechanitorControlGroup groupA = a.GetMechControlGroup();
			MechanitorControlGroup groupB = b.GetMechControlGroup();
			if (groupA == null || groupB == null)
				return SortUtility.CompareNullsLast(groupA, groupB);
			return groupA.Index.CompareTo(groupB.Index);
		}
	}

	public class SM_PawnColumnWorker_MechLabelWithIcon_Sortable : PawnColumnWorker_Label
	{
		public override int Compare(Pawn a, Pawn b) => SortUtility.CompareLabels(a, b, this.def);
	}
}
