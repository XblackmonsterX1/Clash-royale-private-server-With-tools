﻿using System;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Protocol.Messages.Client.Sector
{
    public class SectorCommandMessage : PiranhaMessage
    {
        public SectorCommandMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12904;
            RequiredState = Device.State.Battle;
        }

        public int Tick { get; set; }
        public int Count { get; set; }

        public override void Decode()
        {
            Reader.ReadVInt();
            Tick = Reader.ReadVInt();
            Count = Reader.ReadVInt();
        }

        public override void Process()
        {
            Device.LastSectorCommand = DateTime.UtcNow;

            if (Count < 0 || Count > 128) return;

            var battle = Device.Player.Battle;
            if (battle != null)
                if (!battle.IsRunning && !Resources.Configuration.UseUdp)
                    battle.BattleTimer.Start();

            for (var i = 0; i < Count; i++)
            {
                var type = Reader.ReadVInt();

                if (type >= 500) break;

                if (LogicCommandManager.Commands.ContainsKey(type))
                    try
                    {
                        if (Activator.CreateInstance(LogicCommandManager.Commands[type], Device,
                                Reader) is
                            LogicCommand
                            command)
                        {
                            command.Decode();
                            command.Encode();
                            command.Process();

                            Logger.Log($"SectorCommand {type} with Tick {Tick} has been processed.",
                                GetType(), ErrorLevel.Debug);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, GetType(), ErrorLevel.Error);
                    }
                else
                    Logger.Log($"SectorCommand {type} is unhandled.", GetType(), ErrorLevel.Warning);
            }
        }
    }
}