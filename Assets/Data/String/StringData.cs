using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class StringData : ScriptableObject
{
	public List<StringDBEntity> Messages; // Replace 'EntityType' to an actual type that is serializable.
}
