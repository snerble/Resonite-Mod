using Elements.Core;

namespace Snerble.Resonite.AvatarTools.Shortcuts;

public interface IShortcut
{
	string Name { get; }
	colorX Color => colorX.White;

	void Run();
}