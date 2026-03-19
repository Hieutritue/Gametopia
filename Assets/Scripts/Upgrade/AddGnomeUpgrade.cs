using UnityEngine;

namespace DefaultNamespace.Upgrade
{
    [CreateAssetMenu(menuName = "Upgrade/Add Gnome Upgrade")]
    public class AddGnomeUpgrade : UpgradeData
    {
        public GameObject GnomePrefab;
        
        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            if (GnomePrefab != null)
            {
                GnomeSpawner.Instance.gnomePrefab = GnomePrefab;
                GnomeSpawner.Instance.SpawnGnome();
            }
        }
    }
}