namespace ZBase.Persistence {
    public class IpBanModel {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Reason { get; set; }
        public string BannedBy { get; set; }
    }
}