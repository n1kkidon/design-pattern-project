using Avalonia.Media;
using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared;

namespace game_client.Template
{
    public sealed class CannonProjectileCreation : ProjectileCreationTemplate
    {
        protected override object PrepareMaterials()
        {
            // Prepare materials specific to Cannon projectiles.
            return new { Color = Colors.OrangeRed, Size = 30 };
        }

        protected override Projectile ConstructProjectile(object materials, Vector2 position)
        {
            // Use the materials to construct a new Cannon projectile.
            var mat = (dynamic)materials; // Cast to dynamic for simplicity; in production, use a typed class.
            return new Projectile(position, mat.Color, mat.Size, mat.Size);
        }

        // Optional: override the FinalizeConstruction if cannon projectiles have additional finalization steps.
        protected override void FinalizeConstruction(Projectile projectile)
        {
            // Call base method to ensure common finalization is done.
            base.FinalizeConstruction(projectile);

            // Additional finalization steps for a cannon projectile.
            // For example, setting an explosive charge property.
            projectile.setExplosionRange(5);
        }
    }
}
