using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CssUI.CSS;
using CssUI.Internal;

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
        /// Callback for when any value stage of this property changes
        /// </summary>
        public event Action<ECssPropertyStage, ICssProperty> onValueChange;
        /// <summary>
        /// Tracks which styling rule block this property came from
        /// </summary>
        public WeakReference<CssPropertySet> SourcePtr { get; set; } = null;
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
        /// <summary>
        /// Return TRUE if the assigned value is set to <see cref="CssValue.Auto"/>
        /// </summary>
        public virtual bool IsAuto { get => false; }
        public bool HasValue { get => (Transforms.Count > 0); }
        public bool IsInherited { get => false; }// A transform property CANNOT be inherited.
        /// <summary>
        /// Returns TRUE if this property is inheritable according to its definition
        /// </summary>
        public virtual bool IsInheritable { get => Definition.Inherited; }

        public CssPropertySet Source
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                this.SourcePtr.TryGetTarget(out CssPropertySet src);
                return src;
            }
        }

        public CssPropertyDefinition Definition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (string.IsNullOrEmpty(this.CssName))
                    return null;
                return CssProperties.Definitions[this.CssName];
            }
        }
        #endregion


        #region Cascade
        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public bool Cascade(ICssProperty prop)
        {// Circumvents locking
            TransformListProperty o = prop as TransformListProperty;
            bool changes = false;
            if (o.HasValue)
            {
                changes = true;
                Transforms = new Dictionary<AtomicString, StyleFunction>(o.Transforms);
                this.SourcePtr = o.SourcePtr;
                this.Selector = o.Selector;
            }

            if (changes) Update();
            return changes;
        }

        /// <summary>
        /// Overwrites the values of this instance with any values from another which aren't <see cref="CssValue.Null"/>
        /// </summary>
        /// <returns>Success</returns>
        public async Task<bool> CascadeAsync(ICssProperty prop)
        {
            return await Task.Factory.StartNew(() => Cascade(prop));
        }

        #endregion

        #region Overwrite

        /// <summary>
        /// Overwrites the assigned value of this instance with values from another if they are different
        /// </summary>
        /// <returns>Success</returns>
        public bool Overwrite(ICssProperty prop)
        {
            return false;
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

            if (changes) Update();
            return await Task.FromResult(changes);
        }

        #endregion

        #region Updating
        /// <summary>
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public void Update(bool ComputeNow = false)
        {
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public void UpdateDependent(bool ComputeNow = false)
        {
        }

        /// <summary>
        /// If the Assigned value is one that depends on another value for its final value OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public void UpdateDependentOrAuto(bool ComputeNow = false)
        {
        }

        /// <summary>
        /// If the Assigned value is a percentage OR is <see cref="CssValue.Auto"/> then
        /// Resets all values back to the Assigned and then recomputes them later
        /// </summary>
        /// <param name="ComputeNow">If <c>True</c> the final values will be computed now, In most cases leave this false</param>
        public void UpdatePercentageOrAuto(bool ComputeNow = false)
        {
        }
        #endregion

        #region Unit Resolver
        /// <summary>
        /// Allows external code to notify this property that a certain unit type has changed scale and if we have a value which uses that unit-type we need to fire our Changed event because our Computed value will be different
        /// </summary>
        public void Handle_Unit_Change(EStyleUnit Unit)
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

            if (change) onValueChange?.Invoke(ECssPropertyStage.Computed, this);
        }

        private double Get_Unit_Scale(EStyleUnit Unit)
        {
            return StyleUnitResolver.Get_Scale(Owner, this, Unit);
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
        public TransformListProperty(string CssName, cssElement Owner, WeakReference<CssPropertySet> Source, bool Locked)
        {
            this.CssName = new AtomicString(CssName);
            this.Owner = Owner;
            this.SourcePtr = Source;
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


        #region ToString
        public override string ToString() { throw new NotImplementedException(); }
        #endregion

        #region Serialization
        public string Serialize() { throw new NotImplementedException(); }

        public CssValue Find_Inherited_Value()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
