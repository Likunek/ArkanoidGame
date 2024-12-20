using System;

namespace ArkanoidGame
{
    public class GoldBrick : SimpleBrick
    {

        public GoldBrick(int value, int timesToBreak) : base("#C69245", value)
        {
            TimesToBreak = timesToBreak;
        }
    }
}