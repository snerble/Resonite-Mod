namespace Snerble.Utilities;

public class ValueChangedEventArgs<T>(T? Old, T Value) : EventArgs;

public interface INotifyValueChanged<T>
{
	event EventHandler<ValueChangedEventArgs<T>> Changed;
}

public class Observable<T> : INotifyValueChanged<T>
{
#pragma warning disable IDE0290
	// ReSharper disable once ConvertToPrimaryConstructor
	public Observable(T value) => _value = value;
#pragma warning restore IDE0290

	private T _value;
	public T Value
	{
		get => _value;
		set
		{
			var old = _value;
			SetValueWithoutNotify(value);
			Changed?.Invoke(this, new(old, value));
		}
	}

	public event EventHandler<ValueChangedEventArgs<T>>? Changed;

	protected void SetValueWithoutNotify(T value) => _value = value;
}
