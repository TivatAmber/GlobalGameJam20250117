namespace Entity.Components.Data
{
    /// <summary>
    /// 战斗动作枚举
    /// </summary>
    public enum CombatAction
    {
        None,
        AttackLeft,      // 左侧攻击
        AttackRight,     // 右侧攻击
        AttackDown,      // 下方攻击
        AttackUp,        // 上方攻击
        RuneConfirm      // 符文确认
    }
}