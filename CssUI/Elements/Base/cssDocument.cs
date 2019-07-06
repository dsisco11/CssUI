
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// CSSUI root container element to which CSS elements may be added.
    /// </summary>
    public class cssDocument : cssRootElement
    {
        public override string TypeName { get { return "Document"; } }

        #region Constructors
        public cssDocument(IRenderEngine Engine) : base(Engine)
        {
            Style.ImplicitRules.Overflow_X.Set(EOverflowMode.Auto);
            Style.ImplicitRules.Overflow_Y.Set(EOverflowMode.Scroll);
        }
        /*
        public Document(GLPanel gl, IRenderEngine Engine) : this(Engine)
        {
            gl.Mouse.MouseUp += Handle_GLPNL_MouseUp;
            gl.Mouse.MouseDown += Handle_GLPNL_MouseDown;
            gl.Mouse.MouseWheel += Handle_GLPNL_MouseWheel;
            gl.Mouse.MouseMove += Handle_GLPNL_MouseMove;
            gl.Mouse.MouseLeave += Handle_GLPNL_MouseLeave;

            gl.Keyboard.KeyPress += Handle_GLPNL_KeyPress;
            gl.Keyboard.KeyUp += Handle_GLPNL_KeyUp;
            gl.Keyboard.KeyDown += Handle_GLPNL_KeyDown;
        }

        public Document(GameWindow gl, IRenderEngine Engine) : this(Engine)
        {
            gl.MouseUp += Handle_GW_MouseUp;
            gl.MouseDown += Handle_GW_MouseDown;
            gl.MouseWheel += Handle_GW_MouseWheel;
            gl.MouseMove += Handle_GW_MouseMove;

            gl.KeyUp += Handle_GW_KeyUp;
            gl.KeyDown += Handle_GW_KeyDown;
            gl.KeyPress += Handle_GW_KeyPress;
        }*/

        #endregion
        
        #region Mouse Handling
        /*EMouseButton? Translate_OpenTK_MouseButton(OpenTK.Input.MouseButton Button)
        {
            switch(Button)
            {
                case OpenTK.Input.MouseButton.Left:
                    return EMouseButton.Left;
                case OpenTK.Input.MouseButton.Middle:
                    return EMouseButton.Middle;
                case OpenTK.Input.MouseButton.Right:
                    return EMouseButton.Right;
                case OpenTK.Input.MouseButton.Button1:
                    return EMouseButton.Backwards;
                case OpenTK.Input.MouseButton.Button2:
                    return EMouseButton.Forward;
                default:
                    return null;
            }
        }*/
        

        #region GLPanel Handlers
        /*
        void Handle_GLPNL_MouseUp (MouseButtonEventArgs e) 
        {
            EMouseButton? Btn = Translate_OpenTK_MouseButton(e.Button);
            if (Btn.HasValue)
            {
                var Args = new PreviewMouseButtonEventArgs(e.X, e.Y, Btn.Value, e.IsPressed);
                Fire_MouseUp(Args);
            }
        }

        void Handle_GLPNL_MouseDown (MouseButtonEventArgs e)
        {
            EMouseButton? Btn = Translate_OpenTK_MouseButton(e.Button);
            if (Btn.HasValue)
            {
                var Args = new PreviewMouseButtonEventArgs(e.X, e.Y, Btn.Value, e.IsPressed);
                Fire_MouseDown(Args);
            }
        }

        void Handle_GLPNL_MouseWheel (MouseWheelEventArgs e) 
        {
            Fire_MouseWheel(new PreviewMouseWheelEventArgs(e.X, e.Y, e.Value, e.Delta));
        }

        void Handle_GLPNL_MouseMove (MouseMoveEventArgs e)
        {
            Fire_MouseMove(new PreviewMouseMoveEventArgs(e.X, e.Y, e.XDelta, e.YDelta));
        }

        void Handle_GLPNL_MouseLeave()
        {
            Fire_MouseLeave();
        }*/
        #endregion

        #region GameWindow Handlers
        /*
        void Handle_GW_MouseUp(object Sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            EMouseButton? Btn = Translate_OpenTK_MouseButton(e.Button);
            if (Btn.HasValue)
            {
                var Args = new PreviewMouseButtonEventArgs(e.X, e.Y, Btn.Value, e.IsPressed);
                Fire_MouseUp(Args);
            }
        }

        void Handle_GW_MouseDown (object Sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            EMouseButton? Btn = Translate_OpenTK_MouseButton(e.Button);
            if (Btn.HasValue)
            {
                var Args = new PreviewMouseButtonEventArgs(e.X, e.Y, Btn.Value, e.IsPressed);
                Fire_MouseDown(Args);
            }
        }

        void Handle_GW_MouseWheel (object Sender, OpenTK.Input.MouseWheelEventArgs e) 
        {
            Fire_MouseWheel(new PreviewMouseWheelEventArgs(e.X, e.Y, e.Value, e.Delta));
        }

        void Handle_GW_MouseMove (object Sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            Fire_MouseMove(new PreviewMouseMoveEventArgs(e.X, e.Y, e.XDelta, e.YDelta));
        }*/
        #endregion

        #endregion

        #region Key Handling

        #region GLPanel
        /*private void Handle_GLPNL_KeyPress(KeyPressEventArgs e)
        {
            Handle_KeyPress(null, e);
        }
        private void Handle_GLPNL_KeyUp(KeyboardKeyEventArgs e)
        {
            Handle_KeyUp(null, e);
        }
        private void Handle_GLPNL_KeyDown(KeyboardKeyEventArgs e)
        {
            Handle_KeyDown(null, e);
        }*/
        #endregion

        #region GameWindow
        /*private void Handle_GW_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Handle_KeyUp(null, new KeyboardKeyEventArgs(e));
        }

        private void Handle_GW_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Handle_KeyDown(null, new KeyboardKeyEventArgs(e));
        }

        private void Handle_GW_KeyPress(object sender, KeyPressEventArgs e)
        {
            Handle_KeyPress(null, e);
        }*/
        #endregion

        #endregion
        
    }
}
