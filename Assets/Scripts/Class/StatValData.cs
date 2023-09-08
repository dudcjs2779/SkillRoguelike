using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatValData
{
    public float STR_meleePower, STR_health;
    public float AGL_meleePower, AGL_stamina;
    public float VIT_meleeStgDam, VIT_meleeStgPower, VIT_health;
    public float INT_effectDuration, MST_element, INT_MP;
    public float MP_magicDam, MP_MP;
    public float INT_MpRecover, MST_magicStgDam, MST_MP;

    public StatValData()
    {

    }

    public StatValData(float _STR_meleePower, float _STR_health,
        float _AGL_meleePower, float _AGL_stamina,
        float _VIT_meleeStgDam, float _VIT_meleeStgPower, float _VIT_health,
        float _INT_effectDuration, float _INT_element, float _INT_MP,
        float _MP_magicPower, float _MP_MP,
        float _MST_MPrecover, float _MST_magicStgDam, float _MST_MP)
    {
        STR_meleePower = _STR_meleePower;
        STR_health = _STR_health;
        AGL_meleePower = _AGL_meleePower;
        AGL_stamina = _AGL_stamina;
        VIT_meleeStgDam = _VIT_meleeStgDam;
        VIT_meleeStgPower = _VIT_meleeStgPower;
        VIT_health = _VIT_health;
        INT_effectDuration = _INT_effectDuration;
        MST_element = _INT_element;
        INT_MP = _INT_MP;
        MP_magicDam = _MP_magicPower;
        MP_MP = _MP_MP;
        INT_MpRecover = _MST_MPrecover;
        MST_magicStgDam = _MST_magicStgDam;
        MST_MP = _MST_MP;
    }

    public void StatUpdate(PlayerData playerData){
        switch (playerData.STR)
        {
            case "E":
                STR_meleePower = 0;
                STR_health = 0;
                break;

            case "D":
                STR_meleePower = 0.04f;
                STR_health = 4;
                break;

            case "C":
                STR_meleePower = 0.08f;
                STR_health = 8;
                break;

            case "B":
                STR_meleePower = 0.13f;
                STR_health = 13;
                break;

            case "A":
                STR_meleePower = 0.2f;
                STR_health = 20;
                break;
        }

        switch (playerData.AGL)
        {
            case "E":
                AGL_meleePower = 0;
                AGL_stamina = 0;
                break;

            case "D":
                AGL_meleePower = 0.04f;
                AGL_stamina = 4;
                break;

            case "C":
                AGL_meleePower = 0.08f;
                AGL_stamina = 8;
                break;

            case "B":
                AGL_meleePower = 0.13f;
                AGL_stamina = 13;
                break;

            case "A":
                AGL_meleePower = 0.2f;
                AGL_stamina = 20;
                break;
        }

        switch (playerData.VIT)
        {
            case "E":
                VIT_meleeStgDam = 0f;
                VIT_meleeStgPower = 0;
                break;

            case "D":
                VIT_meleeStgDam = 0.04f;
                VIT_meleeStgPower = 0.04f;
                break;

            case "C":
                VIT_meleeStgDam = 0.08f;
                VIT_meleeStgPower = 0.08f;
                break;

            case "B":
                VIT_meleeStgDam = 0.13f;
                VIT_meleeStgPower = 0.13f;
                break;

            case "A":
                VIT_meleeStgDam = 0.2f;
                VIT_meleeStgPower = 0.2f;
                break;
        }

        switch (playerData.INT)
        {
            case "E":
                INT_effectDuration = 0;
                INT_MpRecover = 0;
                break;

            case "D":
                INT_effectDuration = 0.06f;
                INT_MpRecover = 0.06f;
                break;

            case "C":
                INT_effectDuration = 0.12f;
                INT_MpRecover = 0.12f;
                break;

            case "B":
                INT_effectDuration = 0.2f;
                INT_MpRecover = 0.2f;
                break;

            case "A":
                INT_effectDuration = 0.3f;
                INT_MpRecover = 0.3f;
                break;
        }

        switch (playerData.MP)
        {
            case "E":
                MP_magicDam = 0;
                MP_MP = 0;
                break;

            case "D":
                MP_magicDam = 0.06f;
                MP_MP = 4;
                break;

            case "C":
                MP_magicDam = 0.12f;
                MP_MP = 8;
                break;

            case "B":
                MP_magicDam = 0.2f;
                MP_MP = 13;
                break;

            case "A":
                MP_magicDam = 0.3f;
                MP_MP = 20;
                break;
        }

        switch (playerData.MST)
        {
            case "E":
                MST_element = 0;
                MST_magicStgDam = 0;
                break;

            case "D":
                MST_element = 0.04f;
                MST_magicStgDam = 0.04f;
                break;

            case "C":
                MST_element = 0.08f;
                MST_magicStgDam = 0.08f;
                break;

            case "B":
                MST_element = 0.13f;
                MST_magicStgDam = 0.13f;
                break;

            case "A":
                MST_element = 0.2f;
                MST_magicStgDam = 0.2f;
                break;
        }

    }

    public float GetHealth(){
        return STR_health;
    }

    public float GetMeleeDam(){
        return STR_meleePower + AGL_meleePower;
    }

    public float GetStamina()
    {
        return AGL_stamina;
    }

    public float GetMeleeStgDam()
    {
        return VIT_meleeStgDam;
    }

    public float GetMeleeStgPower()
    {
        return VIT_meleeStgPower;
    }

    public float GetEffectDuration()
    {
        return INT_effectDuration;
    }

    public float GetElement()
    {
        return MST_element;
    }

    public float GetMp()
    {
        return MP_MP;
    }

    public float GetMagicPower()
    {
        return MP_magicDam;
    }

    public float GetMagicStgDam()
    {
        return MST_magicStgDam;
    }

    public float GetMpRecover()
    {
        return INT_MpRecover;
    }

}


