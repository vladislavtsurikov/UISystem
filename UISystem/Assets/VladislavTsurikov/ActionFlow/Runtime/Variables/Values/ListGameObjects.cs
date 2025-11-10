using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Variables
{
    [CreateAssetMenu(fileName = "New List of GameObjects Variable", menuName = "Variables/List of GameObjects")]
    public class ListGameObjects : Variable<List<GameObject>>
    {
    }
}
