using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Provides scale resolution for StyleValue unit types
    /// </summary>
    public static class StyleUnitResolver
    {

        const double Ratio_DegToRad = (Math.PI / 180.0);
        const double Ratio_GradToRad = (Math.PI / 200.0);
        const double Ratio_TurnToRad = (Math.PI / 0.5);
        public static double Get_Scale(cssElement Owner, EStyleUnit Unit)
        {
            switch (Unit)
            {
                case EStyleUnit.EM:
                    {
                        if (Owner.Font != null) return Owner.Font.EmSize;
                        else return Owner.Style.FontSize;
                    }
                case EStyleUnit.CH:
                    return 1.0;// TODO: Implement a system-independent 'FontMetrics' class for getting this kind of info...
                case EStyleUnit.EX:
                    //return Owner.Font.
                    return 1.0;// TODO: Implement a system-independent 'FontMetrics' class for getting this kind of info...
                case EStyleUnit.REM:
                    return Owner.Root.Style.FontSize;
                case EStyleUnit.VMAX:
                    return Math.Max(Owner.Root.Get_Viewport().Block.Width, Owner.Root.Get_Viewport().Block.Height);
                case EStyleUnit.VMIN:
                    return Math.Min(Owner.Root.Get_Viewport().Block.Width, Owner.Root.Get_Viewport().Block.Height);
                case EStyleUnit.VW:
                    return Owner.Root.Get_Viewport().Block.Width;
                case EStyleUnit.VH:
                    return Owner.Root.Get_Viewport().Block.Height;
                case EStyleUnit.PX:
                    return 1.0;
                case EStyleUnit.DEG:// Translate degrees to radians
                    return Ratio_DegToRad;
                case EStyleUnit.GRAD:
                    return Ratio_GradToRad;
                case EStyleUnit.RAD:
                    return 1.0;
                case EStyleUnit.TURN:
                    return Ratio_TurnToRad;
                default:
                    return 1.0;
            }
        }
    }
}
