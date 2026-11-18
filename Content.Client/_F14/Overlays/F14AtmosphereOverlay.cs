using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Client._F14.Overlays;

public sealed class F14AtmosphereOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    private readonly ShaderInstance _shader;

    public F14AtmosphereOverlay()
    {
        IoCManager.InjectDependencies(this);
        _shader = _proto.Index<ShaderPrototype>("F14DepressiveSCP").InstanceUnique();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;
        _shader.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        handle.UseShader(_shader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
