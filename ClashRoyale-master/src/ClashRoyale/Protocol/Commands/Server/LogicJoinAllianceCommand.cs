﻿using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicJoinAllianceCommand : LogicCommand
    {
        public LogicJoinAllianceCommand(Device device) : base(device)
        {
            Type = 206;
        }

        public long AllianceId { get; set; }
        public string AllianceName { get; set; }
        public int AllianceBadge { get; set; }

        public override void Encode()
        {
            Data.WriteLong(AllianceId);
            Data.WriteScString(AllianceName);

            Data.WriteVInt(16);
            Data.WriteVInt(AllianceBadge);

            Data.WriteVInt(0);
            Data.WriteVInt(2);

            Data.WriteNullVInt(2);

            Data.WriteVInt(0);
            Data.WriteVInt(0);
        }
    }
}