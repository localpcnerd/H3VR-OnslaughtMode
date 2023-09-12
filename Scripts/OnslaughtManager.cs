using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sodalite.Api;
using Sodalite.Utilities;
using UnityEngine.UI;
using localpcnerd.OnslaughtMode;
using System;
#if H3VR_IMPORTED
using FistVR;

namespace localpcnerd.OnslaughtMode
{
	public class OnslaughtManager : MonoBehaviour
	{
		[Header("Sosig Settings")]
		public List<Spawner> sosigSpawners = new List<Spawner>();
		public Transform[] attackPoints;
		[HideInInspector] public List<SosigMarker> spawnedSosigs = new List<SosigMarker>();

		[Header("Game Settings")]
		public Transform teleportPoint;
		public Transform endgamePoint;
		[HideInInspector] public int maxSosigs;
		public bool gameRunning;
		public difficulty gameDifficulty = difficulty.med; //defaults to med
		public int sosigsEasy = 5;
		public int sosigsMed = 8;
		public int sosigsHard = 10;

		[Header("UI")]
		public Text sosigCount;
		public Text[] timerTexts;
		public Text killsText;
		public Text finalScoreText;

		[HideInInspector] public int kills;
		[HideInInspector] public float score;
		private float startTime;
		private bool runtimer = false;
		private float elapsed;
		private TimeSpan timePlaying;

        public enum difficulty
		{
			easy,
			med,
			hard
		}

        private void Awake()
        {
			gameRunning = false;
			runtimer = false;
        }

        private void Update()
        {
            if(gameDifficulty == difficulty.easy)
            {
				maxSosigs = sosigsEasy;
            }
			else if(gameDifficulty == difficulty.med)
            {
				maxSosigs = sosigsMed;
            }
			else if (gameDifficulty == difficulty.hard)
			{
				maxSosigs = sosigsHard;
			}
			else
            {
				maxSosigs = sosigsEasy;
            }

			sosigCount.text = "Sosigs: " + maxSosigs.ToString();
			killsText.text = "Kills: " + kills.ToString();
			finalScoreText.text = "Final score: " + score.ToString();

			foreach (SosigMarker t in spawnedSosigs)
			{
				if(t.sosig.BodyState == Sosig.SosigBodyState.Dead)
                {
					t.sosig.ClearSosig();
					spawnedSosigs.Remove(t);
					kills++;
                }
			}

			if (spawnedSosigs.Count <= maxSosigs / 2 && gameRunning)
			{
				CallSpawns();
			}

			if (GM.CurrentPlayerBody.Health <= 0)
			{
				GM.CurrentSceneSettings.DeathResetPoint = endgamePoint.transform;
				EndGame();
			}
		}

		public void startTimer()
        {
			startTime = Time.time;
			runtimer = true;

			StartCoroutine(RunTimer());
        }

		public IEnumerator RunTimer()
        {
			while (runtimer) //timer
			{
				elapsed += Time.deltaTime;
				timePlaying = TimeSpan.FromSeconds(elapsed);
				string tps = timePlaying.Hours + ":" + timePlaying.Minutes + ":" + timePlaying.Seconds;

				foreach(Text t in timerTexts)
                {
					t.text = tps;
                }

				yield return null;
			}
		}

        public void StartGame()
        {
			GM.CurrentMovementManager.TeleportToPoint(teleportPoint.position, true, teleportPoint.forward);
			initialSpawnCall();
			gameRunning = true;
			startTimer();
		}

		public void EndGame()
        {
			gameRunning = false;
			foreach(SosigMarker sm in spawnedSosigs)
            {
				sm.sosig.ClearSosig();
            }
			runtimer = false;
			CalcScore();
        }

		public void CallSpawns()
        {
			if(!gameRunning)
            {
				return;
            }

			foreach(Spawner sp in sosigSpawners)
            {
				if(spawnedSosigs.Count >= maxSosigs)
                {
					return;
                }

				sp.Spawn();
			}
        }

		public IEnumerator spawnDelay(float t)
        {
			yield return new WaitForSeconds(t);
			CallSpawns();
        }

		public void initialSpawnCall()
        {
			float random = UnityEngine.Random.Range(5f, 10f);

			StartCoroutine(spawnDelay(random));
		}

		public void SetDifficulty(int dif)
        {
			if (dif == 0)
			{
				gameDifficulty = difficulty.easy;
			}
			else if (dif == 1)
			{
				gameDifficulty = difficulty.med;
			}
			else if (dif == 2)
			{
				gameDifficulty = difficulty.hard;
			}
			else
			{
				gameDifficulty = difficulty.easy; //failsafe
			}
		}

		public void SetHealth(int hp)
		{
			if (hp == 0)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(100);
				GM.CurrentPlayerBody.ResetHealth();
			}
			else if (hp == 1)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(1000);
				GM.CurrentPlayerBody.ResetHealth();
			}
			else if (hp == 2)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(5000);
				GM.CurrentPlayerBody.ResetHealth();
			}
			else if (hp == 3)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(10000);
				GM.CurrentPlayerBody.ResetHealth();
			}
			else
            {
				GM.CurrentPlayerBody.SetHealthThreshold(100);
				GM.CurrentPlayerBody.ResetHealth();
			}
		}

		public void CalcScore()
        {
			float sc = kills * elapsed;
			sc /= 10;
			score = sc;
			score = Mathf.Round(sc * 100f) / 100f;
        }
	}
}
#endif