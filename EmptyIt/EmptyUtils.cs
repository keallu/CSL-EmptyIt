using ColossalFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmptyIt
{
    public static class EmptyUtils
    {
        public static void EmptyBuildings(List<ushort> list)
        {
            try
            {
                SimulationManager simulationManager = Singleton<SimulationManager>.instance;

                foreach (ushort buildingId in list)
                {
                    simulationManager.AddAction(EmptyBuilding(buildingId));
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] EmptyUtils:EmptyBuildings -> Exception: " + e.Message);
            }
        }

        private static IEnumerator EmptyBuilding(ushort buildingId)
        {
            try
            {
                BuildingManager buildingManager = Singleton<BuildingManager>.instance;
                buildingManager.m_buildings.m_buffer[buildingId].Info.m_buildingAI.SetEmptying(buildingId, ref buildingManager.m_buildings.m_buffer[buildingId], true);
            }
            catch (Exception e)
            {
                Debug.Log("[Empty It!] EmptyUtils:EmptyBuilding -> Exception: " + e.Message);
            }

            yield return null;
        }
    }
}
