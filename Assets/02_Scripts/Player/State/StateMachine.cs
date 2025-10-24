using System;
using System.Collections.Generic;

public class StateMachine
{
    public IState CurrentState { get; private set; }
    private Dictionary<IState, List<Transition>> transitionDic = new Dictionary<IState, List<Transition>>();
    private List<Transition> anyTransitions = new List<Transition>();

    public void Update()
    {
        Transition transition = GetValidTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }
        CurrentState?.Update();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
    public void ChangeState(IState nextState)
    {
        if (CurrentState != null) CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (!transitionDic.ContainsKey(from)) transitionDic[from] = new List<Transition>();
        transitionDic[from].Add(new Transition(to, condition));
    }
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        anyTransitions.Add(new Transition(to, condition));
    }
    private Transition GetValidTransition()
    {
        //먼저 return 때리기 때문에 우선순위가 PlayerController에서 넣어주는대로 됨
        if (anyTransitions.Count != 0 && anyTransitions != null)
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Invoke())
                {
                    return transition;
                }
            }
        }
        //검사 후 전환조건이 없을 시, 부모의 전환조건도 검사
        IState state = CurrentState;

        while (state != null && state is PlayerState)
        {
            if (transitionDic.ContainsKey(state))
            {
                foreach (var transition in transitionDic[state])
                {
                    if (transition.Condition.Invoke())
                    {
                        return transition;
                    }
                }
            }
            state = (state as PlayerState).Parent;
        }
        return null;
    }
    private class Transition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }

    }
}
