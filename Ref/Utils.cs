using Terraria;
using System.Collections.Generic;

namespace RegionFlags
{
    /*class Utils
    {
        public static void GiveItem(string name, int X, int Y, int width, int height, int type, int stack, int prefix, int id, Vector2 velocity)
        {
            int itemid = Item.NewItem((int)X, (int)Y, width, height, type, stack, true, prefix);

            // This is for special pickaxe/hammers/swords etc
            Main.item[itemid].SetDefaults(name);
            // The set default overrides the wet and stack set by NewItem
            Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width,
                                                           Main.item[itemid].height);
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

        public Vector2 getTP()
        {
            Vector2 pos = positions.Peek();
            reset(pos);
            return pos;
        }

    }*/
}
