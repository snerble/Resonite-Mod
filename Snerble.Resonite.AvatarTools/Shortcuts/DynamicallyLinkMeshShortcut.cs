using FrooxEngine;
using Snerble.Resonite.AvatarTools.Extensions;

namespace Snerble.Resonite.AvatarTools.Shortcuts;

public class DynamicallyLinkMeshShortcut(SkinnedMeshRenderer component) : IShortcut
{
	private record DynLink
	{
		private const string DynLinkSlotName = "<color=cyan>DynLink</color>";
		private const string DynLinkSlotTag = "DynLink";

		public DynLink(Slot avatar)
		{
			Root = avatar.FindOrAddSlot(x => x.Tag == DynLinkSlotTag, s =>
			{
				s.Name = DynLinkSlotName;
				s.Tag = DynLinkSlotTag;
			});

			VarsRoot = Root.FindOrAddSlot("<color=red>Vars</color>");
			BonesRoot = Root.FindOrAddSlot("Bones");
		}

		public Slot Root { get; }
		public Slot BonesRoot { get; }
		public Slot VarsRoot { get; }

		public void AddBone(SyncRef<Slot> syncRef, Slot fallback)
		{
			var bone = syncRef.Target;
			var varName = $"Avatar/bone_{bone.Name}";

			var dynSlot = BonesRoot.FindOrAddSlot(varName, s =>
			{
				s.AttachComponent<DynamicReferenceVariable<Slot>>(beforeAttach: variable =>
				{
					variable.VariableName.DirectValue = varName;
					variable.Reference.Target = bone;
				});
			});

			var driver = syncRef.DriveFromVariable(varName);
			driver.DefaultTarget.Target = fallback;
		}
	}

	public string Name => "Setup DynamicLink";

	private readonly Slot root = component.Slot.GetObjectRoot();

	public void Run()
	{
		EnsureAvatarDynVarSpace();

		var dynLink = new DynLink(root);

		var copiedArmatureSlot = component.Slot.AddSlot("Static Armature");
		foreach (var syncRef in component.Bones.Elements)
		{
			var bone = syncRef.Target;
			var clone = bone.AddSlot(bone.Name);
			clone.SetParent(copiedArmatureSlot);

			dynLink.AddBone(syncRef, clone);
		}
	}

	private void EnsureAvatarDynVarSpace()
	{
		if (root.GetComponents<DynamicVariableSpace>().Any(x => x.SpaceName.DirectValue == "Avatar")) return;

		root.AttachComponent<DynamicVariableSpace>(true, space => space.SpaceName.DirectValue = "Avatar");
	}
}