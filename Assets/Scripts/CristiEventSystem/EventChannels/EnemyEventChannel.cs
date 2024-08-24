using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/Enemy Event Channel")]
    public class EnemyEventChannel: EventChannelObject
    {
        public UnityAction<GameObject> OnEnemyKilled;
        public UnityAction OnWaveCompleted;
        public UnityAction OnAllWaveCompleted;
        public UnityAction<float> OnWaveStart;
        public UnityAction OnStartNextWave;
    
        public UnityAction<int> OnEnemyAttack;

        public void RaiseEnemyKilled(GameObject enemy)
        {
            OnEnemyKilled?.Invoke(enemy);
        }

        public void RaiseWaveCompleted()
        {
            OnWaveCompleted?.Invoke();
        }

        public void RaiseAllWaveCompleted()
        {
            OnAllWaveCompleted?.Invoke();
        }

        public void RaiseWaveStart(float waveDelay)
        {
            OnWaveStart?.Invoke(waveDelay);
        }

        public void RaiseStartNextWave()
        {
            OnStartNextWave?.Invoke();
        }

        public void RaiseEnemyAttack(int damage)
        {
            OnEnemyAttack?.Invoke(damage);
        }
    }
}