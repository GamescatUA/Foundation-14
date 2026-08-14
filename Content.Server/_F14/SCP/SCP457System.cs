using Content.Server.Atmos.EntitySystems;
using Content.Server.Temperature.Systems;
using Content.Shared.Damage;
using Content.Shared.Interaction.Events;
using Content.Shared._F14.SCP;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Atmos.Components;
using Robust.Shared.Timing;

namespace Content.Server._F14.SCP;

/// <summary>
/// Server logic for SCP-457:
/// <list type="bullet">
///   <item>Ignition aura — sets nearby entities on fire every pulse.</item>
///   <item>Does NOT raise ambient temperature (no AtmosExposedComponent).</item>
///   <item>3000 HP, immune to fire, weakened by water/foam.</item>
///   <item>Melee attack sets target on fire and applies appearance state.</item>
/// </list>
/// </summary>
///   i realy should start making more summaries and comments
public sealed class SCP457System : EntitySystem
{
    [Dependency] private readonly FlammableSystem _flammable = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly AppearanceSystem _appearance = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SCP457Component, MeleeHitEvent>(OnMeleeHit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SCP457Component, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            comp.IgniteTimer -= frameTime;
            if (comp.IgniteTimer > 0f)
                continue;

            comp.IgniteTimer = comp.IgniteInterval;
            PulseIgniteAura(uid, comp, xform);
        }
    }

    // Ignition aura — sets nearby entities on fire
    private void PulseIgniteAura(EntityUid uid, SCP457Component comp, TransformComponent xform)
    {
        foreach (var target in _lookup.GetEntitiesInRange(uid, comp.IgniteRadius))
        {
            // Don't ignite self 
            if (target == uid || HasComp<SCP457Component>(target))
                continue;

            // Only ignite entities that have a Flammable component
            if (!TryComp<FlammableComponent>(target, out var flammable))
                continue;

            // FlammableSystem updates visual state when igniting, which requires Appearance.
            if (!HasComp<AppearanceComponent>(target))
                continue;

            // seting them on fire
            _flammable.AdjustFireStacks(target, 2f, flammable);
            _flammable.Ignite(target, uid, flammable);

            // Also apply direct heat damage so non-flammable mobs still get hurt.
            var damage = new DamageSpecifier();
            damage.DamageDict["Heat"] = (double) comp.FireDamagePerPulse;
            _damageable.TryChangeDamage(target, damage, ignoreResistances: false, interruptsDoAfters: true, origin: uid);
        }
    }

    // Melee hit — extra ignition on atack
    private void OnMeleeHit(EntityUid uid, SCP457Component comp, MeleeHitEvent args)
    {
        comp.IsAttacking = true;
        _appearance.SetData(uid, SCP457Visuals.Attacking, true);

        foreach (var hit in args.HitEntities)
        {
            if (hit == uid) continue;

            if (TryComp<FlammableComponent>(hit, out var flammable))
            {
                _flammable.AdjustFireStacks(hit, 5f, flammable);
                _flammable.Ignite(hit, uid, flammable);
            }

            _popup.PopupEntity(Loc.GetString("scp457-ignite-hit"), hit, hit, PopupType.LargeCaution);
        }

        Timer.Spawn(500, () =>
        {
            if (!Deleted(uid) && TryComp<SCP457Component>(uid, out var c))
            {
                c.IsAttacking = false;
                _appearance.SetData(uid, SCP457Visuals.Attacking, false);
            }
        });
    }
}
