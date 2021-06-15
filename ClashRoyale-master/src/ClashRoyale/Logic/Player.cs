﻿using System;
using System.Diagnostics;
using System.Linq;
using ClashRoyale.Database;
using ClashRoyale.Extensions;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Logic.Home.StreamEntry;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;
using DotNetty.Buffers;
using Newtonsoft.Json;
using SharpRaven.Data;

namespace ClashRoyale.Logic
{
    public class Player
    {
        public Player(long id)
        {
            Home = new Home.Home(id, GameUtils.GenerateToken);
        }

        public Player()
        {
            // Player.
        }

        public Home.Home Home { get; set; }

        [JsonIgnore] public LogicBattle Battle { get; set; }
        [JsonIgnore] public Device Device { get; set; }

        public void RankingEntry(IByteBuffer packet)
        {
            packet.WriteVInt(Home.ExpLevel);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteScString("DE");
            packet.WriteLong(Home.Id);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            var info = Home.AllianceInfo;

            if (info.HasAlliance)
            {
                packet.WriteBoolean(true);

                packet.WriteLong(info.Id);
                packet.WriteScString(info.Name);

                packet.WriteByte(16);
                packet.WriteVInt(info.Badge);
            }

            packet.WriteVInt(0); // Has League
        }

        public void LogicClientHome(IByteBuffer packet)
        {
            packet.WriteLong(Home.Id);

            // Unknown
            {
                packet.WriteVInt(0);
                packet.WriteVInt(Home.GetFreeChestId()); // Current Freechest Id

                // Free Chest Timer
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(1500268361); // Last Login

                packet.WriteByte(0);
            }

            // Decks
            Home.Deck.Encode(packet);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteNullVInt();

            packet.WriteVInt(33);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(1);
            packet.WriteVInt(0);

            // Events
            packet.WriteVInt(1);
            {
                packet.WriteVInt(1109);
                packet.WriteScString("2v2 Button");

                packet.WriteVInt(8);
                packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
                packet.WriteVInt(1609462800);
                packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteScString("2v2 Button");
                packet.WriteScString("{\"HideTimer\":false,\"HidePopupTimer\":false}\"");
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteNullVInt();

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0); // Challenge Events?

            // Events
            packet.WriteVInt(1);
            {
                packet.WriteVInt(1109);
            }

            // Events
            packet.WriteVInt(2);
            {
                /*packet.WriteVInt(0);
                packet.WriteScString("{\"GameMode\":\"TeamVsTeamLadder\",\"Target_MinXPLevel\":3,\"HideTimer\":false,\"HidePopupTimer\":true}");*/

                /*packet.WriteVInt(1);
                packet.WriteScString("{\"ID\":\"SHOP_CYCLE_MANAGEMENT\",\"Params\":{\"EpicChestCycleDuration\":5,\"LegendaryChestCycleDuration\":7,\"ArenaPackCycleDuration\":7}}");*/

                packet.WriteVInt(2);
                packet.WriteScString("{\"ID\":\"CARD_RELEASE\",\"Params\":{}})");

                /*packet.WriteVInt(3);
                packet.WriteScString("{\"ID\":\"KILL_SWITCH\",\"Params\":{\"HideShopOffersUI\":false}}");*/

                packet.WriteVInt(4);
                packet.WriteScString("{\"ID\":\"CLAN_CHEST\",\"Params\":{}}");
            }

            packet.WriteVInt(4);

            // Chests
            {
                /*for (var i = 0; i < 0; i++)
                {
                    packet.WriteVInt(0);

                    packet.WriteVInt(19); // Instance Id
                    packet.WriteVInt(219); // Class Id 
                    packet.WriteVInt(1); // Unlocked // 8 - unlocking -> timer

                    //packet.WriteVInt(0);
                    //packet.WriteVInt(0);
                    //packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

                    packet.WriteBoolean(false); // Claimed
                    packet.WriteBoolean(false); // New
                    packet.WriteVInt(0);
                    packet.WriteVInt(0);
                    packet.WriteVInt(0);
                }*/

                packet.WriteVInt(0);
            }

            // FreeChest Timer
            if (!Home.IsFirstFreeChestAvailable())
                packet.WriteVInt((int) Home.FreeChestTime.AddHours(4).Subtract(DateTime.UtcNow).TotalSeconds * 20);
            else
                packet.WriteVInt(
                    (int) Home.FreeChestTime.AddHours(4).Subtract(DateTime.UtcNow.AddHours(4)).TotalSeconds * 20);

            packet.WriteVInt(0);

            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            if (Home.IsFirstFreeChestAvailable())
            { 
                packet.WriteBoolean(true);

                packet.WriteVInt(19);
                packet.WriteVInt(12);
                packet.WriteVInt(1);
                packet.WriteVInt(18);
                packet.WriteVInt(0);
                packet.WriteNullVInt();
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
            }
            else
            {
                packet.WriteBoolean(false);
            }

            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            // Crown Chest
            {
                packet.WriteVInt(Home.Crowns); // Crowns

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
            }

            packet.WriteVInt(-1);

            // Request Cooldown
            packet.WriteVInt(1714640);
            packet.WriteVInt(1726960);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(-1);

            packet.WriteVInt(Home.NameSet == 0 ? 1 : 3); // 1 = SetNamePopup, 2 = Upgrade Card Tutorial, 3 = NameSet

            for (var i = 0; i < 7; i++)
                packet.WriteVInt(0);

            packet.WriteVInt(2); // Page Opened
            packet.WriteVInt(Home.ExpLevel); // ExpLevel

            // Arena
            {
                packet.WriteData(Home.Arena.ArenaData(Home.Arena.CurrentArena));
            }

            // Shop
            {
                Home.Shop.Encode(packet);
            }

            // Timers
            for (var i = 0; i < 3; i++)
            {
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
            }

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(0);

            packet.WriteVInt(0); // Card request?

            packet.WriteVInt(0);

            packet.WriteVInt(23);

            // Array
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteShort(-2041);

            packet.WriteVInt(1);
            packet.WriteVInt(1);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(11);
            packet.WriteVInt(0);

            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(4);
            packet.WriteVInt(3);
            packet.WriteVInt(17);
            packet.WriteVInt(1);

            packet.WriteVInt(14);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(4);
            packet.WriteVInt(14);
            packet.WriteVInt(1);

            packet.WriteVInt(74);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(4);
            packet.WriteVInt(1);
            packet.WriteVInt(1);

            packet.WriteVInt(73);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(0);

            packet.WriteVInt(4);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(9);
            packet.WriteVInt(0);

            packet.WriteVInt(15);
            packet.WriteVInt(0);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(6);
            packet.WriteVInt(2);

            packet.WriteVInt(16);
            packet.WriteVInt(0);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(6);
            packet.WriteVInt(2);

            packet.WriteVInt(0);

            // Missions
            packet.WriteVInt(2);
            {
                packet.WriteVInt(26);
                packet.WriteVInt(46);

                packet.WriteVInt(28);
                packet.WriteVInt(16);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(1); // New Arenas Seen Count
            packet.WriteVInt(54000010);

            packet.WriteVInt(0); // Session Reward = 2
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(7); // Training Battles completed         
        }

        public void LogicClientAvatar(IByteBuffer packet)
        {
            // Id - Account Id - HomeId
            for (var i = 0; i < 3; i++)
            {
                packet.WriteVInt(Home.HighId);
                packet.WriteVInt(Home.LowId);
            }

            // Name
            {
                packet.WriteScString(Home.NameSet > 0 ? Home.Name : null);
                packet.WriteBoolean(Home.NameSet > 1); // NameSetByUser
            }

            // Profile
            {
                packet.WriteVInt(Home.Arena.CurrentArena + 1); // Arena 
                packet.WriteVInt(Home.Arena.Trophies); // Trophies 

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(100); // Legendary Trophies

                packet.WriteVInt(0); // Current Season Trophies
                packet.WriteVInt(0);
                packet.WriteVInt(0); // Displays near League // maybe never used

                packet.WriteVInt(0); // Best Season Trophies
                packet.WriteVInt(0); // Rank
                packet.WriteVInt(100); // Trophies
            }

            // League
            packet.WriteVInt(100); // Current Trophies
            packet.WriteVInt(50); // Past Trophies
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0); // set this 1 and it appears on the profile 

            packet.WriteVInt(8);

            // Game Variables
            packet.WriteVInt(10);
            {
                packet.WriteVInt(5);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(1);
                packet.WriteVInt(Home.Gold); // Gold

                packet.WriteVInt(5);
                packet.WriteVInt(3);
                packet.WriteVInt(2);

                packet.WriteVInt(5); // New Crowns
                packet.WriteVInt(4);
                packet.WriteVInt(Home.NewCrowns);

                packet.WriteVInt(5);
                packet.WriteVInt(5);
                packet.WriteVInt(Home.Gold); // Gold

                packet.WriteVInt(5);
                packet.WriteVInt(13);
                packet.WriteVInt(0); // New Gold

                packet.WriteVInt(5);
                packet.WriteVInt(14);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(16);
                packet.WriteVInt(51);

                packet.WriteVInt(5);
                packet.WriteVInt(28);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(29);
                packet.WriteVInt(72000006);
            }

            packet.WriteVInt(0); // Completed Achievements

            // Achievements
            {
                packet.WriteVInt(0); // Achievement Count
                packet.WriteVInt(0); // Achievement Count
            }

            // Profile Statistics
            packet.WriteVInt(6);
            {
                packet.WriteVInt(5);
                packet.WriteVInt(6);
                packet.WriteVInt(30);

                packet.WriteVInt(5);
                packet.WriteVInt(7);
                packet.WriteVInt(0); // Three Crown Win Count

                packet.WriteVInt(5);
                packet.WriteVInt(8);
                packet.WriteVInt(Home.Deck.Count); // Cards found

                packet.WriteVInt(5);
                packet.WriteVInt(1); // Count
                packet.WriteVInt(26000048); // CardId

                packet.WriteVInt(5);
                packet.WriteVInt(11);
                packet.WriteVInt(32);

                packet.WriteVInt(5);
                packet.WriteVInt(27);
                packet.WriteVInt(1);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0); // NPC? / Count?
            packet.WriteVInt(0);

            packet.WriteVInt(Home.Diamonds); // Diamonds
            packet.WriteVInt(Home.Diamonds); // FreeDiamonds

            packet.WriteVInt(Home.ExpPoints); // ExpPoints
            packet.WriteVInt(Home.ExpLevel); // ExpLevel

            packet.WriteVInt(0); // AvatarUserLevelTier

            if (Home.AllianceInfo.HasAlliance)
            {
                packet.WriteVInt(Home.NameSet == 0 ? 8 : 9); // HasAlliance

                var info = Home.AllianceInfo;

                packet.WriteVInt(info.HighId);
                packet.WriteVInt(info.LowId);
                packet.WriteScString(info.Name);
                packet.WriteVInt(info.Badge + 1);
                packet.WriteVInt(info.Role);
            }
            else
            {
                packet.WriteVInt(Home.NameSet == 0 ? 6 : 7); // HasAlliance
            }

            // Battle Statistics
            {
                packet.WriteVInt(0); // Games Played
                packet.WriteVInt(0); // Tournament Matches Played
                packet.WriteVInt(0);
                packet.WriteVInt(0); // Wins
                packet.WriteVInt(0); // Losses

                packet.WriteVInt(0);
            }

            // Tutorials
            {
                packet.WriteVInt(7);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0); // Has Challenge
            //  packet.WriteVInt(); // ID
            //  packet.WriteVInt(0); // WINS
            //  packet.WriteVInt(0); // LOSSES

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(0); // AccountCreated
            packet.WriteVInt(Home.TotalPlayTimeSeconds); // PlayTime
        }

