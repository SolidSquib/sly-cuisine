using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD41;

public class PlayerControl : MonoBehaviour {

	List<CharacterScript> ActiveCharacters = new List<CharacterScript>();

	// Use this for initialization
	void Start()
    {
		if (InputManager.Singleton)
		{
			InputManager.Singleton.RegisterAxisEvent("Horizontal", MoveRight);
			InputManager.Singleton.RegisterAxisEvent("Vertical", MoveUp);
		}

		CharacterScript[] StartCharacters = FindObjectsOfType<CharacterScript>();
		foreach (CharacterScript Character in StartCharacters)
		{
			if (Character.PossessOnStartup)
			{
				Possess(Character);
			}
		}

        LevelManager.Singleton.UpdateLives(ActiveCharacters.Count);
    }

    void Disable()
	{
		if (InputManager.Singleton)
		{
			InputManager.Singleton.UnregisterAxisEvent("Horizontal", MoveRight);
			InputManager.Singleton.UnregisterAxisEvent("Vertical", MoveUp);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="Character"></param>
	/// <returns></returns>
	void Possess(CharacterScript Character)
	{
		if(!ActiveCharacters.Contains(Character))
		{
			ActiveCharacters.Add(Character);
			Character.RegisterOnDeathCallback(Unpossess);
		}			
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="Character"></param>
	/// <returns></returns>
	void Unpossess(CharacterScript Character)
	{
        if (ActiveCharacters.Contains(Character))
        {
            ActiveCharacters.Remove(Character);
        }
    }

	void MoveRight(float Value)
	{
		foreach (CharacterScript Character in ActiveCharacters)
		{
			Character.MoveRight(Value);
		}
	}

	void MoveUp(float Value)
	{
		foreach (CharacterScript Character in ActiveCharacters)
		{
			Character.MoveUp(Value);
		}
	}
}
