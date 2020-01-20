﻿using FCSCommon.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ExStorageDepot.Display
{
    /// <summary>
    /// Component that buttons on the power storage ui will inherit from. Handles working on whether something is hovered via IsHovered as well as interaction text. 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    internal abstract class OnScreenButton : MonoBehaviour
    {
        protected bool IsHovered { get; set; }
        public string TextLineOne { get; set; }
        public string TextLineTwo { get; set; }

        private bool isHoveredOutOfRange;

        public InterfaceButtonMode ButtonMode { get; set; } = InterfaceButtonMode.Background;
        public virtual void OnDisable()
        {
            this.IsHovered = false;
            isHoveredOutOfRange = false;
        }

        public virtual void Update()
        {
            bool inInteractionRange = InInteractionRange();

            if (this.IsHovered && inInteractionRange)
            {
                HandReticle main = HandReticle.main;

#if SUBNAUTICA
                if (ButtonMode == InterfaceButtonMode.None)
                {
                    main.SetIcon(HandReticle.IconType.Hand, 1f);
                    main.SetInteractTextRaw(this.TextLineOne, this.TextLineTwo);
                }
                else
                {
                    main.SetInteractTextRaw(this.TextLineOne, this.TextLineTwo);
                }
#elif BELOWZERO
                if (ButtonMode == InterfaceButtonMode.None)
                {
                    main.SetIcon(HandReticle.IconType.Hand, 1f);
                    main.SetTextRaw(HandReticle.TextType.Hand, this.TextLineOne);
                    main.SetTextRaw(HandReticle.TextType.HandSubscript, this.TextLineTwo);
                }
                else
                {
                    main.SetTextRaw(HandReticle.TextType.Hand, this.TextLineOne);
                    main.SetTextRaw(HandReticle.TextType.HandSubscript, this.TextLineTwo);
                }
#endif
            }

            if (this.IsHovered && inInteractionRange == false)
            {
                this.IsHovered = false;
            }

            if (this.IsHovered == false && isHoveredOutOfRange && inInteractionRange)
            {
                this.IsHovered = true;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (InInteractionRange())
            {
                this.IsHovered = true;
            }

            isHoveredOutOfRange = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            this.IsHovered = false;
            isHoveredOutOfRange = false;
        }

        public virtual void OnPointerClick(PointerEventData pointerEventData)
        {

        }

        protected bool InInteractionRange()
        {
            return Mathf.Abs(Vector3.Distance(this.gameObject.transform.position, Player.main.transform.position)) <= 2.5;
        }
    }
}
