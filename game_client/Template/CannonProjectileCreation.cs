﻿using Avalonia.Media;
using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_client.Models.CanvasItems;
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
            var mat = (dynamic)materials;
            return new Projectile(position, mat.Color, mat.Size, mat.Size);
        }

        protected override void FinalizeConstruction(Projectile projectile)
        {
            base.FinalizeConstruction(projectile);
            projectile.setExplosionRange(5);
        }
    }
}
