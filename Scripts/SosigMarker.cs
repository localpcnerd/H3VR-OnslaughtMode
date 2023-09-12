using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using localpcnerd.OnslaughtMode;
using FistVR;

namespace localpcnerd.OnslaughtMode
{
	public class SosigMarker : MonoBehaviour
	{
        public Sosig sosig;

        public void Awake()
        {
            sosig = GetComponent<Sosig>();
        }
    }
}