﻿using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Decks.Items;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class DoSpellCommand : LogicCommand
    {
        public DoSpellCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Type = 1;
        }

        public int ClientTick { get; set; }
        public int Checksum { get; set; }
        public int SenderHighId { get; set; }
        public int SenderLowId { get; set; }
        public int SpellDeckIndex { get; set; }
        public int SpellIndex { get; set; }
        public int ClassId { get; set; }
        public int InstanceId { get; set; }
        public int TroopLevel { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override void Decode()
        {
            // Header
            {
                ClientTick = Reader.ReadVInt();
                Checksum = Reader.ReadVInt();

                SenderHighId = Reader.ReadVInt();
                SenderLowId = Reader.ReadVInt();
            }

            SpellDeckIndex = Reader.ReadVInt();

            ClassId = Reader.ReadVInt();
            InstanceId = Reader.ReadVInt();

            SpellIndex = Reader.ReadVInt();

            TroopLevel = Reader.ReadVInt();

            X = Reader.ReadVInt();
            Y = Reader.ReadVInt();
        }

        public override void Encode()
        {
            // Header
            {
                Data.WriteVInt(Type);

                Data.WriteVInt(ClientTick);
                Data.WriteVInt(Checksum);

                Data.WriteVInt(SenderHighId);
                Data.WriteVInt(SenderLowId);
            }

            Data.WriteVInt(SpellDeckIndex);

            Data.WriteVInt(ClassId);
            Data.WriteVInt(InstanceId);

            Data.WriteVInt(SpellIndex);
        }

        public override void Process()
        {
            var battle = Device.Player.Battle;
            if (battle == null) return;

            var data = Data.ReadBytes(Data.ReadableBytes).Array;

            var buffer = Unpooled.Buffer(9);
            {
                buffer.WriteBytes(data);

                buffer.WriteVInt(TroopLevel);

                buffer.WriteVInt(X);
                buffer.WriteVInt(Y);

                battle.GetOwnQueue(Device.Player.Home.Id).Enqueue(buffer.Array);
            }

            var enemyBuffer = Unpooled.Buffer(9);
            {
                enemyBuffer.WriteBytes(data);

                enemyBuffer.WriteVInt(1); // IsAttack
                {
                    enemyBuffer.WriteVInt(Card.Id(ClassId, InstanceId));
                }

                enemyBuffer.WriteVInt(TroopLevel);

                enemyBuffer.WriteVInt(X);
                enemyBuffer.WriteVInt(Y);

                if (battle.Is2V2)
                {
                    var others = battle.GetOtherQueues(Device.Player.Home.Id);

                    foreach (var queue in others)
                    {
                        queue.Enqueue(enemyBuffer.Array);
                    }
                }
                else 
                    battle.GetEnemyQueue(Device.Player.Home.Id).Enqueue(enemyBuffer.Array);
            }

            //battle.Replay.AddCommand(Type, ClientTick - 20, ClientTick, SenderHighId, SenderLowId, ClassId * 1000000 + InstanceId, X, Y, SpellDeckIndex);
        }
    }
}