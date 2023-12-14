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
    public abstract class ProjectileCreationTemplate
    {
        // The template method defines the steps of the algorithm.
        public Projectile CreateProjectile(Vector2 position)
        {
            var materials = PrepareMaterials();
            var projectile = ConstructProjectile(materials, position);
            FinalizeConstruction(projectile);
            return projectile;
        }

        // Abstract method for preparing materials. This needs to be implemented by subclasses.
        protected abstract object PrepareMaterials();

        // Abstract method for constructing the projectile. This needs to be implemented by subclasses.
        protected abstract Projectile ConstructProjectile(object materials, Vector2 position);

        protected virtual void FinalizeConstruction(Projectile projectile)
        {
            projectile.setFinalized(true);
        }
    }
}
