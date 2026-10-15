using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MechTab.Source
{
	public class MechTabModSettings : ModSettings
	{
		public static int maxPriority = 4;
		public static bool showGroupInfo = false;
		public static bool hideNonWorkingMechs = false;

		// buffers
		private static string _maxPriorityBuffer = maxPriority.ToString();

		public MechTabModSettings()
		{

		}

		public static void DoWindowContents(Rect rect)
		{
			Listing_Aligned options = new Listing_Aligned();
			options.Begin(rect);
			options.TextFieldNumericLabeledAligned("SM_MechWork.MaxPriority".Translate(), ref maxPriority, ref _maxPriorityBuffer, 4, 9);

			options.CheckboxLabeled("SM_MechWork.ShowGroupInfo".Translate(), ref showGroupInfo, "SM_MechWork.ShowGroupInfoTip".Translate());

			options.CheckboxLabeled("SM_MechWork.HideNonWorkingMechs".Translate(), ref hideNonWorkingMechs, "SM_MechWork.HideNonWorkingMechsTip".Translate());

			options.End();
		}

		public override void ExposeData()
		{
			Scribe_Values.Look(ref maxPriority, "MaxPriority", 4);
			Scribe_Values.Look(ref showGroupInfo, "ShowGroupInfo", false);
			Scribe_Values.Look(ref hideNonWorkingMechs, "HideNonWorkingMechs", false);
		}
	}

	internal class Listing_Aligned : Listing_Standard
	{

		public void TextFieldNumericLabeledAligned<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
		{
			Rect rect = GetRect(Text.LineHeight);
			if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
			{
				TextFieldNumericLabeledAlignedWidget(rect, label, ref val, ref buffer, min, max);
			}

			Gap(verticalSpacing);
		}


		public static void TextFieldNumericLabeledAlignedWidget<T>(
		  Rect rect,
		  string label,
		  ref T val,
		  ref string buffer,
		  float min = 0.0f,
		  float max = 1E+09f)
		  where T : struct
		{
			Rect rect1 = rect.LeftHalf().Rounded();
			Rect rect2 = rect.RightHalf().Rounded();
			int anchor = (int)Verse.Text.Anchor;
			Verse.Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect1, label);
			ref T local1 = ref val;
			ref string local2 = ref buffer;
			double min1 = (double)min;
			double max1 = (double)max;
			Verse.Text.Anchor = TextAnchor.MiddleRight;
			Widgets.TextFieldNumeric<T>(rect2, ref local1, ref local2, (float)min1, (float)max1);
			Verse.Text.Anchor = (TextAnchor)anchor;
		}



	}
}
