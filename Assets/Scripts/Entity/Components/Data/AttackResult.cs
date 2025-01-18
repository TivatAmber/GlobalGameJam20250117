namespace Entity.Components.Data
{
    /// <summary>
    /// 攻击结果枚举
    /// </summary>
    public enum AttackResult
    {
        Perfect,    // 完美击中
        Normal,     // 普通击中
        Fail,       // 攻击失败
        Puncture,      // 道具击中 （无视盾牌
    }
}