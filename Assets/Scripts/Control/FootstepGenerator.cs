using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepGenerator : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] GameObject footstepPrefab = null;
    [SerializeField] [Range(16, 256)] int maxFootsteps = 52;
    [SerializeField] PlayerControllerPC playerController = null;
    #endregion

    #region parameters
    struct Footstep
    {
        public GameObject gameObject;
        public Material material;
    }
    Queue<Footstep> footstepQueue = new Queue<Footstep>();

    Color newestColor = new Color(0.1f, 0.1f, 0.12f);
    Color oldestColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    #endregion

    #region unity methods

    private void Awake()
    {
        playerController.OnStepCallback += StepFootstep;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region footstep path
    
    Vector3 FootstepPositionProjectedFrom(Vector3 from)
    {
        if(Physics.Raycast(from, -Vector3.up, out RaycastHit info, 1.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            return info.point;
        }

        return from;
    }
    
    Vector3 NoisyForwardFootstep(Vector3 footstep)
    {
        float forwardScale = 0.1f;
        float noiseScale = 0.3f;
        return footstep + forwardScale * playerController.transform.forward + noiseScale * Random.Range(-1.0f, 1.0f) * playerController.transform.right;
    }

    void StepFootstep()
    {
        // add footsteps until full.
        if(footstepQueue.Count < maxFootsteps)
        {
            Footstep footstep;
            footstep.gameObject = GameObject.Instantiate(footstepPrefab);
            footstep.gameObject.transform.position = NoisyForwardFootstep(FootstepPositionProjectedFrom(playerController.playerBase));
            footstep.material = footstep.gameObject.GetComponent<Renderer>().material;
            footstepQueue.Enqueue(footstep);
        }

        // once full, cycle the footsteps.
        else
        {
            Footstep oldest = footstepQueue.Dequeue();
            oldest.gameObject.transform.position = NoisyForwardFootstep(FootstepPositionProjectedFrom(playerController.playerBase));
            footstepQueue.Enqueue(oldest);
        }

        // age the footsteps.
        int count = 0;
        foreach(Footstep f in footstepQueue)
        {
            f.material.color = Color.Lerp(oldestColor, newestColor, (float)count / (float)footstepQueue.Count);
            count++;
        }

    }
    #endregion
}
