using System;
using System.Collections;
using System.Collections.Generic;

namespace Sfs2X.FSM
{
	public class FiniteStateMachine
	{
		public delegate void OnStateChangeDelegate(int fromStateName, int toStateName);

		private List<FSMState> states = new List<FSMState>();

		private int currentStateName;

		public OnStateChangeDelegate onStateChange;

		private object locker = new object();

		public void AddState(object st)
		{
			int stateName = (int)st;
			FSMState fSMState = new FSMState();
			fSMState.SetStateName(stateName);
			states.Add(fSMState);
		}

		public void AddAllStates(Type statesEnumType)
		{
			IEnumerator enumerator = Enum.GetValues(statesEnumType).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					AddState(current);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void AddStateTransition(object from, object to, object tr)
		{
			int num = (int)from;
			int outputState = (int)to;
			int transition = (int)tr;
			FSMState fSMState = FindStateObjByName(num);
			fSMState.AddTransition(transition, outputState);
		}

		public int ApplyTransition(object tr)
		{
			//Discarded unreachable code: IL_006d
			lock (locker)
			{
				int transition = (int)tr;
				int num = currentStateName;
				currentStateName = FindStateObjByName(currentStateName).ApplyTransition(transition);
				if (num != currentStateName && onStateChange != null)
				{
					onStateChange(num, currentStateName);
				}
				return currentStateName;
			}
		}

		public int GetCurrentState()
		{
			//Discarded unreachable code: IL_0019
			lock (locker)
			{
				return currentStateName;
			}
		}

		public void SetCurrentState(object state)
		{
			int toStateName = (int)state;
			if (onStateChange != null)
			{
				onStateChange(currentStateName, toStateName);
			}
			currentStateName = toStateName;
		}

		private FSMState FindStateObjByName(object st)
		{
			int num = (int)st;
			foreach (FSMState state in states)
			{
				if (num.Equals(state.GetStateName()))
				{
					return state;
				}
			}
			return null;
		}
	}
}
