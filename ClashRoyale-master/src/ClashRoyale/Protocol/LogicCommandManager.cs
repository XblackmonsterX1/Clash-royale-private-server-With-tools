﻿using System;
using System.Collections.Generic;
using ClashRoyale.Protocol.Commands.Client;
using ClashRoyale.Protocol.Commands.Server;

namespace ClashRoyale.Protocol
{
    public class LogicCommandManager
    {
        public static Dictionary<int, Type> Commands;

        static LogicCommandManager()
        {
            Commands = new Dictionary<int, Type>
            {
                {1, typeof(DoSpellCommand)},

                {500, typeof(LogicSwapSpellsCommand)},
                {501, typeof(LogicSelectDeckCommand)},
                {504, typeof(LogicFuseSpellsCommand)},
                {507, typeof(LogicBuyResourcePackCommand)},
                {509, typeof(LogicCollectFreeChestCommand)},
                {511, typeof(LogicCollectCrownChestCommand)},
                {513, typeof(LogicFreeWorkerCommand)},
                {514, typeof(LogicKickAllianceMemberCommand)},
                {516, typeof(LogicBuyChestCommand)},
                {517, typeof(LogicBuyResourcesCommand)},
                {518, typeof(LogicBuySpellCommand)},
                //{520, typeof(LogicShopSeenCommand)},
                //{521, typeof(LogicSendAllianceMailCommand)},
                {522, typeof(LogicChallengeCommand)},
                {525, typeof(StartMatchmakeCommand)},
                {526, typeof(LogicChestNextCardCommand)},
                {529, typeof(LogicCopyDeckCommand)},
                //{557, typeof(UnknownCommand)} // NewLeaguePopupSeen?
            };
        }
    }
}