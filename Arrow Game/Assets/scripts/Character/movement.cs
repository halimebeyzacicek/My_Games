using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]

public class movement : MonoBehaviour
{
    CharacterController cc;
    Animator anim;

    [System.Serializable]
    public class AnimationStrings
    {
        public string forward = "forward";//animator->parameters isimleri   //geri-ileri
        public string strafe = "strafe";//sag-sol
        public string sprint = "sprint";//walk-run
        public string aim = "aim";//niþan almak
        public string pull = "pullString";
        public string fire = "fire";
    }
    [SerializeField]
    public AnimationStrings animationStrings;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateCharacter(float forward,float strafe)
    {
        anim.SetFloat(animationStrings.forward, forward);//ileri-geri
        anim.SetFloat(animationStrings.strafe, strafe);//sag-sol
    }
    public void SprintCharacter(bool isSprinting)
    {
        anim.SetBool(animationStrings.sprint, isSprinting);
    }
    public void CharacterAim(bool aiming)
    {
        anim.SetBool(animationStrings.aim, aiming);
    }

    public void CharacterPullString(bool pull)
    {
        //Debug.Log("characterpull geldi");
        anim.SetBool(animationStrings.pull, pull);
    }
    public void CharacterFireArrow()
    {
        anim.SetTrigger(animationStrings.fire);
    }

}
