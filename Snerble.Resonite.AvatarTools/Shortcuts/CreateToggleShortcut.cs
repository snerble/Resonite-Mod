using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using ProtoFlux.Runtimes.Execution.Nodes.Operators;
using Snerble.ResoAvatarTools;
using Snerble.Resonite.AvatarTools.Extensions;

namespace Snerble.Resonite.AvatarTools.Shortcuts;

internal class CreateToggleShortcut(Sync<bool> component) : IShortcut
{
	public string Name => $"Create toggle: {component.Name}";

	public void Run()
	{
		var boolean = component;
		var root = boolean.Slot;
		var name = root.Name;

		var stateSlot = root.AddSlot($"<color=red>{name}</color>");
		var vf = stateSlot.AttachComponent<ValueField<bool>>(beforeAttach: field => field.Value.DirectValue = boolean);
		var vmd = stateSlot.AttachComponent<ValueMultiDriver<bool>>(beforeAttach: driver =>
		{
			var drive = driver.Drives.Add();
			drive.Target = boolean;
		});
		stateSlot.AttachComponent<MultiBoolConditionDriver>(beforeAttach: driver =>
		{
			driver.Mode.DirectValue = MultiBoolConditionDriver.ConditionMode.All;
			driver.Target.Target = vmd.Value;
			var condition = driver.Conditions.Add();
			condition.Field.Target = vf.Value;
		});


		var menuSlot = root.AddSlot(name);
		menuSlot.AttachComponent<ButtonToggle>(beforeAttach: toggle => toggle.TargetValue.Target = vf.Value);
		var cmis = menuSlot.AttachComponent<ContextMenuItemSource>();
		var vodd = menuSlot.AttachComponent<ValueOptionDescriptionDriver<bool>>(beforeAttach: driver =>
		{
			driver.Value.Target = boolean;

			driver.DefaultOption.Color.DirectValue = colorX.Red;
			driver.DefaultOption.Label.DirectValue = $"<color=red>{name}";
			driver.DefaultOption.Sprite.Target = null!;

			var option = driver.Options.Add();
			option.ReferenceValue.DirectValue = true;
			option.Color.DirectValue = colorX.Green;
			option.Label.DirectValue = $"<color=green>{name}";
			option.Sprite.Target = null!;
		});

		vodd.Label.Target = cmis.Label;
		vodd.Color.Target = cmis.Color;
		vodd.Sprite.Target = cmis.Sprite;
	}
}

internal class ToastShortcut(Sync<string> sync) : IShortcut
{
    public string Name => "Display toast";

    public void Run()
    {
        var userRoot = sync.Slot.LocalUserRoot;
        var user = userRoot.ActiveUser;
        var userSlot = userRoot.Slot;
        var world = userSlot.World;

        var root = userSlot.AddSlot("Toast", false);
        var text = root.AddSlot("Text", false);

        text.AttachComponent<TextRenderer>(renderer =>
        {
            renderer.Text.Value = sync.Value;
        });

        var pos = root.AttachComponent<PositionAtUser>(false, pos =>
        {
            // pos.TargetPositionOffset.Value = new(0f, 0.66f, 1f);
            pos.TargetPositionOffset.Value = new(0f, 0f, 1f);
            pos.PositionSource.Value = UserRoot.UserNode.GroundProjectedHead;
            pos.RotationSource.Value = UserRoot.UserNode.GroundProjectedHead;
            pos.TargetUser.Target = user;
            pos.PositionDrive.Target = root.Position_Field;
        });

        root.AttachComponent<LookAtUser>(look =>
        {
            look.TargetUser.Target = user;
            look.Invert.Value = true;
        });

        text.AttachComponent<SmoothTransform>(smooth =>
        {
            smooth.InterpolationSpace.Default.Value = RootSpace.DefaultSpace.LocalUserRoot;
            smooth.SmoothSpeed.Value = 6f;
        });

        root.StartTask(async () =>
        {
            await pos.TargetPositionOffset.TweenTo(new(0f, 1f, 1f), 0.5f, CurvePreset.CubicOut).AsTask();
            await root.DelaySeconds(5);
            await pos.TargetPositionOffset.TweenTo(new(0f, 0f, 1f), 0.5f, CurvePreset.CubicOut).AsTask();
            root.Destroy();
        });
    }
}