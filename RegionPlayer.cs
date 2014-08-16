using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionFlags
{
    class RegionPlayer
    {
        private TSPlayer player;
        private bool previousPVPMode = false;
        private bool forcedPVP = false;
        private bool removedPVP = false;
        private PositionQueue positions;
        private FlaggedRegionManager regionManager;
        private DateTime lastWarned = DateTime.Now;

        public RegionPlayer( TSPlayer ply, FlaggedRegionManager regionManager )
        {
            player = ply;
            positions = new PositionQueue();
            this.regionManager = regionManager;
        }

        private DateTime lastUpdate = DateTime.Now;
        private DateTime lastUpdateAnnounce = DateTime.Now;
        private DateTime lastDamageUpdate = DateTime.Now;
        private DateTime lastHealUpdate = DateTime.Now;

        public void Update()
        {
            DateTime now = DateTime.Now;
            
            Region r = TShock.Regions.GetTopRegion( TShock.Regions.InAreaRegion(player.TileX, player.TileY) );

            bool inPVPZone = false;
            bool inNoPVPZone = false;

            bool warning = ((now - lastWarned).TotalSeconds > 5);

            if( r != null )
            {
                FlaggedRegion reg = regionManager.getRegion(r.Name);
                if( reg != null )
                {
                    List<Flags> flags = reg.getFlags();
                    if( flags.Contains(Flags.PRIVATE) && !r.HasPermissionToBuildInRegion(player) )
                    {
                        Vector2 pos = positions.getTP();
                        player.Teleport((int)pos.X, (int)pos.Y);
                        if( warning )
                        {
                            player.SendMessage(String.Format("[SYSTEM] You can't enter {0} at your level!", r.Name), Color.OrangeRed);// Terrabear
                            lastWarned = now;
                        }
                    }
	                List<string> bannedItems = new List<string>();
	                if (flags.Contains(Flags.ITEMBAN) && InvalidInventory(reg.getItembans(), out bannedItems))
	                {
						Vector2 pos = positions.getTP();
		                Vector2 diff = pos - player.TPlayer.position;
						if(((diff.X*diff.X) + (diff.Y*diff.Y)) > (100*100))
							player.Teleport((int)pos.X, (int)pos.Y);
						else
							player.Spawn(Main.spawnTileX, Main.spawnTileY);


						if (warning)
						{
							player.SendMessage(String.Format("The following are banned in that area: {0}", string.Join(",", bannedItems)), Color.Red);
							lastWarned = now;
						}
	                }
                    if (flags.Contains(Flags.DEATH) && !r.HasPermissionToBuildInRegion(player))
                    {
                        NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1, " died Indiana Jone's style.", player.Index, 0, 999999,
                                (float)0);
                        if (warning)
                        {
                            player.SendMessage("You just stumbled into a death trap... no pun intended.", Color.Yellow);
                            lastWarned = now;
                        }
                    }
                    if (flags.Contains(Flags.PVP))
                    {
                        if (!player.TPlayer.hostile)
                        {
                            player.SendMessage("[SYSTEM] PVP Arena: PvP enabled.", Color.Yellow);
                        }

                        player.TPlayer.hostile = true;
                        player.SendData(PacketTypes.TogglePvp);
                        NetMessage.SendData((int) PacketTypes.TogglePvp, -1, -1, "", player.Index);
                        inPVPZone = true;
                        forcedPVP = true;
                    }
                    if (flags.Contains(Flags.NOPVP))
                    {
                        if (player.TPlayer.hostile)
                        {
                            player.SendMessage("[SYSTEM] None-PvP Zone: PvP disabled.", Color.Yellow);
                        }

                        player.TPlayer.hostile = false;
                        player.SendData(PacketTypes.TogglePvp);
                        NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, "", player.Index);
                        inNoPVPZone = true;
                        removedPVP = true;
                    }
                    if (flags.Contains(Flags.HURT))
                    {
                        if( (now - lastDamageUpdate).TotalSeconds > 0 )
                        {
                            lastDamageUpdate = now;
                            if( reg.getDPS() > 0 )
                            {
                                int damage = (player.TPlayer.statDefense/2) + reg.getDPS();
                                NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1, " died a slow, horrible death.", player.Index, 0, damage,
                                (float)0);
                            }
                        }
                    }
                    if (flags.Contains(Flags.HEAL) && reg.getHPS() > 0)
                    {
                        if ((now - lastHealUpdate).TotalSeconds >= reg.getHPS())
                        {
                            lastHealUpdate = now;
                            var items = TShock.Utils.GetItemByIdOrName("heart");
                            Player ply = player.TPlayer;
                            Utils.GiveItem(items[0].name, (int)ply.position.X, (int)ply.position.Y, items[0].width,
                                        items[0].height, items[0].type, 1, items[0].prefix, player.Index, ply.velocity);
                        }
                    }
                }
            }

            if (!inPVPZone && forcedPVP)
            {
                forcedPVP = false;
                player.TPlayer.hostile = false;
                player.SendData(PacketTypes.TogglePvp);
                NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, "", player.Index);
                player.SendMessage("[SYSTEM] PvP Arena left, Now you can toggle PvP.", Color.Green);
            }

            if (!inNoPVPZone && removedPVP)
            {
                removedPVP = false;
                player.TPlayer.hostile = false;
                player.SendData(PacketTypes.TogglePvp);
                NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, "", player.Index);
                player.SendMessage("[SYSTEM] None-PvP Zone left, Now you can toggle PvP.", Color.Green);
            }

            if ((now - lastUpdate).TotalSeconds > 1)
            {
                positions.enqueue(player.TPlayer.position);
                lastUpdate = now;
            }
        }

        public TSPlayer GetPlayer()
        {
            return player;
        }

	    private bool InvalidInventory(List<string> items, out List<string> banned )
	    {
			banned = new List<string>();
		    foreach (Item i in player.TPlayer.inventory)
		    {
			    if (i != null && i.stack > 0)
			    {
				    if (items.Select(it => it.ToUpper()).Contains(i.name.ToUpper()) || items.Contains(i.netID.ToString()))
				    {
					    banned.Add(i.name);
				    }
			    }
		    }

		    return banned.Count > 0;
	    }
    }
}
