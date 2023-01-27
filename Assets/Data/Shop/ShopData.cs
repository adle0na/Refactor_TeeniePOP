using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class ShopData : ScriptableObject
{
	public List<ShopDBEntity> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
}
