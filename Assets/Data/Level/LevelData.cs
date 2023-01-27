using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class LevelData : ScriptableObject
{
	public List<LevelDBEntity> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
