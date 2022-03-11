using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");       // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");         // -1, 0, 1
        Vector3 dir = new Vector3(h, 0, v);
        Debug.LogFormat("dir: {0}", dir);

        if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
        {
            if (dir == Vector3.zero) {
                this.anim.Play("StandA_idleA", -1, 0);
                this.anim.SetBool("IsRun", false);
            }
        }

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            this.transform.eulerAngles = Vector3.up * angle;
        }

        if (dir != Vector3.zero)
        {
            this.anim.SetBool("IsRun", true);
            this.transform.Translate(Vector3.forward * 1f * Time.deltaTime);
        }



    }
}
