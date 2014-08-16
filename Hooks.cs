using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionFlags
{
    class PlayerHooks
    {
        private FlaggedRegionManager regionManager;

        public PlayerHooks(FlaggedRegionManager region)
        {
            regionManager = region;
        }

       public void OnDamage( object sender, TShockAPI.GetDataHandlers.PlayerDamageEventArgs args )
        {
            Region r = TShock.Regions.GetTopRegion(
                TShock.Regions.InAreaRegion((int)Main.player[args.ID].position.X / 16, (int)Main.player[args.ID].position.Y / 16));
            if( r != null )
            {
               FlaggedRegion reg = regionManager.getRegion(r.Name);
               if (reg != null)
               {
                   List<Flags> flags = reg.getFlags();
                   if( flags.Contains( Flags.HEALONDAMAGE ) )
                   {
                       int heal = 0;
                       int damage = Math.Max(args.Damage*(args.Critical ? 2 : 1) -
                                    (int)(Math.Round(Main.player[args.ID].statDefense * .5)), 1);

                       var items = TShock.Utils.GetItemByIdOrName("heart");
                       while(heal < damage)
                       {
                           Utils.GiveItem(items[0].name, (int)Main.player[args.ID].position.X, (int)Main.player[args.ID].position.Y, items[0].width,
                                items[0].height, items[0].type, 1, items[0].prefix, args.ID, Main.player[args.ID].velocity);
                           heal += 20;
                       }
                   }
               }
            }
        }
    }

    class NPCHooks
    {
        private FlaggedRegionManager regionManager;

        public NPCHooks(FlaggedRegionManager region)
        {
            regionManager = region;
        }

        public void OnNPCStrike(object sender, TShockAPI.GetDataHandlers.NPCStrikeEventArgs args)
        {
            Region r = TShock.Regions.GetTopRegion(
                TShock.Regions.InAreaRegion((int)Main.npc[args.ID].position.X / 16, (int)Main.npc[args.ID].position.Y / 16));
            if (r != null)
            {
                FlaggedRegion reg = regionManager.getRegion(r.Name);
                if (reg != null)
                {
                    List<Flags> flags = reg.getFlags();
                    if (flags.Contains(Flags.GODMOB))
                    {
                        args.Handled = true;
                        Main.npc[args.ID].life = Main.npc[args.ID].lifeMax;
                        NetMessage.SendData(23, -1, -1, "", args.ID, 0f, 0f, 0f, 0);
                    }
                }
            }
        }
    }
}
