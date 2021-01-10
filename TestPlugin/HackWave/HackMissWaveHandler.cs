using CustomExpeditions;
using CustomExpeditions.HandlerBase;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin.HackWave
{
    public class HackMissWaveHandler : CustomExpHandlerBase
    {
        public override void OnSetup()
        {
            Inject_HackFail.OnCalled = (node) =>
            {
                var waveSetting = new GenericEnemyWaveData()
                {
                    WaveSettings = 1,
                    WavePopulation = 5,
                    TriggerAlarm = false,
                    SpawnDelay = 3.0f
                };
                TriggerWave(waveSetting, node);
            };
        }

        public override void OnUnload()
        {
            Inject_HackFail.OnCalled = null;
        }
    }
}
