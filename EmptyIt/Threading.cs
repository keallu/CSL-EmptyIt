using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EmptyIt
{
    public class Threading : ThreadingExtensionBase
    {
        private ModConfig _modConfig;
        private SimulationManager _simulationManager;
        private BuildingManager _buildingManager;
        private Building[] _buildings;
        private FastList<ushort> _serviceBuildings;
        private Building _building;
        private BuildingAI _buildingAI;
        private LandfillSiteAI _landfillSiteAI;
        private CemeteryAI _cemeteryAI;
        private SnowDumpAI _snowDumpAI;
        private List<ushort> _buildingIds;
        private bool _running;
        private int _cachedInterval;
        private float _timer;
        private bool _intervalPassed;

        public override void OnCreated(IThreading threading)
        {
            try
            {
                _modConfig = ModConfig.Instance;
                _simulationManager = Singleton<SimulationManager>.instance;
                _buildingManager = Singleton<BuildingManager>.instance;
                _buildingIds = new List<ushort>();
            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] Threading:OnCreated -> Exception: " + e.Message);
            }
        }

        public override void OnReleased()
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] Threading:OnReleased -> Exception: " + e.Message);
            }
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            try
            {
                if (!_running)
                {
                    switch (_modConfig.Interval)
                    {
                        case 1:
                            _intervalPassed = _simulationManager.m_currentGameTime.Day != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Day;
                            break;
                        case 2:
                            _intervalPassed = _simulationManager.m_currentGameTime.Month != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Month;
                            break;
                        case 3:
                            _intervalPassed = _simulationManager.m_currentGameTime.Year != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Year;
                            break;
                        case 4:
                            _timer += realTimeDelta;
                            if (_timer > 5f)
                            {
                                _timer = _timer - 5f;
                                _intervalPassed = true;
                            }
                            break;
                        case 5:
                            _timer += realTimeDelta;
                            if (_timer > 10f)
                            {
                                _timer = _timer - 10f;
                                _intervalPassed = true;
                            }
                            break;
                        case 6:
                            _timer += realTimeDelta;
                            if (_timer > 30f)
                            {
                                _timer = _timer - 30f;
                                _intervalPassed = true;
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (_intervalPassed)
                {
                    _running = true;

                    _intervalPassed = false;

                    _buildingIds.Clear();

                    if (_modConfig.EmptyLandfillSites)
                    {
                        _serviceBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.Garbage);

                        _buildingIds.AddRange(EmptyServiceBuildingsIfThresholdExceeded(_serviceBuildings));
                    }

                    if (_modConfig.EmptyCemeteries)
                    {
                        _serviceBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.HealthCare);

                        _buildingIds.AddRange(EmptyServiceBuildingsIfThresholdExceeded(_serviceBuildings));
                    }

                    if (_modConfig.EmptySnowDumps)
                    {
                        _serviceBuildings = _buildingManager.GetServiceBuildings(ItemClass.Service.Road);

                        _buildingIds.AddRange(EmptyServiceBuildingsIfThresholdExceeded(_serviceBuildings));
                    }

                    EmptyUtils.EmptyBuildings(_buildingIds);

                    _running = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] Threading:OnUpdate -> Exception: " + e.Message);
                _running = false;
            }
        }

        private List<ushort> EmptyServiceBuildingsIfThresholdExceeded(FastList<ushort> serviceBuildings)
        {
            List<ushort> buildingsIds = new List<ushort>();

            try
            {
                _buildings = _buildingManager.m_buildings.m_buffer;

                int capacity;
                int amount;
                float percentage;

                foreach (ushort buildingId in _serviceBuildings)
                {
                    _building = _buildings[buildingId];

                    if ((_building.m_flags & Building.Flags.Downgrading) == Building.Flags.None)
                    {
                        _buildingAI = _buildings[buildingId].Info.m_buildingAI;

                        if (_buildingAI is LandfillSiteAI && _buildingAI.CanBeEmptied())
                        {
                            _landfillSiteAI = _buildingAI as LandfillSiteAI;

                            capacity = _landfillSiteAI.m_garbageCapacity;
                            amount = Mathf.Min(capacity, _building.m_customBuffer1 * 1000 + _building.m_garbageBuffer);
                            percentage = ((float)amount / (float)capacity) * 100;

                            if (percentage > _modConfig.ThresholdLandfillSites)
                            {
                                buildingsIds.Add(buildingId);
                            }
                        }
                        else if (_buildingAI is CemeteryAI && _buildingAI.CanBeEmptied())
                        {
                            _cemeteryAI = _buildingAI as CemeteryAI;

                            capacity = _cemeteryAI.m_graveCount;
                            amount = _building.m_customBuffer1;
                            percentage = ((float)amount / (float)capacity) * 100;

                            if (percentage > _modConfig.ThresholdCemeteries)
                            {
                                buildingsIds.Add(buildingId);
                            }
                        }
                        else if (_buildingAI is SnowDumpAI && _buildingAI.CanBeEmptied())
                        {
                            _snowDumpAI = _buildingAI as SnowDumpAI;

                            capacity = _snowDumpAI.m_snowCapacity;
                            amount = Mathf.Min(capacity, _building.m_customBuffer1 * 1000 + _building.m_garbageBuffer);
                            percentage = ((float)amount / (float)capacity) * 100;

                            if (percentage > _modConfig.ThresholdSnowDumps)
                            {
                                buildingsIds.Add(buildingId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] Threading:EmptyServiceBuildingsIfThresholdExceeded -> Exception: " + e.Message);
            }

            return buildingsIds;
        }
    }
}