        public async void AddEntry(AvatarStreamEntry entry)
        {
            lock (Home.Stream)
            {
                while (Home.Stream.Count >= 40)
                    Home.Stream.RemoveAt(0);

                var max = Home.Stream.Count == 0 ? 1 : Home.Stream.Max(x => x.Id);
                entry.Id = max == int.MaxValue ? 1 : max + 1; // If we ever reach that value... but who knows...

                Home.Stream.Add(entry);
            }

            await new AvatarStreamEntryMessage(Device)
            {
                Entry = entry
            }.SendAsync();
        }

        /// <summary>
        ///     Validates this session
        /// </summary>
        public void ValidateSession()
        {
            var session = Device.Session;
            session.Duration = (int) DateTime.UtcNow.Subtract(session.SessionStart).TotalSeconds;

            Home.TotalPlayTimeSeconds += session.Duration;

            while (Home.Sessions.Count >= 50) Home.Sessions.RemoveAt(0);

            Home.Sessions.Add(session);
        }

        public async void Save()
        {
#if DEBUG
            var st = new Stopwatch();
            st.Start();

            Resources.ObjectCache.CachePlayer(this);
            await PlayerDb.SaveAsync(this);

            st.Stop();
            Logger.Log($"Player {Home.Id} saved in {st.ElapsedMilliseconds}ms.", GetType(), ErrorLevel.Debug);
#else
            Resources.ObjectCache.CachePlayer(this);
            await PlayerDb.SaveAsync(this);
#endif
        }
    }
}