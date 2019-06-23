using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public enum cPosDir { NONE = 0, ABOVE, BELOW, LEFT, RIGHT, TOP_OF, BOTTOM_OF, LEFT_SIDE_OF, RIGHT_SIDE_OF, CENTER_X, CENTER_Y }

    /// <summary>
    /// Applys positioning logic to a control everytime it's Layout is performed
    /// </summary>
    public class uiAligner
    {
        #region Values
        private cPosDir Dir;
        private int Offset = 0;
        private WeakReference<uiElement> Owner, Target;
        #endregion

        #region Constructors
        public uiAligner(uiElement Owner, uiElement Target, int Offset, cPosDir Dir)
        {
            this.Owner = new WeakReference<uiElement>(Owner);
            this.Target = new WeakReference<uiElement>(Target);
            this.Offset = Offset;
            this.Dir = Dir;

            if (Target != null) Target.onMoved += onTargetMoved;
        }

        ~uiAligner()
        {
            Unhook();
        }
        #endregion
        /// <summary>
        /// Unhooks the aligner from the control it is assigned to.
        /// </summary>
        public void Unhook()
        {
            if (Target != null)
            {
                uiElement targ;
                if (Target.TryGetTarget(out targ))
                {
                    targ.onMoved -= onTargetMoved;
                }
            }
        }

        uiElement Get_Owner()
        {
            uiElement C = null;
            if (Owner == null) return null;
            if (!Owner.TryGetTarget(out C)) return null;

            return C;
        }

        uiElement Get_Target()
        {
            uiElement C = null;
            if (Target == null) return null;
            if (!Target.TryGetTarget(out C)) return null;

            return C;
        }

        void onTargetMoved(uiElement Target, ePos oPos, ePos nPos)
        {
            uiElement Owner = Get_Owner();
            if (Owner == null) throw new ArgumentNullException("Owning control has dissapeared!");

            /*
            switch (Dir)
            {
                case cPosDir.ABOVE:
                    Owner.moveAbove(Target, Offset);
                    break;
                case cPosDir.BELOW:
                    Owner.moveBelow(Target, Offset);
                    break;
                case cPosDir.LEFT:
                    Owner.moveLeftOf(Target, Offset);
                    break;
                case cPosDir.RIGHT:
                    Owner.moveRightOf(Target, Offset);
                    break;
            }
            */
        }

        public void Apply_Relative()
        {
            uiElement Owner = Get_Owner();
            if (Owner == null) throw new ArgumentNullException("Owning control has dissapeared!");

            /*
            switch (Dir)
            {
                case cPosDir.TOP_OF:
                    Owner.alignTop(Offset);
                    break;
                case cPosDir.BOTTOM_OF:
                    Owner.alignBottom(Offset);
                    break;
                case cPosDir.LEFT_SIDE_OF:
                    Owner.alignLeftSide(Offset);
                    break;
                case cPosDir.RIGHT_SIDE_OF:
                    Owner.alignRightSide(Offset);
                    break;
                case cPosDir.CENTER_X:
                    Owner.CenterHorizontally();
                    break;
                case cPosDir.CENTER_Y:
                    Owner.CenterVertically();
                    break;
            }
            */
        }

        public bool Equals(uiElement targ, float off, cPosDir d)
        {
            return (Get_Target() == targ && MathExt.floatEq(this.Offset, off) && this.Dir == d);
        }

        public override string ToString()
        {
            return string.Format("[" + nameof(uiAligner) + "] DIR: {0}  OFFSET: {1}  TARGET: {2}", Enum.GetName(typeof(cPosDir), Dir), Offset, Target);
        }
    }

}
