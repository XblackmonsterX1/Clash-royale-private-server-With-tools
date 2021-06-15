﻿using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol
{
    public class ClusterMessage
    {
        public ClusterMessage()
        {
            Writer = Unpooled.Buffer(5);
        }

        public ClusterMessage(IByteBuffer buffer)
        {
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }

        public virtual void Decrypt()
        {
            if (Length <= 0) return;

            var buffer = Reader.ReadBytes(Length);

            Resources.ClusterClient.Rc4.Decrypt(ref buffer);

            Reader = buffer;
            Length = buffer.ReadableBytes;
        }

        public virtual void Encrypt()
        {
            if (Writer.ReadableBytes <= 0) return;

            var buffer = Writer;

            Resources.ClusterClient.Rc4.Encrypt(ref buffer);

            Length = buffer.ReadableBytes;
        }

        public virtual void Decode()
        {
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }

        /// <summary>
        ///     Writes this message to the clients channel
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync()
        {
            try
            {
                await Resources.ClusterClient.Handler.Channel.WriteAndFlushAsync(this);

                Logger.Log($"[C] Message {Id} ({GetType().Name}) sent.", GetType(), ErrorLevel.Debug);
            }
            catch (Exception)
            {
                Logger.Log($"[C] Failed to send {Id}.", GetType(), ErrorLevel.Debug);
            }
        }
    }
}