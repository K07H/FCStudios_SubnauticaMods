﻿using FCS_AIMarineTurbine.Buildable;
using FCS_AIMarineTurbine.Mono;
using FCSCommon.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace FCS_AIJetStreamT242.Mono
{
    /// <summary>
    // This class handles all power management  of the turbine
    /// </summary>
    internal class AIJetStreamT242PowerManager : MonoBehaviour, IPowerInterface
    {
        private bool _hasBreakerTripped;
        internal bool IsSafeToContinue { get; set; }
        internal Action OnKillBattery { get; set; }
        internal Action OnBreakerTripped { get; set; }
        internal Action OnBreakerReset { get; set; }
        private float _charge;
        public float MaxPowerPerMin { get; set; } = 100;
        private float _capacity;
        private PowerRelay _powerRelay;
        private bool _isBatteryDestroyed;
        private AIJetStreamT242Controller _mono;
        private float _timeCurrDeltaTime;
        private float _storedPower;
        private float _energyPerSec;
        private bool _initialize;


        #region Unity Methods

        private void Start()
        {
            _capacity = AIJetStreamT242Buildable.JetStreamT242Config.MaxCapacity;

            StartCoroutine(UpdatePowerRelay());
            //InvokeRepeating();
        }

        private void Update()
        {
            if (_initialize)
            {
                ProducePower();
            }
        }
        #endregion

        private IEnumerator UpdatePowerRelay()
        {
            QuickLogger.Debug("In UpdatePowerRelay");

            var i = 1;

            while (_powerRelay == null)
            {
                QuickLogger.Debug($"Checking For Relay... Attempt {i}");

                PowerRelay relay = PowerSource.FindRelay(this.transform);
                if (relay != null && relay != _powerRelay)
                {
                    _powerRelay = relay;
                    _powerRelay.AddInboundPower(this);
                    QuickLogger.Debug("PowerRelay found");
                }
                else
                {
                    _powerRelay = null;
                }

                i++;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void ProducePower()
        {
            if (_mono.HealthManager == null) return;

            
            
            if(_mono.MaxSpeed > 0)
            {
                var decPercentage = (MaxPowerPerMin / _mono.MaxSpeed) / 60;
                _energyPerSec = _mono.GetCurrentSpeed() * decPercentage;
            }
            else
            {
                _energyPerSec = 0;
            }

            if (_hasBreakerTripped || _mono.HealthManager.IsDamageApplied()) return;

            _timeCurrDeltaTime += DayNightCycle.main.deltaTime;

            if (!(_timeCurrDeltaTime >= 1)) return;
            
            _charge = Mathf.Clamp(_charge + _energyPerSec, 0, AIJetStreamT242Buildable.JetStreamT242Config.MaxCapacity);

            _timeCurrDeltaTime = 0;
        }

        internal void KillBattery()
        {
            QuickLogger.Debug($"KillBattery");
            _capacity = _charge = 0;
            OnKillBattery?.Invoke();
        }

        internal void DestroyBattery()
        {
            QuickLogger.Debug($"Destroy Battery");
            _charge = _capacity = 0f;
            _isBatteryDestroyed = true;
        }

        internal void RepairBattery()
        {
            _capacity = AIJetStreamT242Buildable.JetStreamT242Config.MaxCapacity;
            _isBatteryDestroyed = false;
        }

        private void OnDestroy()
        {
            try
            {
                KillBattery();

                if (_powerRelay != null)
                {
                    _powerRelay.RemoveInboundPower(this);
                }
            }
            catch (Exception e)
            {
                QuickLogger.Error(e.Message);
            }
        }

        #region IPowerInterface

        public float GetPower()
        {
            if (_charge < 0.1)
            {
                _charge = 0.0f;
            }

            return _charge;
        }

        public float GetMaxPower()
        {
            return _capacity;
        }

        public bool ModifyPower(float amount, out float modified)
        {
            modified = 0f;


            bool result;
            if (amount >= 0f)
            {
                result = (amount <= AIJetStreamT242Buildable.JetStreamT242Config.MaxCapacity - _charge);
                modified = Mathf.Min(amount, AIJetStreamT242Buildable.JetStreamT242Config.MaxCapacity - _charge);
                _charge += Mathf.Round(modified);
            }
            else
            {
                result = (_charge >= -amount);
                if (GameModeUtils.RequiresPower())
                {
                    modified = -Mathf.Min(-amount, _charge);
                    _charge += Mathf.Round(modified);
                }
                else
                {
                    modified = amount;
                }
            }

            return result;
        }

        public bool HasInboundPower(IPowerInterface powerInterface)
        {
            return false;
        }

        public bool GetInboundHasSource(IPowerInterface powerInterface)
        {
            return false;
        }
        #endregion

        internal float GetEnergyPerSecond()
        {
            return _energyPerSec;
        }

        internal float GetCharge()
        {
            return _charge;
        }

        internal void SetCharge(float savedDataCharge)
        {
            _charge = savedDataCharge;
        }

        private void TriggerPowerOff()
        {
            _hasBreakerTripped = true;
            _storedPower = _charge;
            _charge = 0.0f;
            OnBreakerTripped?.Invoke();
        }

        private void TriggerPowerOn()
        {
            _charge = _storedPower;
            _storedPower = 0.0f;
            _hasBreakerTripped = false;
            OnBreakerReset?.Invoke();
        }

        internal void Initialize(AIJetStreamT242Controller mono)
        {
            _mono = mono;
            _initialize = true;
        }

        internal bool GetHasBreakerTripped()
        {
            return _hasBreakerTripped;
        }

        internal void SetHasBreakerTripped(bool value)
        {
            _hasBreakerTripped = value;
        }

        internal bool IsBatteryDestroyed()
        {
            return _isBatteryDestroyed;
        }

        internal void TogglePower()
        {
            if (GetHasBreakerTripped())
            {
                TriggerPowerOn();
            }
            else if (GetHasBreakerTripped() == false)
            {
                TriggerPowerOff();
            }
        }

        internal float GetStoredPower()
        {
            return _storedPower;
        }

        internal void SetStoredPower(float value)
        {
            _storedPower = value;
        }

        public void PollPowerRate(out float consumed, out float created)
        {
            consumed = 0f;
            created = 0f;
        }

        public GameObject GetGameObject()
        {
            return base.gameObject;
        }
    }
}
