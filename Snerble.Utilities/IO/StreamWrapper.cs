namespace Snerble.Utilities.IO;

public abstract class StreamWrapper(Stream stream) : Stream
{
	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => stream.BeginRead(buffer, offset, count, callback, state);

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => stream.BeginWrite(buffer, offset, count, callback, state);

	public override void Close()
	{
		stream.Close();
		base.Close();
	}

	public override void CopyTo(Stream destination, int bufferSize) => stream.CopyTo(destination, bufferSize);

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => stream.CopyToAsync(destination, bufferSize, cancellationToken);

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			stream.Dispose();
		}
	}

	public override async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);
		await stream.DisposeAsync();
		Dispose(true);
	}

	public override int EndRead(IAsyncResult asyncResult) => stream.EndRead(asyncResult);

	public override void EndWrite(IAsyncResult asyncResult) => stream.EndWrite(asyncResult);

	public override void Flush() => stream.Flush();

	public override Task FlushAsync(CancellationToken cancellationToken) => stream.FlushAsync(cancellationToken);

	public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);

	public override int Read(Span<byte> buffer) => stream.Read(buffer);

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => stream.ReadAsync(buffer, offset, count, cancellationToken);

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => stream.ReadAsync(buffer, cancellationToken);

	public override int ReadByte() => stream.ReadByte();

	public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

	public override void SetLength(long value) => stream.SetLength(value);

	public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);

	public override void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer);

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => stream.WriteAsync(buffer, offset, count, cancellationToken);

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => stream.WriteAsync(buffer, cancellationToken);

	public override void WriteByte(byte value) => stream.WriteByte(value);

	public override bool CanRead => stream.CanRead;

	public override bool CanSeek => stream.CanSeek;

	public override bool CanTimeout => stream.CanTimeout;

	public override bool CanWrite => stream.CanWrite;

	public override long Length => stream.Length;

	public override long Position
	{
		get => stream.Position;
		set => stream.Position = value;
	}

	public override int ReadTimeout
	{
		get => stream.ReadTimeout;
		set => stream.ReadTimeout = value;
	}

	public override int WriteTimeout
	{
		get => stream.WriteTimeout;
		set => stream.WriteTimeout = value;
	}
}
