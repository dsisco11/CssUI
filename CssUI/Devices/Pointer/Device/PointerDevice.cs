using CssUI.DOM.Geometry;
using System.Collections.Generic;

namespace CssUI.Devices
{
    public abstract class PointerDevice
    {/* Docs: https://w3c.github.io/pointerevents/#intro */

        #region Static
        internal static HashSet<PointerDevice> DEVICE_LIST = new HashSet<PointerDevice>();

        public static IReadOnlyCollection<PointerDevice> Get_All()
        {
            return DEVICE_LIST;
        }
        #endregion

        #region Properties
        /// <summary>
        /// A unique identifier for the pointer causing the event. This identifier MUST be unique from all other active pointers in the top-level browsing context (as defined by [HTML5]) at the time. A user agent MAY recycle previously retired values for pointerId from previous active pointers, if necessary.
        /// </summary>
        public readonly long PointerID;
        /// <summary>
        /// If <c>True</c> this device is the "master" pointer
        /// </summary>
        /// Docs: https://w3c.github.io/pointerevents/#the-primary-pointer
        public readonly bool IsPrimary;
        /// <summary>
        /// All of the currently pressed buttons for this pointer device
        /// </summary>
        public readonly EPointerDeviceType Type;
        public EPointerButtonFlags Buttons { get; protected set; }
        public DOMPoint Position { get; protected set; }

        /// <summary>
        /// The width (magnitude on the X axis), in CSS pixels (see [CSS21]), of the contact geometry of the pointer. This value MAY be updated on each event for a given pointer. For inputs that typically lack contact geometry (such as a traditional mouse), and in cases where the actual geometry of the input is not detected by the hardware, the user agent MUST return a default value of 1.
        /// </summary>
        public double Width { get; protected set; }
        /// <summary>
        /// The height (magnitude on the Y axis), in CSS pixels (see [CSS21]), of the contact geometry of the pointer. This value MAY be updated on each event for a given pointer. For inputs that typically lack contact geometry (such as a traditional mouse), and in cases where the actual geometry of the input is not detected by the hardware, the user agent MUST return a default value of 1.
        /// </summary>
        public double Height { get; protected set; }

        /// <summary>
        /// The normalized pressure of the pointer input in the range of [0,1], where 0 and 1 represent the minimum and maximum pressure the hardware is capable of detecting, respectively. For hardware and platforms that do not support pressure, the value MUST be 0.5 when in the active buttons state and 0 otherwise. Note: all pointerup events will have pressure 0.
        /// </summary>
        public double Pressure { get; protected set; }
        /// <summary>
        /// The normalized tangential pressure (also known as barrel pressure), typically set by an additional control (e.g. a finger wheel on an airbrush stylus), of the pointer input in the range of [-1,1], where 0 is the neutral position of the control. Note that some hardware may only support positive values in the range of [0,1]. For hardware and platforms that do not support tangential pressure, the value MUST be 0.
        /// </summary>
        public double TangentalPressure { get; protected set; }
        /// <summary>
        /// The plane angle (in degrees, in the range of [-90,90]) between the Y-Z plane and the plane containing both the transducer (e.g. pen stylus) axis and the Y axis. A positive tiltX is to the right. tiltX can be used along with tiltY to represent the tilt away from the normal of a transducer with the digitizer. For hardware and platforms that do not report tilt, the value MUST be 0.
        /// </summary>
        public double TiltX { get; protected set; }
        /// <summary>
        /// The plane angle (in degrees, in the range of [-90,90]) between the X-Z plane and the plane containing both the transducer (e.g. pen stylus) axis and the X axis. A positive tiltY is towards the user. tiltY can be used along with tiltX to represent the tilt away from the normal of a transducer with the digitizer. For hardware and platforms that do not report tilt, the value MUST be 0.
        /// </summary>
        public double TiltY { get; protected set; }
        /// <summary>
        /// The clockwise rotation (in degrees, in the range of [0,359]) of a transducer (e.g. pen stylus) around its own major axis. For hardware and platforms that do not report twist, the value MUST be 0.
        /// </summary>
        public double Twist { get; protected set; }
        #endregion

        #region Constructors
        public PointerDevice(long pointerID)
        {
            PointerID = pointerID;
            DEVICE_LIST.Add(this);
        }

        ~PointerDevice()
        {
            DEVICE_LIST.Remove(this);
        }
        #endregion

        #region Accessors
        public double X => Position.x;
        public double Y => Position.y;
        #endregion

        #region Geometry
        public DOMRect GetBoundingClientRect()
        {
            return new DOMRect(Position.x, Position.y, Width, Height);
        }
        #endregion


        #region Overrides
        public override int GetHashCode()
        {
            return PointerID.GetHashCode();
        }
        #endregion
    }
}
