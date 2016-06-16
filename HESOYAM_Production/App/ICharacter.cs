namespace App
{

    public interface ICharacter
    {
        void ReduceLife(float reduceBy);

        void IncreaseLife(float increaseBy);

        bool IsDead();
    }
}
