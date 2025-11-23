using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Ksa.Controller;

namespace KittenProtoLink;

internal sealed class TelemetryServer : IDisposable
{
    private readonly TcpListener _listener;
    private readonly ConcurrentDictionary<TcpClient, NetworkStream> _clients = new();
    private readonly CancellationTokenSource _cts = new();
    private Task? _acceptLoop;

    public event Action<ManualControlCommand>? CommandReceived;

    public TelemetryServer(int port = 47011)
    {
        _listener = new TcpListener(IPAddress.Loopback, port);
    }

    public void Start()
    {
        if (_acceptLoop != null) return;
        _listener.Start();
        _acceptLoop = Task.Run(AcceptLoopAsync);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        foreach (var kv in _clients)
        {
            kv.Value.Dispose();
            // kv.Key.Dispose();
        }
    }

    public void Broadcast(Envelope envelope)
    {
        var bytes = envelope.ToByteArray();
        Span<byte> lengthPrefix = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(lengthPrefix, (uint)bytes.Length);
        foreach (var kv in _clients.ToArray())
        {
            var stream = kv.Value;
            try
            {
                stream.Write(lengthPrefix);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
            catch
            {
                RemoveClient(kv.Key);
            }
        }
    }

    private async Task AcceptLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await _listener.AcceptTcpClientAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            var stream = client.GetStream();
            _clients[client] = stream;
            _ = Task.Run(() => ReceiveLoopAsync(client, stream), _cts.Token);
        }
    }

    private async Task ReceiveLoopAsync(TcpClient client, NetworkStream stream)
    {
        var lengthBuffer = new byte[4];
        while (!_cts.IsCancellationRequested)
        {
            if (!await ReadExactAsync(stream, lengthBuffer, _cts.Token))
            {
                break;
            }

            var length = BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
            if (length == 0)
                continue;

            var buffer = ArrayPool<byte>.Shared.Rent((int)length);
            try
            {
                if (!await ReadExactAsync(stream, buffer.AsMemory(0, (int)length), _cts.Token))
                {
                    break;
                }

                var envelope = Envelope.Parser.ParseFrom(buffer, 0, (int)length);
                if (envelope.PayloadCase == Envelope.PayloadOneofCase.Controls)
                {
                    CommandReceived?.Invoke(envelope.Controls);
                }
            }
            catch (InvalidProtocolBufferException)
            {
                // Ignore malformed frames.
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        RemoveClient(client);
    }

    private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken token)
        => await ReadExactAsync(stream, buffer.AsMemory(), token);

    private static async Task<bool> ReadExactAsync(NetworkStream stream, Memory<byte> destination, CancellationToken token)
    {
        var offset = 0;
        while (offset < destination.Length)
        {
            int read = await stream.ReadAsync(destination[offset..], token);
            if (read == 0)
            {
                return false;
            }
            offset += read;
        }
        return true;
    }

    private void RemoveClient(TcpClient client)
    {
        if (_clients.TryRemove(client, out var stream))
        {
            stream.Dispose();
            client.Dispose();
        }
    }
}
