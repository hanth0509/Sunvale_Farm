namespace StateMachine.Player
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {

        }

        public override void Enter()
        {
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);

            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;
        }

        public override void Update()
        {
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            
        }

        public override void InitializeSubState()
        {
            
        }

        public override void CheckSwitchState()
        {
            if (!Context.MoveInputPress)
            {
                return;
            }

            // Giữ nút đi bộ thì vào Walk. Nếu thả nút đó thì chạy.
            if (Context.WalkInputPress)
            {
                SwitchState(StateFactory.Walk());
            }
            else
            {
                SwitchState(StateFactory.Run());
            }
        }
    }
}
