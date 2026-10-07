using Robust.Shared.Prototypes;

namespace Content.Shared._F14.Pager;

[Prototype("pagerSoftware")]
public sealed partial class PagerSoftwarePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public string Name { get; private set; } = default!;

    [DataField(required: true)]
    public string Description { get; private set; } = default!;

    [DataField(required: true)]
    public int DiskCost { get; private set; }

    [DataField]
    public List<string> RequiredAccess { get; private set; } = new();

    [DataField]
    public string? UnlocksProgram { get; private set; }

    [DataField]
    public string Icon { get; private set; } = "/Textures/_F14/Interface/Pager/Icons/briefcase.png";

    [DataField]
    public bool Core { get; private set; } = false;
}
