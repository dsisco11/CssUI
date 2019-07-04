using System;
using System.Collections;
using System.Collections.Generic;

namespace CssUI
{
    public class CssBoxEnumerator : IEnumerator<CssBox>
    {

        #region Proeprties
        private CssBox Box;
        private cssCompoundElement Owner;
        private int index = -1;
        #endregion

        #region Constructor
        public CssBoxEnumerator(CssBox Box, cssCompoundElement Owner)
        {
            this.Box = Box;
            this.Owner = Owner;
        }
        #endregion
        public CssBox Current
        {
            get
            {
                if (index <= -1) throw new InvalidOperationException();
                return Owner[index].Box;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (index <= -1) throw new InvalidOperationException();
                return Owner[index].Box;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            index++;
            if (index < Owner.Count)
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
