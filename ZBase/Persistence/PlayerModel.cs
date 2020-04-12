using System;

namespace ZBase.Persistence {
    public class PlayerModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public short Rank { get; set; }
        public bool GlobalChat { get; set; }
        public bool Stopped { get; set; }
        public bool Vanished { get; set; }
        public int BoundBlock { get; set; }
        public double TimeMuted { get; set; }
        public double BannedUntil { get; set; }
        public bool Banned { get; set; }
        public string BannedBy { get; set; }
        public string BanMessage { get; set; }
    }
}