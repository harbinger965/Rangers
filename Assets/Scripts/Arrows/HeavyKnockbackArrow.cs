using UnityEngine;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// When it hits a player, applies more knockback - outward from point of impact
    /// </summary>

    public class HeavyKnockbackArrow : ArrowProperty
    {
        // Force of the explosion
        private float HeavyKnockbackForce = 850f;
        // Radius of the explosion
        private float HeavyKnockbackRadius = 50f;

        public override void Effect(PlayerID hitPlayer)
        {
            //applies explosive force iff a player was hit
            if (hitPlayer != 0)
            {
                Player.Controller hitPlayerController = Data.GameManager.instance.GetPlayer(hitPlayer);
                hitPlayerController.GetComponent<Rigidbody>().AddExplosionForce(HeavyKnockbackForce, transform.position, HeavyKnockbackRadius);
            }
        }
    }
}