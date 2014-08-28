using System;
using System.Collections.Generic;
using TShockAPI;
using TShockAPI.DB;
using Terraria;
using System.Linq;
namespace RegionFlags
{ 
        #region FlaggedRegion
        class FlaggedRegion {
        
        private int flags = 0;
        private Region region;
        private int dps = 0;
        private int hps = 0;
        private List<string> bannedItems = new List<string>();

        public FlaggedRegion(Region r)
        {
            region = r;
        }

        public FlaggedRegion(Region r, int f)
        {
            region = r;
            flags = f;
        }

        public Region getRegion()
        {
            return region;
        }

        public void SetFlags(Flags f)
        {
            if (f == Flags.NONE)
                flags = 0;
            else
                flags |= (int)f; // |: shift + \
        }

        public void DelFlags(Flags f)
        {
            flags &= (int)(~f);
        }
        #endregion

        #region GetFlags
        public List<Flags> getFlags()
        {
            List<Flags> f = new List<Flags>();
            if ((flags & (int)Flags.PRIVATE) == (int)Flags.PRIVATE)
            {
                f.Add(Flags.PRIVATE);
            }
            if ((flags & (int)Flags.DEATH) == (int)Flags.DEATH)
            {
                f.Add(Flags.DEATH);
            }
            if ((flags & (int)Flags.PVP) == (int)Flags.PVP)
            {
                f.Add(Flags.PVP);
            }
            if ((flags & (int)Flags.PVP) == (int)Flags.PVP)
            {
                f.Add(Flags.PVP);
            }
            if ((flags & (int)Flags.HURT) == (int)Flags.HURT)
            {
                f.Add(Flags.HURT);
            }
            if ((flags & (int)Flags.NOITEM) == (int)Flags.NOITEM)
            {
                f.Add(Flags.NOITEM);
            }
            if ((flags & (int)Flags.NOMOB) == (int)Flags.NOMOB)
            {
                f.Add(Flags.NOMOB);
            }
            if ((flags & (int)Flags.MOBKILL) == (int)Flags.MOBKILL)
            {
                f.Add(Flags.MOBKILL);
            }
            if ((flags & (int)Flags.HEALONDAMAGE) == (int)Flags.HEALONDAMAGE)
            {
                f.Add(Flags.HEALONDAMAGE);
            }
            if ((flags & (int)Flags.GODMOB) == (int)Flags.GODMOB)
            {
                f.Add(Flags.GODMOB);
            }
            if ((flags & (int)Flags.HEAL) == (int)Flags.HEAL)
            {
                f.Add(Flags.HEAL);
            }
            if ((flags & (int)Flags.NOPVP) == (int)Flags.NOPVP)
            {
                f.Add(Flags.NOPVP);
            }
            if ((flags & (int)Flags.ITEMBAN) == (int)Flags.ITEMBAN)
            {
                f.Add(Flags.ITEMBAN);
            }
            return f;
        }

        public int getIntFlags()
        {
            return flags;
        }

        public void setDPS(int s)
        {
            dps = s;
        }

        public int getDPS()
        {
            return dps;
        }

        public void setHPS(int s)
        {
            hps = s;
        }

        public int getHPS()
        {
            return hps;
        }

        public void setBannedItems(List<string> items)
        {
            bannedItems = items;
        }

        public List<String> getBannedItems()
        {
            return bannedItems;
        }
        #endregion
}
        #region RegionManager
        class FlaggedRegionManager {
        
        private Dictionary<string, FlaggedRegion> regions;
        
        public FlaggedRegionManager()
        {
            regions = new Dictionary<string, FlaggedRegion>();
        }
        public void ImportRegion( string name, int flags, int d, int h, List<string> items ) // add string wid after name
        {
            //public List<Region> ListAllRegions(string worldid);
            //var rid = TShock.Regions.ListAllRegions(wid);
            var reg = TShock.Regions.GetRegionByName(name);
            //var regwid = TShock.Regions.GetRegionByWorldID(wid);
            if (reg == null) // && regwid == Main.worldID.ToString // add rid.Equals(Main.worldID)
            {
                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Log.Error("Some regions were not found in this world.");

                //Console.WriteLine( "{0} was not found in region list.", name);
                return;
            }
            FlaggedRegion f = new FlaggedRegion(reg, flags);
            f.setDPS(d);
            f.setHPS(h);
			f.setBannedItems(items);
            regions.Add(name, f);
        }

        public bool AddRegion( string name, int flags )
        {
            if( regions.ContainsKey( name ) )
            {
                return false;
            }
            var reg = TShock.Regions.GetRegionByName(name);
            FlaggedRegion f = new FlaggedRegion(reg, flags);
            f.setDPS(0);
            //TODO: Save to db
            RegionFlags.db.Query(
                    "INSERT INTO Regions (Name, Flags, Damage) VALUES (@0, @1, @2);",
                    name, flags, 0);
            regions.Add(name, f);

            return true;
        }

        public bool UpdateRegion( string name )
        {
            if( !regions.ContainsKey(name))
            {
                return false;
            }

            FlaggedRegion f = regions[name];

            //string Flagsnew = "";
            //Flagsnew = String.Join(",", f.getIntFlags);
            RegionFlags.db.Query(
                    "UPDATE Regions SET Flags=@0, Damage=@1, Heal=@2 WHERE Name=@3", f.getIntFlags(), f.getDPS(), f.getHPS(), name);
            return true;
        }

        public FlaggedRegion getRegion( string region )
        {
            if( regions.ContainsKey(region))
            {
                return regions[region];
            }
            return null;
        }

        public List<FlaggedRegion> InRegions( int x, int y )
        {
            List<FlaggedRegion> ret = new List<FlaggedRegion>();
            foreach( FlaggedRegion reg in regions.Values )
            {
                if (reg.getRegion() != null)
                {
                    if (reg.getRegion().InArea(x, y))
                        ret.Add(reg);
                }
            }
            return ret;
        }

	    public void Clear()
	    {
		    regions.Clear();
	    }
    }
    #endregion
}
