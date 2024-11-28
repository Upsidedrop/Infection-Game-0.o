using UnityEngine;

public class ItemFunctions : MonoBehaviour
{
    public LayerMask enemyLayer;
    public void UseItem(InventoryManager.Item type)
    {
        switch (type)
        {
            case InventoryManager.Item.Beeper:
                if(Physics.CheckSphere(transform.position, 25, enemyLayer))
                {
                    print("You've been heard.");
                }
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 25);
    }
}
