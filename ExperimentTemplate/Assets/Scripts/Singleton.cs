using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T instance;
	
	/// <summary>
    /// Returns the instance of this singleton. User is responsible for adding an instance to the scene.
    /// </summary>
	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (T) FindObjectOfType(typeof(T));
				
				if (instance == null)
				{
					Debug.LogError("An instance of " + typeof(T) + 
					               " is needed in the scene, but there is none.");
                    throw new UnityException("An instance of " + typeof(T) +
                                   " is needed in the scene, but there is none.");
				}
			}			
			return instance;
		}
	}
}