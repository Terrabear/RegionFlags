using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionFlags
{
    #region Utils
    class Utils
    {
        public static void GiveItem(string name, int X, int Y, int width, int height, int type, int stack, int prefix, int id, Vector2 velocity)
        {
            int itemid = Item.NewItem((int)X, (int)Y, width, height, type, stack, true, prefix);

            // This is for special pickaxe/hammers/swords etc
            Main.item[itemid].SetDefaults(name);
            // The set default overrides the wet and stack set by NewItem
            Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width, Main.item[itemid].height);
            Main.item[itemid].stack = stack;
            Main.item[itemid].owner = id;
            Main.item[itemid].prefix = (byte)prefix;
            Main.item[itemid].velocity = velocity;
            Main.item[itemid].noGrabDelay = 1;
            NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, "", itemid, 0f, 0f, 0f);
            NetMessage.SendData((int)PacketTypes.ItemOwner, -1, -1, "", itemid, 0f, 0f, 0f);
        }
    }

    public enum Flags
    {
        NONE = 0,
        PRIVATE = 1,
        PVP = 2,
        DEATH = 4,
        HURT = 8,
        NOITEM = 16,
        NOMOB = 32,
        MOBKILL = 64,
        HEALONDAMAGE = 128,
        GODMOB = 256,
        HEAL = 512,
        NOPVP = 1024,
        ITEMBAN = 2048,
        NOPROJ = 4096,
    }

    class PositionQueue
    {
        private Queue<Vector2> positions;
        public PositionQueue()
        {
            positions = new Queue<Vector2>();
        }

        public void enqueue(Vector2 pos)
        {
            positions.Enqueue(pos);
            if (positions.Count > 3)
                positions.Dequeue();
        }

        public void reset(Vector2 pos)
        {
            positions.Clear();
            positions.Enqueue(pos);
        }

        public Vector2 getpos()
        {
            Vector2 pos = positions.Peek();
            reset(pos);
            return pos;
        }

    }
    #endregion

    #region Hooks
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
    #endregion
}
