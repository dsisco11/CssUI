namespace CssUI.DOM.Events
{
    public enum EEventPhase : ushort
    {
        /// <summary>
        /// Events not currently dispatched are in this phase.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// When an event is dispatched to an object that participates in a tree it will be in this phase before it reaches its target.
        /// </summary>
        CAPTURING_PHASE = 1,
        /// <summary>
        /// When an event is dispatched it will be in this phase on its target.
        /// </summary>
        AT_TARGET = 2,
        /// <summary>
        /// When an event is dispatched to an object that participates in a tree it will be in this phase after it reaches its target.
        /// </summary>
        BUBBLING_PHASE = 3
    }
}
