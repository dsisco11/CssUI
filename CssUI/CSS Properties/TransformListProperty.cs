using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Represents a property which is a collection of multiple transform function properties
    /// </summary>
    public class TransformListProperty : ICssProperty
    {
        #region Properties
        /// <summary>
        /// Maps a list of transform function names to the list of their arguments
        /// </summary>
        private Dictionary<AtomicString, StyleFunction> Transforms = new Dictionary<AtomicString, StyleFunction>();

        /// <summary>
        /// The UI element which contains this property
        /// </summary>
        public cssElement Owner { get; protected set; } = null;
        /// <summary>
        /// The propertys field-name in whatever class is holding it.
        /// </summary>
        public AtomicString FieldName { get; set; } = null;
        /// <summary>
        /// The propertys identifier token in stylesheets.
        /// </summary>
        public AtomicString CssName { get; protected set; } = null;
        /// <summary>
        /// Callback for when the computed value of this property changes
        /// </summary>
        public event PropertyChangeDelegate onChanged;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public WeakReference<CssPropertySet> Source { get; set; } = null;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public CssSelector Selector { get; set; } = null;

        /// <summary>
        /// If true then this propertys values cannot be set externally
        /// </summary>
        public readonly bool Locked = false;
        #endregion

        #region Accessors
        public bool HasValue { get { return (Transforms.Count > 0); } }
        public bool IsInherited { get { return false; } }// This property CANNOT be inherited.
        #endregion


        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Non-asynchronous methods are now obsolete, please use CascadeAsync instead.")]
        public bool Cascade(ICssProperty prop)
        {// Circumvents locking
            TransformListProperty o = prop as TransformListProperty;
            bool changes = false;
            if (o.HasValue)
            {
                changes = true;
                Transforms = new Dictionary<AtomicString, StyleFunction>(o.Transforms);
                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);
            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {// Circumvents locking
            TransformListProperty o = prop as TransformListProperty;
            bool changes = false;
            if (o.HasValue)
            {
                changes = true;
                Transforms = new Dictionary<AtomicString, StyleFunction>(o.Transforms);
                this.Source = o.Source;
                this.Selector = o.Selector;
            }

            if (changes) onChanged?.Invoke(this);
            return await Task.FromResult(changes);
        }

        #endregion

        #region Overwrite
        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Non-asynchronous methods are now obsolete, please use OverwriteAsync instead.")]
        public bool Overwrite(ICssProperty prop)
        {// Circumvents locking
            // TODO: Finish the overwrite logic for TransformsList properties

            TransformListProperty o = prop as TransformListProperty;
            bool changes = false;
            /*
            if (o.HasValue)
            {
                // Check if any of the functions present in the other property 
                foreach(StyleFunction func in o.Transforms.Values)
                {
                    if (!Transforms.ContainsKey(func.Name))
                    {
                        changes = true;
                        break;
                    }
                }

                if (!changes)
                {
                    foreach (StyleFunction func in Transforms.Values)
                    {
                        if (!o.Transforms.ContainsKey(func.Name))
                        {
                            changes = true;
                            break;
                        }
                    }
                }

                if (changes)
                {
                    Transforms = new Dictionary<AtomicString, StyleFunction>(o.Transforms);
                    this.Source = o.Source;
                    this.Selector = o.Selector;
                }
            }

            if (changes) onChanged?.Invoke(this);
            return changes;
            */
            if (o.HasValue || this.HasValue != o.HasValue)
            {
                changes = true;
            }

            //onChanged?.Invoke(this);
            //return true;
            if (changes) onChanged?.Invoke(this);
            return changes;
        }

        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> OverwriteAsync(ICssProperty prop)
        {// Circumvents locking

            TransformListProperty o = prop as TransformListProperty;
            bool changes = false;

            // XXX: Finish the overwrite logic for TransformsList properties
            /*
            if (o.HasValue)
            {
                // Check if any of the functions present in the other property 
                foreach(StyleFunction func in o.Transforms.Values)
                {
                    if (!Transforms.ContainsKey(func.Name))
                    {
                        changes = true;
                        break;
                    }
                }

                if (!changes)
                {
                    foreach (StyleFunction func in Transforms.Values)
                    {
                        if (!o.Transforms.ContainsKey(func.Name))
                        {
                            changes = true;
                            break;
                        }
                    }
                }

                if (changes)
                {
                    Transforms = new Dictionary<AtomicString, StyleFunction>(o.Transforms);
                    this.Source = o.Source;
                    this.Selector = o.Selector;
                }
            }

            if (changes) onChanged?.Invoke(this);
            return changes;
            */
            if (o.HasValue || this.HasValue != o.HasValue)
            {
                changes = true;
            }

            if (changes) onChanged?.Invoke(this);
            return await Task.FromResult(changes);
        }

        #endregion


        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Notify_Unit_Change(EStyleUnit Unit)
        {
            bool change = false;
            foreach(StyleFunction func in Transforms.Values)
            {
                foreach(CssValue arg in func.Args)
                {
                    if (arg.Unit == Unit)
                    {
                        change = true;
                        break;
                    }
                }
            }

            if (change) onChanged?.Invoke(this);
        }

        private double Get_Unit_Scale(EStyleUnit Unit)
        {
            return StyleUnitResolver.Get_Scale(Owner, Unit);
        }
        #endregion


        #region Accessors
        public StyleFunction[] Get_All()
        {
            return Transforms.Values.ToArray();
        }

        public StyleFunction this[AtomicString Name]
        {
            get
            {
                StyleFunction func;
                if (Transforms.TryGetValue(Name, out func)) return func;

                return null;
            }
            set
            {
                if (Locked) throw new Exception("Cannoy modify value of a locked property!");
                Transforms[Name] = value;
            }
        }
        #endregion



        #region Constructors
        // TODO: Finish the logic for when 'Unset' = TRUE
        public TransformListProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked, bool Unset)
        {
            this.CssName = new AtomicString(CssName);
            this.Owner = Owner;
            this.Source = Source;
            this.Locked = Locked;
        }
        #endregion

        public void Set(params StyleFunction[] Args)
        {
            if (Locked) throw new Exception("Cannoy modify value of a locked property!");
            this.Transforms.Clear();
            foreach (StyleFunction func in Args)
            {
                this.Transforms[func.Name] = func;
            }
        }

    }
}
