﻿using FCSCommon.Utilities;

namespace FCS_DeepDriller.Configuration
{
    internal static class EquipmentConfiguration
    {
        internal static readonly string[] SlotIDs = new string[7]
        {
            "DDPowerCellCharger1",
            "DDPowerCellCharger2",
            "DDPowerCellCharger3",
            "DDPowerCellCharger4",
            "HDDAttachmentSlot1",
            "HDDAttachmentSlot2",
            "CDDAttachmentSlot1",
        };

        private static bool _addingSlots;

        public static void AddNewSlots()
        {
            if (!_addingSlots)
            {
                foreach (string slotID in SlotIDs)
                {
                    if (slotID.StartsWith("DD"))
                    {
                        Equipment.slotMapping.Add(slotID, EquipmentType.PowerCellCharger);
                        QuickLogger.Debug($"Adding slot {slotID}");
                    }

                    if (slotID.StartsWith("HDD"))
                    {
                        Equipment.slotMapping.Add(slotID, EquipmentType.Hand);
                        QuickLogger.Debug($"Adding slot {slotID}");
                    }

                    if (slotID.StartsWith("CDD"))
                    {
                        Equipment.slotMapping.Add(slotID, EquipmentType.CyclopsModule);
                        QuickLogger.Debug($"Adding slot {slotID}");
                    }
                }

                _addingSlots = true;
            }
        }

        internal static void RefreshPDA()
        {
            if (_pdaRefreshed || Player.main == null || !LargeWorldStreamer.main.IsReady()) return;      

            PDA pdaMain = Player.main?.GetPDA();

            if (pdaMain == null)
            {
                QuickLogger.Error("Player GetPDA was null when trying to refresh PDA");
                return;
            }

            pdaMain.Open(PDATab.None, null, null, -1f);
            pdaMain.Close();
            QuickLogger.Debug("Deep Driller refreshed PDA.");
            _pdaRefreshed = true;
        }

        private static bool _pdaRefreshed;
    }
}
