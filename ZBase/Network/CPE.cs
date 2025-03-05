using ZBase.Common;

namespace ZBase.Network
{
    public class CPE
    {
        public static void CPEHandshake(Client c)
        {
            var extInfo = new ExtInfo
            {
                AppName = "ZBase " + ZBase.Main.Version,
                ExtensionCount = 2,
            };
            c.SendPacket(extInfo);

            var extEntry = new ExtEntry
            {
                ExtName = Constants.CustomBlocksExt,
                Version = 1
            };
            c.SendPacket(extEntry);

            extEntry = new ExtEntry
            {
                ExtName = Constants.ClickDistanceExt,
                Version = 1,
            };
            c.SendPacket(extEntry);

        }

        public static void CPEPackets(Client c)
        {
            if (c.ClientPlayer.Extensions.ContainsKey(Constants.ClickDistanceExt))
            {
                var setClick = new SetClickDistance
                {
                    Distance = Configuration.Settings.Cpe.ClickDistance
                };
                c.SendPacket(setClick);
            }

            if (c.ClientPlayer.Extensions.ContainsKey(Constants.CustomBlocksExt))
            {
                var supportLevel = new CustomBlockSupportLevel
                {
                    SupportLevel = 1
                };

                c.SendPacket(supportLevel);
            } else
            {
                c.ClientPlayer.Login();
            }
        }
    }
}
