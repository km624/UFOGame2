using System.Collections.Generic;
//using UnityEngine;

[System.Serializable]
public class UserUFOData
{
    public string UFOName;             
    //public bool IsUnlocked;          
    public int ReinforceMoveSpeed;       
    public int ReinforceLiftSpeed;
    public int ReinforceBeamRange;

    public int MaxReinforceMoveSpeed;
    public int MaxReinforceLiftSpeed;
    public int MaxReinforceBeamRange;

    public List<int> OwnedColorIndexes = new(); 
    public int CurrentColorIndex = 0;

    //public bool AllStat = false;

    public UserUFOData()
    {
        UFOName = "UFONormal";

        ReinforceMoveSpeed = 0;
        ReinforceLiftSpeed = 0;
        ReinforceBeamRange = 0;

        MaxReinforceMoveSpeed = 1;
        MaxReinforceLiftSpeed = 1;
        MaxReinforceBeamRange = 1;

        OwnedColorIndexes.Add(0);

        CurrentColorIndex = 0;

        //AllStat = false;
    }
    public UserUFOData(UFOData ufoData)
    {
        UFOName = ufoData.name;

        ReinforceMoveSpeed = 0;
        ReinforceLiftSpeed = 0;
        ReinforceBeamRange = 0;

        MaxReinforceMoveSpeed = ufoData.MaxMoveSpeed - ufoData.BaseMoveSpeed;
        MaxReinforceLiftSpeed = ufoData.MaxLiftSpeed - ufoData.BaseLiftSpeed;
        MaxReinforceBeamRange = ufoData.MaxBeamRange - ufoData.BaseBeamRange;


        OwnedColorIndexes.Add(0);

        CurrentColorIndex = 0;

        //AllStat = false;
    }

    public void AddColor(int colorIndex)
    {
        CurrentColorIndex = colorIndex;
        
        OwnedColorIndexes.Add(colorIndex);

    }

    public bool AllStat()
    {
        return ReinforceMoveSpeed >= MaxReinforceMoveSpeed &&
          ReinforceLiftSpeed >= MaxReinforceLiftSpeed &&
          ReinforceBeamRange >= MaxReinforceBeamRange;

    }



    
}



