﻿using ARS_SeaBreezeFCS32.Buildables;
using FCSCommon.Helpers;
using FCSCommon.Utilities;

namespace ARS_SeaBreezeFCS32.Mono
{
    public partial class ARSolutionsSeaBreezeController : HandTarget, IHandTarget
    {
        private bool _onInterfaceButton;

        public void OnHandClick(GUIHand hand)
        {
            if (_onInterfaceButton || Player.main == null || !IsConstructed) return;
            QuickLogger.Debug($"Clicked on Seabreeze {PrefabId}", true);
            _fridgeContainer.OpenStorage();
        }

        internal void OnInterfaceButton(bool value)
        {
            QuickLogger.Debug($"OnInterfaceButton: {value}", true);
            _onInterfaceButton = value;
        }

        public void OnHandHover(GUIHand hand)
        {
            if (_onInterfaceButton) return;
            HandReticle main = HandReticle.main;
            main.SetInteractText(LanguageHelpers.GetLanguage(ARSSeaBreezeFCS32Buildable.OnSeabreezeHoverkey));
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        internal void ResetOnInterceButton()
        {
            _onInterfaceButton = false;
        }
    }
}
