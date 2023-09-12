using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sodalite.Api;
using Sodalite.Utilities;
using localpcnerd.OnslaughtMode;
#if H3VR_IMPORTED
using FistVR;

namespace localpcnerd.OnslaughtMode 
{
	public class Spawner : MonoBehaviour
	{
		public SosigEnemyID[] spawnableSosigs;
		public OnslaughtManager osm;

        private void Update()
        {
            if(osm == null)
            {
				osm = FindObjectOfType<OnslaughtManager>();
            }
        }

		private readonly SosigAPI.SpawnOptions _spawnOptions = new SosigAPI.SpawnOptions
		{
			SpawnState = Sosig.SosigOrder.Assault,
			SpawnActivated = true,
			EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
			SpawnWithFullAmmo = true,
			IFF = 1
		};

		public void Spawn()
        {
			if(osm.spawnedSosigs.Count >= osm.maxSosigs)
            {
				return;
            }

			SosigEnemyID id = spawnableSosigs.GetRandom();
			_spawnOptions.SosigTargetPosition = osm.attackPoints.GetRandom().position;

			var sosig = SosigAPI.Spawn(IM.Instance.odicSosigObjsByID[id], _spawnOptions, transform.position, transform.rotation);
			var st = sosig.gameObject.AddComponent<SosigMarker>();
			osm.spawnedSosigs.Add(st);
		}
	}
}
#endif