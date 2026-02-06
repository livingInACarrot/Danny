using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordCard", menuName = "Scriptable Objects/WordCard")]
public class WordCard : ScriptableObject
{
    public List<string> words;
}
