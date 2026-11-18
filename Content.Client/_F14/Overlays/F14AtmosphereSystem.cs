using Robust.Client.Graphics;

namespace Content.Client._F14.Overlays;

public sealed class F14AtmosphereSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;

    private F14AtmosphereOverlay? _overlay;

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new F14AtmosphereOverlay();
        _overlayMan.AddOverlay(_overlay);
    }

    public override void Shutdown()
    {
        base.Shutdown();

        if (_overlay != null)
        {
            _overlayMan.RemoveOverlay(_overlay);
            _overlay = null;
        }
    }
}
