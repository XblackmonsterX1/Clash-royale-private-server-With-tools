﻿using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class GoHomeMessage : PiranhaMessage
    {
        public GoHomeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14101;
            Save = true;
            RequiredState = Device.State.Battle;
        }

        public override async void Process()
        {
            await new OwnHomeDataMessage(Device).SendAsync();
        }
    }
}