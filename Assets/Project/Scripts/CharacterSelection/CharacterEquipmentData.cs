[System.Serializable]
public class CharacterEquipmentData
{
    public int headItemNumber;
    public int bodyItemNumber;
    public int hairItemNumber;
    public int torsoItemNumber;
    public int legsItemNumber;

    public void ApplyToCharacterAnimator(CharacterAnimator animator)
    {
        animator.headItemNumber = headItemNumber;
        animator.bodyItemNumber = bodyItemNumber;
        animator.hairItemNumber = hairItemNumber;
        animator.torsoItemNumber = torsoItemNumber;
        animator.legsItemNumber = legsItemNumber;

        animator.RefreshCurrentFrame();
    }
}
