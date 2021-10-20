using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    GameObject thisTree;
    public int treeHealth = 5;
    private bool isFallen = false;

    private void Start()
    {
        thisTree = transform.parent.gameObject;
    }

   
  

    // Update is called once per frame
    private void Update()
    {
        if (treeHealth <= 0 && isFallen == false)
        {
            Rigidbody rb = thisTree.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.forward, ForceMode.Impulse);
            isFallen = true;
        }
    }

    private IEnumerator destroyTree()
    {
        yield return new WaitForSeconds(10);
        Destroy(thisTree);
    }
}
