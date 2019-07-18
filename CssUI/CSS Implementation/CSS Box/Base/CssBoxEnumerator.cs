using System;
using System.Collections;
using System.Collections.Generic;

namespace CssUI.CSS
{
    public class CSSBoxEnumerator : IEnumerator<CssBoxFragment>
    {

        #region Proeprties
        private CssPrincipalBox Box;
        private cssCompoundElement Owner;
        private int index = -1;
        #endregion

        #region Constructor
        public CSSBoxEnumerator(CssPrincipalBox Box, cssCompoundElement Owner)
        {
            this.Box = Box;
            this.Owner = Owner;
        }
        #endregion
        public CssBoxFragment Current
        {
            get
            {
                if (index <= -1) throw new InvalidOperationException();
                return Box[index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (index <= -1) throw new InvalidOperationException();
                return Box[index];
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            index++;
            if (index < Box.Count)
            {
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
