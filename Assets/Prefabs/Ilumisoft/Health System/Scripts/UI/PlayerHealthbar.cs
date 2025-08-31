using UnityEngine;
using Ilumisoft.HealthSystem;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Player Healthbar")]
    [DefaultExecutionOrder(10)]
    public class PlayerHealthbar : Healthbar
    {
        public GameObject player;

        protected override void Start() // Використовуємо Start замість Awake
        {
            // Знаходимо гравця за тегом, якщо не встановлено
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            
            if (player != null)
            {
                Health = player.GetComponent<HealthComponent>();
                
                if (Health == null)
                {
                    Debug.LogError("HealthComponent не знайдено на гравцеві!");
                }
            }
            else
            {
                Debug.LogError("Гравець не знайдений!");
            }
            
            base.Start(); // Викликаємо базовий Start
        }
    }
}