using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MajorArcanaStacks {
    private readonly int _left = -1;
    private readonly int _right = 22;

    public MajorArcanaStacks() { }

    private MajorArcanaStacks(int left, int right)
    {
        _left = left;
        _right = right;
    }

    public bool TryAscend(MajorArcana card, out MajorArcanaStacks stacks) {
        var success = false;
        var newLeft = _left;
        if (card.Value == _left + 1) {
            newLeft += 1;
            success = true;
        }

        var newRight = _right;
        if (card.Value == _right - 1) {
            newRight -= 1;
            success = true;
        }

        stacks = success ? new MajorArcanaStacks(newLeft, newRight) : this;
        return success;
    }
}