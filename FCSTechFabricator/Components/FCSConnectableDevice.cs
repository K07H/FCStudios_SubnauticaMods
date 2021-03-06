﻿using FCSTechFabricator.Abstract;
using FCSTechFabricator.Interfaces;
using System;
using SMLHelper.V2.Crafting;
using UnityEngine;

namespace FCSTechFabricator.Components
{
    public class FCSConnectableDevice : MonoBehaviour
    {
        private PrefabIdentifier _prefabId;
        private NameController _nameController;
        private FCSController _mono;
        private IFCSStorage _storage;
        private TechType _techtype;

        public void Initialize(FCSController mono, IFCSStorage storage)
        {
            _mono = mono;
            _storage = storage;
        }

        public TechType GetTechType()
        {
            if (_techtype == TechType.None)
            {
                var techTag = GetComponentInParent<TechTag>() ?? GetComponentInChildren<TechTag>();
                _techtype = techTag.type;
            }

            return _techtype;
        }
        
        public int GetContainerFreeSpace => _storage.GetContainerFreeSpace;

        public string GetPrefabIDString()
        {
            return _prefabId?.Id;
        }
        
        public bool CanBeStored(int amount)
        {
            return _storage.CanBeStored(amount);
        }

        public virtual bool AddItemToContainer(InventoryItem item, out string reason)
        {
            reason = null;//Todo add needed thibgsb
            return _storage.AddItemToContainer(item);
        }

        public virtual void SetNameControllerTag(object obj)
        {
            if (_nameController != null)
            {
                _nameController.Tag = obj;
            }
        }

        public string GetName()
        {
            return _nameController != null ? _nameController.GetCurrentName() : string.Empty;
        }
        
        public void Start()
        {
            if (_nameController == null)
                _nameController = GetComponentInParent<NameController>();

            if (_prefabId == null)
                _prefabId = GetComponentInParent<PrefabIdentifier>() ?? GetComponent<PrefabIdentifier>();
        }

        public void SubscribeToNameChange(Action<string, NameController> method)
        {
            if (_nameController != null)
            {
                _nameController.OnLabelChanged += method;
            }
        }

        public void OnDestroy()
        {

        }
    }
}
