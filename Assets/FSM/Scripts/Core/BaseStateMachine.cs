using com.lineact.lit.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace com.lineact.lit.FSM
{
    public class BaseStateMachine : MonoBehaviourCachedComponent
    {
        [SerializeField] private BaseState _initialState;
        public BaseState CurrentState;
        private void Awake()
        {
            CurrentState = _initialState;
        }
        private void Start()
        {
            CurrentState.Enter(this);
        }
        private void Update()
        {
            CurrentState.Execute(this);
        }

        /// <summary>
        /// Change the state of the state machine. Call the Exit of the previous state and call the Enter of the new state
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(BaseState newState)
        {
            CurrentState.Exit(this);
            CurrentState = newState;
            CurrentState.Enter(this);
        }



        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public void SetData(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// R�cup�re une donn�e locale typ�e. Retourne default(T) si la cl� n'existe pas.
        /// </summary>
        public T GetData<T>(string key)
        {
            if (_data.TryGetValue(key, out object value) && value is T tValue)
                return tValue;

            return default;
        }

        /// <summary>
        /// Supprime une donn�e du stockage.
        /// </summary>
        public void RemoveData(string key)
        {
            _data.Remove(key);
        }

        /// <summary>
        /// Vide compl�tement le stockage (utile � la sortie d�un �tat).
        /// </summary>
        public void ClearData()
        {
            _data.Clear();
        }
    }
}