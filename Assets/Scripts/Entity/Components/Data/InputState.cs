namespace Entity.Components.Data
{
    /// <summary>
    /// 输入状态类，记录当前输入状态
    /// </summary>
    public class InputState
    {
        public CombatAction CurrentAction { get; set; }
        public bool IsActionTriggered { get; set; }
    }
}