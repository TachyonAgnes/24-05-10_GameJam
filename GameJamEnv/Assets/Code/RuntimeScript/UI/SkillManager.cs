using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public Image Ult;
    public Image Skill1;
    public Image Skill2;
    public Image Passive;
    public Color grey;
    public Color red;

    public float ultCooldown = 5f;
    public float skill1Cooldown = 3f;
    public float skill2Cooldown = 3f;
    public float passiveCooldown = 0.1f;

    private float ultTimer;
    private float skill1Timer;
    private float skill2Timer;
    private float passiveTimer;

    private bool ultReady = false;
    private bool skill1Ready = false;
    private bool skill2Ready = false;
    private bool passiveReady = false;

 

    void Start()
    {
        SetSkillInitialColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && ultReady)
        {
            UseSkill(Ult, ref ultTimer, ref ultReady);
            Debug.Log("Ultimate skill used!");
        }

        if (Input.GetKeyDown(KeyCode.J) && skill1Ready)
        {
            UseSkill(Skill1, ref skill1Timer, ref skill1Ready);
            Debug.Log("Skill 1 used!");
        }

        if (Input.GetKeyDown(KeyCode.K) && skill2Ready)
        {
            UseSkill(Skill2, ref skill2Timer, ref skill2Ready);
            Debug.Log("Skill 2 used!");
        }

        UpdateSkillCooldown(Ult, ref ultTimer, ref ultReady, ultCooldown);
        UpdateSkillCooldown(Skill1, ref skill1Timer, ref skill1Ready, skill1Cooldown);
        UpdateSkillCooldown(Skill2, ref skill2Timer, ref skill2Ready, skill2Cooldown);
        UpdateSkillCooldown(Passive, ref passiveTimer, ref passiveReady, passiveCooldown);
    }

    void SetSkillInitialColor()
    {
        Ult.color = grey;
        Skill1.color = grey;
        Skill2.color = grey;
        Passive.color = grey;
    }

    void UseSkill(Image skillImage, ref float timer, ref bool isReady)
    {
        skillImage.color = grey; // Set the color to grey when skill is used
        timer = 0; // Reset the timer to start the cooldown
        isReady = false; // Mark the skill as not ready
    }

    void UpdateSkillCooldown(Image skillImage, ref float timer, ref bool isReady, float cooldown)
    {
        if (!isReady)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                timer = cooldown; // Ensuring the timer does not exceed the cooldown period
                skillImage.color = red; // Change color to red when the skill is ready
                isReady = true; // Mark the skill as ready
            }
        }
    }
}
