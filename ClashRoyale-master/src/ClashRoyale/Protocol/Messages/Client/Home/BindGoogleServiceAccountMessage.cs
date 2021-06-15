﻿using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class BindGoogleServiceAccountMessage : PiranhaMessage
    {
        public BindGoogleServiceAccountMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14262;
        }
    }
}