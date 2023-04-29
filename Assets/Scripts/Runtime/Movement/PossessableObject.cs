using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Movement
{
    public class PossessableObject : MonoBehaviour
    {
        [Label("Input")]
        public List<MonoBehaviour> inputables;

        [Label("Events")]
        public UnityEvent OnPossess;
        public UnityEvent OnLeave;
    }
}