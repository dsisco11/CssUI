namespace CssUI;

/// <summary>
/// Defines a contract for objects that update themselves over time.
/// </summary>
public interface ITickable
{
    /// <summary>
    /// Called to update the object based on elapsed time.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last tick, in seconds.</param>
    void OnTicked(double deltaTime);
}
