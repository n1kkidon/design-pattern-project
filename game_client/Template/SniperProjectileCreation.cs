using Avalonia.Media;
using game_client.Models.CanvasItems;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Template
{
    public sealed class SniperProjectileCreation : ProjectileCreationTemplate
    {
        protected override object PrepareMaterials()
        {
            // Prepare materials specific to Sniper projectiles.
            return new { Color = Colors.Red, Size = 5 };
        }

        protected override Projectile ConstructProjectile(object materials, Vector2 position)
        {
            var mat = (dynamic)materials;
            return new Projectile(position, mat.Color, mat.Size, mat.Size);
        }
    }
}
