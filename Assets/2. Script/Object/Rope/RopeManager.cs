using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RopeManager : MonoBehaviour
{
    public GameObject parentObject = null;
    public GameObject lastObject = null;

    public int ropeCount = 0;
    private int deleteIndex = 0;

    internal bool isFire = false;

    private List<GameObject> ropes = new List<GameObject>();
    void Start()
    {
        CreateRope();
    }

    public void CreateRope()
    {
        Reset();

        for (int i = 0; i < ropeCount; i++)
        {
            GameObject rope = ObjectPool.Instance.GetObject(PoolObjectType.ROPE);
            rope.transform.SetParent(this.transform);
            ropes.Add(rope);
        }
        SetRotation();
    }

    public void SetRotation()
    {
        for (int i = 0; i < ropes.Count; i++)
            ropes[i].transform.rotation = Quaternion.Euler(Vector3.zero);

        parentObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        lastObject.transform.rotation = Quaternion.Euler(Vector3.zero);

        SetRopePosition();
    }

    public void SetRopeSetting()
    {
        for(int i = 0; i < ropeCount; i++)
        {
            ropes[i].GetComponent<RopeCollision>().SetIndex(i);
        }

        for(int i = 0; i < ropeCount; i++)
        {
            ropes[i].GetComponent<HingeJoint2D>().anchor = new Vector2(0, .5f);
        }

        for(int i = ropeCount - 1; i > 0; i--)
        {
            ropes[i].GetComponent<HingeJoint2D>().connectedBody = ropes[i - 1].GetComponent<Rigidbody2D>();
        }

        ropes[0].GetComponent<HingeJoint2D>().connectedBody = parentObject.GetComponent<Rigidbody2D>();

        lastObject.GetComponent<HingeJoint2D>().connectedBody = ropes[ropes.Count - 1].GetComponent<Rigidbody2D>();

        for (int i = ropeCount - 1; i > 0; i--)
        {
            ropes[i].GetComponent<DistanceJoint2D>().connectedBody = ropes[i - 1].GetComponent<Rigidbody2D>();
        }
        ropes[0].GetComponent<DistanceJoint2D>().connectedBody = parentObject.GetComponent<Rigidbody2D>();
    }

    public void SetRopePosition()
    {
        float parentPos = parentObject.transform.position.y - (parentObject.transform.localScale.y * .5f);
        ropes[0].transform.position = new Vector2(parentObject.transform.position.x, parentPos);

        for(int i = 1; i < ropeCount; i++)
        {
            ropes[i].transform.position = new Vector2(ropes[i - 1].transform.position.x,
                ropes[i - 1].transform.position.y - ropes[i].transform.localScale.y * .5f - ropes[i].transform.localScale.y * .2f);
        }

        float lastPos = ropes[ropes.Count - 1].transform.position.y -
            (ropes[ropes.Count - 1].transform.localScale.y * .5f) - (lastObject.transform.localScale.y * .5f);
        lastObject.transform.position = new Vector2(parentObject.transform.position.x, lastPos);

        SetRopeSetting();
    }

    IEnumerator RopeFire(int index)
    {
        isFire = true;

        ObjectPool.Instance.ReturnObject(PoolObjectType.ROPE, ropes[index]);

        for (int i = 1; i < ropes.Count; i++)
        {
            yield return new WaitForSeconds(.05f);
            if (index - i >= 0)
            {
                ObjectPool.Instance.ReturnObject(PoolObjectType.ROPE, ropes[index - i]);
            }
            if (index + i < ropeCount)
            {
                ObjectPool.Instance.ReturnObject(PoolObjectType.ROPE, ropes[index + i]);
        }
            }

        ropes.Clear();

        isFire = false;
    }

    public void SetDeleteIndex(int index)
    {
        deleteIndex = index;
        StartCoroutine(RopeFire(deleteIndex));
    }

    public void Reset()
    {
        if(ropes.Count != 0)
        {
            for (int i = 0; i < ropes.Count; i++)
                ObjectPool.Instance.ReturnObject(PoolObjectType.ROPE, ropes[i]); 

            ropes.Clear();
        }
    }
}
