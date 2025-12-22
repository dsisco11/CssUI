using CssUI;
using CssUI.CSS.Media;
using CssUI.DOM;
using CssUI.DOM.Mutation;
using CssUI.DOM.Nodes;

namespace CssUITests.DOM.Mutation;

/// <summary>
/// A minimal test implementation of Screen for unit testing purposes.
/// </summary>
internal class TestScreen : Screen
{
    public override EMediaType MediaType => EMediaType.Screen;
    public override long availWidth => 1920;
    public override long availHeight => 1080;
    public override long width => 1920;
    public override long height => 1080;
}

/// <summary>
/// A minimal test implementation of Window for MutationObserver unit testing.
/// This provides the necessary infrastructure for creating MutationObservers
/// and processing mutation records.
/// </summary>
internal class TestWindow : Window
{
    private static readonly TestScreen _screen = new TestScreen();
    private Point2i _location = Point2i.Zero;
    private Rect2i _size = new Rect2i(800, 600);

    public TestWindow() : base(_screen, "TestDocument")
    {
    }

    #region Abstract Method Implementations

    protected override Point2i Get_Window_Location() => _location;

    protected override Rect2i Get_Window_Size() => _size;

    protected override void Set_Window_Location(Point2i Pos) => _location = Pos;

    protected override void Set_Window_Size(Rect2i Size) => _size = Size;

    #endregion

    /// <summary>
    /// Override to prevent the async microtask from racing with synchronous TakeRecords() calls in tests.
    /// Tests should use ProcessMutationRecords() or TakeRecords() directly to check mutation records.
    /// </summary>
    internal override void QueueObserverMicroTask()
    {
        // No-op: Prevents race condition between async task and synchronous test assertions
    }

    /// <summary>
    /// Synchronously processes all pending mutation records by invoking callbacks.
    /// This is useful for testing since the normal microtask queue is asynchronous.
    /// </summary>
    public void ProcessMutationRecords()
    {
        // Clone the observers list
        var notifySet = Observers.ToArray();

        foreach (var mo in notifySet)
        {
            var records = mo.TakeRecords();
            // Remove transient registered observers
            foreach (Node node in mo.Nodes)
            {
                node.RegisteredObservers.RemoveAll(o => o is TransientRegisteredObserver tro && ReferenceEquals(tro, mo));
            }
            if (records.Count > 0)
            {
                mo.callback?.Invoke(records, mo);
            }
        }
    }
}
