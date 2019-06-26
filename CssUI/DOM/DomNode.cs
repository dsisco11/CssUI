using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CssUI.DOM
{
    /// <summary>
    /// Node in a DOM tree hierarchy
    /// Implements node attributes and functions to modify them
    /// Implements common DOM attributes
    /// Defines all UI events and handlers
    /// </summary>
    public abstract class DomNode : IDomEvents
    {
        #region Events
        public event Action<cssElement, DomMouseButtonEventArgs> MouseUp;
        public event Action<cssElement, DomMouseButtonEventArgs> MouseDown;
        public event Action<cssElement, DomMouseWheelEventArgs> MouseWheel;
        public event Action<cssElement, DomRoutedEventArgs> Clicked;
        public event Action<cssElement, DomRoutedEventArgs> DoubleClicked;
        public event Action<cssElement, DomMouseButtonEventArgs> MouseClick;
        public event Action<cssElement, DomMouseButtonEventArgs> MouseDoubleClick;
        public event Action<cssElement, DomMouseMoveEventArgs> MouseMove;

        public event Action<cssElement> MouseHover;
        public event Action<cssElement> MouseEnter;
        public event Action<cssElement> MouseLeave;

        public event Action<cssElement, DomCancellableEvent<DomKeyboardKeyEventArgs>> KeyPress;
        public event Action<cssElement, DomKeyboardKeyEventArgs> KeyUp;
        public event Action<cssElement, DomKeyboardKeyEventArgs> KeyDown;
        #endregion

        #region Attributes
        private Dictionary<string, object> Attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Attempts to set the value of a specified DOM element attribute, returning whether or not the operation succeeded
        /// </summary>
        /// <param name="Attrib">Name of the attribute to set</param>
        /// <param name="Value">Value to set</param>
        /// <returns>Success</returns>
        public bool Set_Attribute(string Attrib, object Value)
        {
            bool result = false;
            object old;
            if (Attributes.TryGetValue(Attrib, out old))
            {
                if (!object.Equals(old, Value))
                    result = true;
            }
            else
            {
                result = true;
            }
            Attributes[Attrib] = Value;

            if (result)
            {
                // XXX: check if any of our style rules concern this attribute and if so recascade rules from the stylesheet and update our Style accordingly
            }
            return true;
        }

        /// <summary>
        /// Attempts to set the value of a specified DOM element attribute, returning whether or not the operation succeeded
        /// </summary>
        /// <param name="Attrib">Name of the attribute to set</param>
        /// <param name="Value">Value to set</param>
        /// <returns>Success</returns>
        public async Task<bool> Set_Attribute_Async(string Attrib, object Value)
        {
            return await Task.Factory.StartNew(() => { return this.Set_Attribute(Attrib, Value); });
        }

        /// <summary>
        /// Returns the value of a specified attribute
        /// </summary>
        /// <param name="Attrib">Name of the attribute to get</param>
        public object Get_Attribute(string Attrib)
        {
            object Value;
            if (Attributes.TryGetValue(Attrib, out Value))
            {
                return Value;
            }

            return null;
        }

        /// <summary>
        /// Returns the value of a specified attribute
        /// </summary>
        /// <param name="Attrib">Name of the attribute to get</param>
        public Ty Get_Attribute<Ty>(string Attrib)
        {
            object Value;
            if (Attributes.TryGetValue(Attrib, out Value))
            {
                return (Ty)Value;
            }

            return default(Ty);
        }

        /// <summary>
        /// Returns whether or not a specified attribute is present
        /// </summary>
        /// <param name="Attrib">Name of the attribute to check</param>
        public bool Has_Attribute(string Attrib)
        {
            return Attributes.ContainsKey(Attrib);
        }

        /// <summary>
        /// Attempts to remove a specified attribute, returning whether or not the operation succeeded
        /// </summary>
        /// <param name="Attrib">Name of the attribute to clear</param>
        /// <returns>Success</returns>
        public bool Clear_Attribute(string Attrib)
        {
            bool result = Attributes.Remove(Attrib);
            if (result)
            {// Attribute changed (was removed)
                // TODO: recascade rules from the stylesheet and update our Style accordingly
            }

            return result;
        }

        #endregion

        #region ID
        /// <summary>
        /// functionally identical to an elements "ID" in HTML, a UNIQUE identifier for the element
        /// </summary>
        public string ID { get { return Get_Attribute<string>("id"); } set { Set_Attribute("id", value); } }
        #endregion

        #region Classes
        /// <summary>
        /// Adds a styling class to the element
        /// </summary>
        /// <returns>Success</returns>
        public bool Add_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            var Classes = new List<string>(Get_Attribute<string>("class")?.Split(' '));
            Classes.Add(ClassName);
            return Set_Attribute("class", string.Join(" ", Classes));
        }

        /// <summary>
        /// Removes a styling class from the element
        /// </summary>
        /// <returns>Success</returns>
        public bool Remove_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            var Classes = new List<string>(Get_Attribute<string>("class")?.Split(' '));
            Classes.Remove(ClassName);
            return Set_Attribute("class", string.Join(" ", Classes));
        }

        /// <summary>
        /// Returns whether the element is assigned the specified styling class
        /// </summary>
        /// <returns></returns>
        public bool Has_Class(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName)) return false;
            return new List<string>(Get_Attribute<string>("class")?.Split(' ')).Contains(ClassName);
        }
        #endregion

        #region DOM Event Handlers
        #endregion


    }
}
