using UnityEngine;

namespace StateMachine.Player
{
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }

        public override void Enter()
        {
            this.InitializeSubState();

            this.Context.Animator.SetBool(this.Context.FallingAnimationHash, false);
            this.Context.CurrentMovementY = this.Context.Gravity;
            this.Context.ApplyMovementY = this.Context.Gravity;

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
            PlayerBaseState state;

            if (!Context.MoveInputPress)
            {
                state = StateFactory.Idle();
            }
            else if (Context.WalkInputPress)
            {
                state = StateFactory.Walk();
            }
            else
            {
                state = StateFactory.Run();
            }

            state.Enter();
            SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            if (Context.JumpInputPress && !Context.RequireJumpAgain)
            {
                SwitchState(StateFactory.Jump());
            }
            else if (!Context.CharacterController.isGrounded)
            {
                SwitchState(StateFactory.Fall());
            }
        }
      
    }
}
