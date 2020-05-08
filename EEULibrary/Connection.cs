﻿using System;
using System.Threading.Tasks;

namespace EEUniverse.Library
{
    /// <summary>
    /// Provides a default implementation for interacting with the Everybody Edits Universe™ servers
    /// </summary>
    public class Connection : IConnection
    {
        ///// <summary>
        ///// An event that raises when the client receives a message with the assigned scope.
        ///// </summary>
        public event EventHandler<Message> OnMessage;

        private readonly IClient _client;
        private readonly ConnectionScope _scope;

        ///// <summary>
        ///// Creates a new connection handler for the specified scope.
        ///// </summary>
        ///// <param name="client">The underlying client of this connection.</param>
        ///// <param name="scope">The scope of the connection.<br />This is what the connection listens to in the OnMessage EventHandler.</param>
        ///// <param name="worldId">The world ID to connect to.<br />This should only be filled when creating a connection with the 'World' scope.</param>
        public Connection(IClient client, ConnectionScope scope, string worldId = "")
        {
            _client = client;
            _scope = scope;

            client.OnMessage += (s, message) =>
            {
                if (message.Scope != scope)
                    return;

                OnMessage?.Invoke(this, message);
            };

            if (scope == ConnectionScope.World)
            {
                if (string.IsNullOrEmpty(worldId))
                    throw new ArgumentNullException($"{nameof(worldId)} should not be null or empty when the scope is world.");

                ((IConnection)this).Send(new Message(ConnectionScope.Lobby, 0, "world", worldId));
            }
        }

        ///// <summary>
        ///// Sends a message to the server.
        ///// </summary>
        ///// <param name="type">The type of the message.</param>
        ///// <param name="data">An array of data to be sent.</param>
        public void Send(MessageType type, params object[] data) => SendAsync(new Message(_scope, type, data)).GetAwaiter().GetResult();

        ///// <summary>
        ///// Sends a message to the server.
        ///// </summary>
        ///// <param name="message">The message to be sent.</param>
        //public void Send(Message message) => SendRawAsync(Serializer.Serialize(message)).GetAwaiter().GetResult();
        public void Send(Message message) => SendAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();

        ///// <summary>
        ///// Sends an asynchronous message to the server.<br />Use with caution.
        ///// </summary>
        ///// <param name="buffer">The buffer containing the message to be sent.</param>
        //public void SendRaw(ArraySegment<byte> buffer) => SendRawAsync(buffer).GetAwaiter().GetResult();
        //public void SendRaw(ArraySegment<byte> buffer) => SendAsync(buffer).GetAwaiter().GetResult();

        /// <summary>
        /// Sends an asynchronous message to the server.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="data">An array of data to be sent.</param>
        //public Task SendAsync(MessageType type, params object[] data) => SendRawAsync(Serializer.Serialize(new Message(_scope, type, data)));
        public Task SendAsync(MessageType type, params object[] data) => _client.SendAsync(_scope, type, data);

        ///// <summary>
        ///// Sends an asynchronous message to the server.
        ///// </summary>
        ///// <param name="message">The message to be sent.</param>
        //public Task SendAsync(Message message) => SendRawAsync(Serializer.Serialize(message));

        ///// <summary>
        ///// Sends an asynchronous message to the server.<br />Use with caution.
        ///// </summary>
        ///// <param name="buffer">The buffer containing the message to be sent.</param>
        //public Task SendRawAsync(ArraySegment<byte> buffer) => _client.SendRawAsync(buffer.Array);
        //public Task SendRawAsync(byte[] buffer) => _client.SendRawAsync(buffer);
        public Task SendAsync(Message message) => _client.SendAsync(message);
    }
}