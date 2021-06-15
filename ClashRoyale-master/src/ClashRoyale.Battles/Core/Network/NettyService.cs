﻿using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core.Network.Handlers;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ClashRoyale.Battles.Core.Network
{
    public class NettyService
    {
        public MultithreadEventLoopGroup Group { get; set; }
        public Bootstrap Bootstrap { get; set; }

        public async Task RunServerAsync()
        {
            Group = new MultithreadEventLoopGroup();

            Bootstrap = new Bootstrap();
            Bootstrap.Group(Group);
            Bootstrap.Channel<SocketDatagramChannel>();

            Bootstrap
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new LoggingHandler("SRV-ICR"))
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("PacketHandler", new PacketHandler());
                }));

            var boundChannel = await Bootstrap.BindAsync(Resources.Configuration.ServerPort);
            var endpoint = (IPEndPoint) boundChannel.LocalAddress;

            Logger.Log(
                $"Listening on {endpoint.Address.MapToIPv4()}:{endpoint.Port}. Time to fight!",
                GetType());
        }
    }
}