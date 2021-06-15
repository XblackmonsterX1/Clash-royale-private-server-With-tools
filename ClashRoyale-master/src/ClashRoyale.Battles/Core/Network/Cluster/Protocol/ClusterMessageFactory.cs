﻿using System;
using System.Collections.Generic;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Server;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol
{
    public class ClusterMessageFactory
    {
        public static Dictionary<int, Type> Messages;

        static ClusterMessageFactory()
        {
            Messages = new Dictionary<int, Type>
            {
                {20103, typeof(ConnectionFailedMessage)},
                {20104, typeof(ConnectionOkMessage)}
            };
        }
    }
}